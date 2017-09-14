using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static System.Half;

public class Fmdl
{
    private enum Section0BlockType
    {
        Bones = 0,
        MeshGroups = 1,
        ObjectAssignment = 2,
        MeshInfo = 3,
        Type4 = 4,
        BoneGroups = 5,
        Type6 = 6,
        TextureTypes = 7,
        Type8 = 8,
        Type9 = 9,
        MeshDataSectionInfo = 10,
        TypeB = 11,
        StringTable = 12,
        TypeD = 13,
        BufferOffsets = 14,
        LodInfo = 16,
        FaceIndices = 17,
        Type12 = 18,
        Type14 = 20,
        TextureList = 21,
        NameList = 22
    }; //Section0BlockType

    private enum Section1BlockType
    {
        Type0 = 0,
        MeshData = 2,
        Strings = 3
    }; //Section1BlockType

    private struct Section0Info
    {
        public ushort id;
        public ushort numEntries;
        public uint offset;
    } //struct

    private struct Section1Info
    {
        public uint id;
        public uint offset;
        public uint length;
    } //struct

    private struct Section0Block0Entry
    {
        public ushort nameId;
        public ushort parentId;
        public ushort unknown0; //id of some sort
        public ushort unknown1; //always 0x1?
        public float localPositionX;
        public float localPositionY;
        public float localPositionZ;
        public float locaPositionlW;
        public float worldPositionX;
        public float worldPositionY;
        public float worldPositionZ;
        public float worldPositionW;
    } //struct

    private struct Section0Block1Entry
    {
        public ushort nameId;
        public ushort invisibilityFlag;
        public ushort parentId;
        public ushort unknown0; //always 0xFF
    } //struct

    private struct Section0Block2Entry
    {
        public ushort meshGroupId;
        public ushort numObjects;
        public ushort numPrecedingObjects;
        public ushort id;
        public ushort materialId;
    } //struct

    private struct Section0Block3Entry
    {
        public uint unknown0;
        public ushort materialId;
        public ushort boneGroupId;
        public ushort id;
        public ushort numVertices;
        public uint numPrecedingFaceVertices;
        public uint numFaceVertices;
        public ulong unknown2; //probably related to section 0xA or 0x11
    } //struct

    private struct Section0Block5Entry
    {
        public ushort unknown0;
        public ushort numEntries;
        public ushort[] entries;
    } //struct

    private struct Section0Block6Entry
    {
        public ushort nameId;
        public ushort textureId;
    } //struct

    private struct Section0Block7Entry
    {
        public ushort nameId;
        public ushort referenceId;
    } //struct

    private struct Section0Block8Entry
    {
        public ushort nameId;
        public ushort materialNameId;
    } //struct

    private struct Section0BlockAEntry
    {
        public byte unknown0; //always 0 for first entry and 1 for others?
        public byte unknown1; //entry type 0 has 1. entry type 1 has 2. entry type 2 has 1. entry type 3 has 3.
        public byte length; //length for whatever data it's pointing to.
        public byte type; //seems to identify the type of data it's associated with. 1 is for the "vertex buffer" I think.
        public uint offset; //this offset is where the entry lands in its respective list.
    } //struct

    private struct Section0BlockCEntry
    {
        public ushort unknown0;
        public ushort length;
        public uint offset;
    } //struct

    private struct Section0BlockDEntry
    {
        public float[] entries;
    } //struct

    private struct Section0BlockEEntry
    {
        public uint unknown0;
        public uint length;
        public uint offset;
    } //struct

    private struct Section0Block10Entry
    {
        //variables here are assumptions. may not be correct.
        public uint unknown0;
        public float highDetailDistance;
        public float midDetailDistance;
        public float lowDetailDistance;
    } //struct

    private struct Vertex
    {
        public float x;
        public float y;
        public float z;
    } //struct

    private struct AdditionalVertexData
    {
        public Half normalX;
        public Half normalY;
        public Half normalZ;
        public Half normalW;

        public Half unknown0;
        public Half unknown1;
        public Half unknown2;
        public Half unknown3;

        public uint unknown4; //Dunno what this is. Always seems to be 00 00 00 FF though. So it might be colour.

        // These bytes are the bone weights, which are divided by 255
        public float boneWeightX;
        public float boneWeightY;
        public float boneWeightZ;
        public float boneWeightW;

        // These bytes correspond to a bone id in 0x5
        public byte boneGroup0Id;
        public byte boneGroup1Id;
        public byte boneGroup2Id;
        public byte boneGroup3Id;

        public Half textureU; //UV U coordinate.
        public Half textureV; //UV V coordinate.

        //Dunno what these are for.
        public Half unknown13;
        public Half unknown14;

        //These bytes add up to 0xFF. Seems like they're a second set of weights? Possibly LOD related?
        public byte unknown5;
        public byte unknown6;
        public byte unknown7;
        public byte unknown8;

        //These look like ids of some sort. Possibly LOD related?
        public ushort unknown9;
        public ushort unknown10;
        public ushort unknown11;
        public ushort unknown12;
    } //struct

    private struct Face
    {
        public ushort vertex1Id;
        public ushort vertex2Id;
        public ushort vertex3Id;
    } //struct

    private struct Object
    {
        public Vertex[] vertices;
        public AdditionalVertexData[] additionalVertexData;
        public Face[] faces;
    }

    private struct UnityMesh
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public BoneWeight[] boneWeights;
        public Vector2[] UVs;
        public int[] faces;
    }

    //local variables
    private uint signature;
    private uint unknown0;
    private ulong unknown1;
    private ulong unknown2;
    private ulong unknown3;
    private uint numSection0Blocks;
    private uint numSection1Blocks;
    private uint section0Offset;
    private uint section0Length;
    private uint section1Offset;
    private uint section1Length;

    private int bonesPosition = -1;
    private int meshGroupsPosition = -1;
    private int objectAssignmentPosition = -1;
    private int meshInfoPosition = -1;
    private int type4Position = -1;
    private int boneGroupsPosition = -1;
    private int type6Position = -1;
    private int textureTypesPosition = -1;
    private int type8Position = -1;
    private int type9Position = -1;
    private int meshDataSectionInfoPosition = -1;
    private int typeBPosition = -1;
    private int stringTablePosition = -1;
    private int typeDPosition = -1;
    private int bufferOffsetsPosition = -1;
    private int lodInfoPosition = -1;
    private int faceIndicesPosition = -1;
    private int type12Position = -1;
    private int type14Position = -1;
    private int textureListPosition = -1;
    private int nameListPosition = -1;

    private int unknownPosition = -1;
    private int meshDataPosition = -1;
    private int stringsPosition = -1;

    private Object[] objects;
    private UnityMesh[] unityMesh;
    private string[] strings;

    private Section0Info[] section0Info;
    private Section1Info[] section1Info;

    private Section0Block0Entry[] section0Block0Entries;
    private Section0Block1Entry[] section0Block1Entries;
    private Section0Block2Entry[] section0Block2Entries;
    private Section0Block3Entry[] section0Block3Entries;
    private Section0Block5Entry[] section0Block5Entries;
    private Section0Block6Entry[] section0Block6Entries;
    private Section0Block7Entry[] section0Block7Entries;
    private Section0Block8Entry[] section0Block8Entries;
    private Section0BlockAEntry[] section0BlockAEntries;
    private Section0BlockCEntry[] section0BlockCEntries;
    private Section0BlockDEntry[] section0BlockDEntries;
    private Section0BlockEEntry[] section0BlockEEntries;
    private Section0Block10Entry[] section0Block10Entries;
    private ulong[] section0Block15Entries;
    private ulong[] section0Block16Entries;

    public void Read(FileStream stream)
    {
        BinaryReader reader = new BinaryReader(stream, Encoding.Default);

        signature = reader.ReadUInt32();
        unknown0 = reader.ReadUInt32();
        unknown1 = reader.ReadUInt64();
        unknown2 = reader.ReadUInt64();
        unknown3 = reader.ReadUInt64();
        numSection0Blocks = reader.ReadUInt32();
        numSection1Blocks = reader.ReadUInt32();
        section0Offset = reader.ReadUInt32();
        section0Length = reader.ReadUInt32();
        section1Offset = reader.ReadUInt32();
        section1Length = reader.ReadUInt32();
        reader.BaseStream.Position += 0x8; //8 bytes of padding here.

        section0Info = new Section0Info[numSection0Blocks];

        //get the section0 info.
        for (int i = 0; i < section0Info.Length; i++)
        {
            section0Info[i].id = reader.ReadUInt16();
            section0Info[i].numEntries = reader.ReadUInt16();
            section0Info[i].offset = reader.ReadUInt32();

            switch (section0Info[i].id)
            {
                case (ushort)Section0BlockType.Bones:
                    bonesPosition = i;
                    section0Block0Entries = new Section0Block0Entry[section0Info[bonesPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshGroups:
                    meshGroupsPosition = i;
                    section0Block1Entries = new Section0Block1Entry[section0Info[meshGroupsPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.ObjectAssignment:
                    objectAssignmentPosition = i;
                    section0Block2Entries = new Section0Block2Entry[section0Info[objectAssignmentPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshInfo:
                    meshInfoPosition = i;
                    section0Block3Entries = new Section0Block3Entry[section0Info[meshInfoPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.Type4:
                    type4Position = i;
                    break;
                case (ushort)Section0BlockType.BoneGroups:
                    boneGroupsPosition = i;
                    section0Block5Entries = new Section0Block5Entry[section0Info[boneGroupsPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.Type6:
                    type6Position = i;
                    section0Block6Entries = new Section0Block6Entry[section0Info[type6Position].numEntries];
                    break;
                case (ushort)Section0BlockType.TextureTypes:
                    textureTypesPosition = i;
                    section0Block7Entries = new Section0Block7Entry[section0Info[textureTypesPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.Type8:
                    type8Position = i;
                    section0Block8Entries = new Section0Block8Entry[section0Info[type8Position].numEntries];
                    break;
                case (ushort)Section0BlockType.Type9:
                    type9Position = i;
                    break;
                case (ushort)Section0BlockType.MeshDataSectionInfo:
                    meshDataSectionInfoPosition = i;
                    section0BlockAEntries = new Section0BlockAEntry[section0Info[meshDataSectionInfoPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.TypeB:
                    typeBPosition = i;
                    break;
                case (ushort)Section0BlockType.StringTable:
                    stringTablePosition = i;
                    section0BlockCEntries = new Section0BlockCEntry[section0Info[stringTablePosition].numEntries];
                    strings = new string[section0Info[stringTablePosition].numEntries];
                    break;
                case (ushort)Section0BlockType.TypeD:
                    typeDPosition = i;
                    section0BlockDEntries = new Section0BlockDEntry[section0Info[typeDPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.BufferOffsets:
                    bufferOffsetsPosition = i;
                    section0BlockEEntries = new Section0BlockEEntry[section0Info[bufferOffsetsPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.LodInfo:
                    lodInfoPosition = i;
                    section0Block10Entries = new Section0Block10Entry[section0Info[lodInfoPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.FaceIndices:
                    faceIndicesPosition = i;
                    break;
                case (ushort)Section0BlockType.Type12:
                    type12Position = i;
                    break;
                case (ushort)Section0BlockType.Type14:
                    type14Position = i;
                    break;
                case (ushort)Section0BlockType.TextureList:
                    textureListPosition = i;
                    section0Block15Entries = new ulong[section0Info[textureListPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.NameList:
                    nameListPosition = i;
                    section0Block16Entries = new ulong[section0Info[nameListPosition].numEntries];
                    break;
                default:
                    break;
            } //switch
        } //for

        section1Info = new Section1Info[numSection1Blocks];

        //get the section1 info.
        for (int i = 0; i < section1Info.Length; i++)
        {
            section1Info[i].id = reader.ReadUInt32();
            section1Info[i].offset = reader.ReadUInt32();
            section1Info[i].length = reader.ReadUInt32();

            switch (section1Info[i].id)
            {
                case (uint)Section1BlockType.Type0:
                    unknownPosition = i;
                    break;
                case (uint)Section1BlockType.MeshData:
                    meshDataPosition = i;
                    break;
                case (uint)Section1BlockType.Strings:
                    stringsPosition = i;
                    break;
                default:
                    break;
            } //switch
        } //for

        objects = new Object[section0Info[meshInfoPosition].numEntries];
        unityMesh = new UnityMesh[section0Info[meshInfoPosition].numEntries];

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x0 - BONE DEFINITIONS
         *
         ****************************************************************/
        if (bonesPosition != -1)
        {
            //go to and get the section 0x0 entry info.
            reader.BaseStream.Position = section0Info[bonesPosition].offset + section0Offset;

            for (int i = 0; i < section0Block0Entries.Length; i++)
            {
                section0Block0Entries[i].nameId = reader.ReadUInt16();
                section0Block0Entries[i].parentId = reader.ReadUInt16();
                section0Block0Entries[i].unknown0 = reader.ReadUInt16();
                section0Block0Entries[i].unknown1 = reader.ReadUInt16();
                reader.BaseStream.Position += 0x8;
                section0Block0Entries[i].localPositionX = reader.ReadSingle();
                section0Block0Entries[i].localPositionY = reader.ReadSingle();
                section0Block0Entries[i].localPositionZ = reader.ReadSingle();
                section0Block0Entries[i].locaPositionlW = reader.ReadSingle();
                section0Block0Entries[i].worldPositionX = reader.ReadSingle();
                section0Block0Entries[i].worldPositionY = reader.ReadSingle();
                section0Block0Entries[i].worldPositionZ = reader.ReadSingle();
                section0Block0Entries[i].worldPositionW = reader.ReadSingle();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x1 - MESH GROUP DEFINITIONS
         *
         ****************************************************************/
        if (meshGroupsPosition != -1)
        {
            //go to and get the section 0x1 entry info.
            reader.BaseStream.Position = section0Info[meshGroupsPosition].offset + section0Offset;

            for (int i = 0; i < section0Block1Entries.Length; i++)
            {
                section0Block1Entries[i].nameId = reader.ReadUInt16();
                section0Block1Entries[i].invisibilityFlag = reader.ReadUInt16();
                section0Block1Entries[i].parentId = reader.ReadUInt16();
                section0Block1Entries[i].unknown0 = reader.ReadUInt16();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x2 - OBJECT ASSIGNMENT
         *
         ****************************************************************/
        if (objectAssignmentPosition != -1)
        {
            //go to and get the section 0x2 entry info.
            reader.BaseStream.Position = section0Info[objectAssignmentPosition].offset + section0Offset;

            for (int i = 0; i < section0Block2Entries.Length; i++)
            {
                reader.BaseStream.Position += 0x4;
                section0Block2Entries[i].meshGroupId = reader.ReadUInt16();
                section0Block2Entries[i].numObjects = reader.ReadUInt16();
                section0Block2Entries[i].numPrecedingObjects = reader.ReadUInt16();
                section0Block2Entries[i].id = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section0Block2Entries[i].materialId = reader.ReadUInt16();
                reader.BaseStream.Position += 0xE;
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x3 - OBJECT DATA
         *
         ****************************************************************/
        if (meshInfoPosition != -1)
        {
            //go to and get the section 0x3 entry info.
            reader.BaseStream.Position = section0Info[meshInfoPosition].offset + section0Offset;

            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                section0Block3Entries[i].unknown0 = reader.ReadUInt32();
                section0Block3Entries[i].materialId = reader.ReadUInt16();
                section0Block3Entries[i].boneGroupId = reader.ReadUInt16();
                section0Block3Entries[i].id = reader.ReadUInt16();
                section0Block3Entries[i].numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section0Block3Entries[i].numPrecedingFaceVertices = reader.ReadUInt32();
                section0Block3Entries[i].numFaceVertices = reader.ReadUInt32();
                section0Block3Entries[i].unknown2 = reader.ReadUInt64();
                reader.BaseStream.Position += 0x10;
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x5 - BONE GROUPS
         *
         ****************************************************************/
        if (boneGroupsPosition != -1)
        {
            //go to and get the section 0x5 entry info.
            reader.BaseStream.Position = section0Info[boneGroupsPosition].offset + section0Offset;

            for (int i = 0; i < section0Block5Entries.Length; i++)
            {
                section0Block5Entries[i].unknown0 = reader.ReadUInt16();
                section0Block5Entries[i].numEntries = reader.ReadUInt16();
                section0Block5Entries[i].entries = new ushort[section0Block5Entries[i].numEntries];

                for (int j = 0; j < section0Block5Entries[i].entries.Length; j++)
                    section0Block5Entries[i].entries[j] = reader.ReadUInt16();

                reader.BaseStream.Position += 0x40 - section0Block5Entries[i].numEntries * 2;
            } //for ends
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x6 - UNKNOWN - TEXTURE RELATED
         *
         ****************************************************************/
        if (type6Position != -1)
        {
            //go to and get the section 0x6 entry info.
            reader.BaseStream.Position = section0Info[type6Position].offset + section0Offset;

            for (int i = 0; i < section0Block6Entries.Length; i++)
            {
                section0Block6Entries[i].nameId = reader.ReadUInt16();
                section0Block6Entries[i].textureId = reader.ReadUInt16();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x7 - TEXTURE TYPE/MATERIAL PARAMETER ASSIGNMENT
         *
         ****************************************************************/
        if (textureTypesPosition != -1)
        {
            //go to and get the section 0x7 entry info.
            reader.BaseStream.Position = section0Info[textureTypesPosition].offset + section0Offset;

            for (int i = 0; i < section0Block7Entries.Length; i++)
            {
                section0Block7Entries[i].nameId = reader.ReadUInt16();
                section0Block7Entries[i].referenceId = reader.ReadUInt16();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x8 - UNKNOWN - MATERIAL RELATED
         *
         ****************************************************************/
        if (type8Position != -1)
        {
            //go to and get the section 0x8 entry info.
            reader.BaseStream.Position = section0Info[type8Position].offset + section0Offset;

            for (int i = 0; i < section0Block8Entries.Length; i++)
            {
                section0Block8Entries[i].nameId = reader.ReadUInt16();
                section0Block8Entries[i].materialNameId = reader.ReadUInt16();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xA - UNKNOWN - VERTEX DEFINITION RELATED
         *
         ****************************************************************/
        if (meshDataSectionInfoPosition != -1)
        {
            //go to and get the section 0xA entry info.
            reader.BaseStream.Position = section0Info[meshDataSectionInfoPosition].offset + section0Offset;

            for (int i = 0; i < section0BlockAEntries.Length; i++)
            {
                section0BlockAEntries[i].unknown0 = reader.ReadByte();
                section0BlockAEntries[i].unknown1 = reader.ReadByte();
                section0BlockAEntries[i].length = reader.ReadByte();
                section0BlockAEntries[i].type = reader.ReadByte();
                section0BlockAEntries[i].offset = reader.ReadUInt32();
            } //for
        } //if ends

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xC - STRING TABLE
         *
         ****************************************************************/
        if (stringTablePosition != -1)
        {
            //go to and get the section 0xD entry info.
            reader.BaseStream.Position = section0Info[stringTablePosition].offset + section0Offset;

            for (int i = 0; i < section0BlockCEntries.Length; i++)
            {
                section0BlockCEntries[i].unknown0 = reader.ReadUInt16();
                section0BlockCEntries[i].length = reader.ReadUInt16();
                section0BlockCEntries[i].offset = reader.ReadUInt32();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xD - UNKNOWN - FLOATS
         *
         ****************************************************************/
        if (typeDPosition != -1)
        {
            //go to and get the section 0xD entry info.
            reader.BaseStream.Position = section0Info[typeDPosition].offset + section0Offset;

            for (int i = 0; i < section0BlockDEntries.Length; i++)
            {
                section0BlockDEntries[i].entries = new float[8];

                for (int j = 0; j < section0BlockDEntries[i].entries.Length; j++)
                    section0BlockDEntries[i].entries[j] = reader.ReadSingle();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xD - UNKNOWN - FLOATS
         *
         ****************************************************************/
        if (typeDPosition != -1)
        {
            //go to and get the section 0xD entry info.
            reader.BaseStream.Position = section0Info[typeDPosition].offset + section0Offset;

            for (int i = 0; i < section0BlockDEntries.Length; i++)
            {
                section0BlockDEntries[i].entries = new float[8];

                for (int j = 0; j < section0BlockDEntries[i].entries.Length; j++)
                    section0BlockDEntries[i].entries[j] = reader.ReadSingle();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xE - BUFFER OFFSETS
         *
         ****************************************************************/
        if (bufferOffsetsPosition != -1)
        {
            //go to and get the section 0xE entry info.
            reader.BaseStream.Position = section0Info[bufferOffsetsPosition].offset + section0Offset;

            for (int i = 0; i < section0BlockEEntries.Length; i++)
            {
                section0BlockEEntries[i].unknown0 = reader.ReadUInt32();
                section0BlockEEntries[i].length = reader.ReadUInt32();
                section0BlockEEntries[i].offset = reader.ReadUInt32();
                reader.BaseStream.Position += 0x4;
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x10 - LOD Info
         *
         ****************************************************************/
        if (lodInfoPosition != -1)
        {
            //go to and get the section 0x10 entry info.
            reader.BaseStream.Position = section0Info[lodInfoPosition].offset + section0Offset;

            for (int i = 0; i < section0Block10Entries.Length; i++)
            {
                section0Block10Entries[i].unknown0 = reader.ReadUInt32();
                section0Block10Entries[i].highDetailDistance = reader.ReadSingle();
                section0Block10Entries[i].midDetailDistance = reader.ReadSingle();
                section0Block10Entries[i].lowDetailDistance = reader.ReadSingle();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK Ox15 - TEXTURE HASH LIST
         *
         ****************************************************************/
        if (textureListPosition != -1)
        {
            //go to and get the section 0x15 entry info.
            reader.BaseStream.Position = section0Info[textureListPosition].offset + section0Offset;

            for (int i = 0; i < section0Block15Entries.Length; i++)
            {
                section0Block15Entries[i] = reader.ReadUInt64();
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x16 - NAME HASH LIST
         *
         ****************************************************************/
        if (nameListPosition != -1)
        {
            //go to and get the section 0x16 entry info.
            reader.BaseStream.Position = section0Info[nameListPosition].offset + section0Offset;

            for (int i = 0; i < section0Block16Entries.Length; i++)
            {
                section0Block16Entries[i] = reader.ReadUInt64();
            } //for
        } //if

        /****************************************************************
         *
         * POSITION
         *
         ****************************************************************/
        reader.BaseStream.Position = section1Info[meshDataPosition].offset + section1Offset;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].vertices = new Vertex[section0Block3Entries[i].numVertices];

            for (int j = 0; j < objects[i].vertices.Length; j++)
            {
                objects[i].vertices[j].x = reader.ReadSingle();
                objects[i].vertices[j].y = reader.ReadSingle();
                objects[i].vertices[j].z = reader.ReadSingle();
            } //for

            //align the stream.
            if (reader.BaseStream.Position % 0x10 != 0)
                reader.BaseStream.Position += (0x10 - reader.BaseStream.Position % 0x10);
        } //for

        /****************************************************************
         *
         * ADDITIONAL VERTEX DATA
         *
         ****************************************************************/
        reader.BaseStream.Position = section0BlockEEntries[1].offset + section1Offset + section1Info[meshDataPosition].offset;

        int section0BlockACount = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            int type3Position = 0;
            int length = 0;
            int position = 0;

            objects[i].additionalVertexData = new AdditionalVertexData[section0Block3Entries[i].numVertices];

            while (section0BlockAEntries[section0BlockACount].type != 0)
                section0BlockACount++;

            if (section0BlockAEntries[section0BlockACount + 2].type == 2)
                type3Position = 3;
            else
                type3Position = 2;

            length = section0BlockAEntries[section0BlockACount + 1].length;

            if (section0BlockAEntries[section0BlockACount + 1].length != 0x10 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x14 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x18 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x1C &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x20 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x24 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x28 &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x2C &&
                section0BlockAEntries[section0BlockACount + 1].length != 0x30)
            {
                UnityEngine.Debug.Log("You've found an unknown additional vertex data length! Please report which model you used to BobDoleOwndU so he can analyze it.");
                UnityEngine.Debug.Log("Additional Info:\n" + "Id: " + section0BlockACount.ToString("x") + "\nLength: " + section0BlockAEntries[section0BlockACount + 1].length.ToString("x"));
                return;
            } //if

            for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
            {
                objects[i].additionalVertexData[j].normalX = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalY = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalZ = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalW = ToHalf(reader.ReadUInt16());

                position += 8;

                if (section0BlockAEntries[section0BlockACount + 1].unknown1 > 1)
                {
                    objects[i].additionalVertexData[j].unknown0 = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].unknown1 = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].unknown2 = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].unknown3 = ToHalf(reader.ReadUInt16());

                    position += 8;
                } //if ends

                if (type3Position == 3)
                {
                    objects[i].additionalVertexData[j].unknown4 = reader.ReadUInt32();
                    position += 4;
                } //if ends

                if ((section0BlockAEntries[section0BlockACount + 1].length == 0x10 && type3Position == 2) || section0BlockAEntries[section0BlockACount + 1].length > 0x18)
                {
                    objects[i].additionalVertexData[j].boneWeightX = reader.ReadByte() / 255.0f;
                    objects[i].additionalVertexData[j].boneWeightY = reader.ReadByte() / 255.0f;
                    objects[i].additionalVertexData[j].boneWeightZ = reader.ReadByte() / 255.0f;
                    objects[i].additionalVertexData[j].boneWeightW = reader.ReadByte() / 255.0f;
                    objects[i].additionalVertexData[j].boneGroup0Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup1Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup2Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup3Id = reader.ReadByte();

                    position += 8;
                } //if

                if (!(section0BlockAEntries[section0BlockACount + 1].length == 0x10 && type3Position == 2))
                {
                    objects[i].additionalVertexData[j].textureU = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].textureV = -ToHalf(reader.ReadUInt16()); //Value is negated.
                    position += 4;
                } //if

                if ((section0BlockAEntries[section0BlockACount + 1].length == 0x18 && type3Position == 2) ||
                    (section0BlockAEntries[section0BlockACount + 1].length == 0x20 && type3Position == 2 && section0BlockAEntries[section0BlockACount + 1].unknown1 != 1) ||
                    section0BlockAEntries[section0BlockACount + 1].length == 0x24 ||
                    section0BlockAEntries[section0BlockACount + 1].length == 0x30)
                {
                    objects[i].additionalVertexData[j].unknown13 = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].unknown14 = ToHalf(reader.ReadUInt16());

                    position += 4;
                } //if ends

                if ((section0BlockAEntries[section0BlockACount + 1].length == 0x20 && type3Position == 2 && section0BlockAEntries[section0BlockACount + 1].unknown1 == 1) ||
                    section0BlockAEntries[section0BlockACount + 1].length == 0x28 ||
                    section0BlockAEntries[section0BlockACount + 1].length == 0x2C ||
                    section0BlockAEntries[section0BlockACount + 1].length == 0x30)
                {
                    objects[i].additionalVertexData[j].unknown5 = reader.ReadByte();
                    objects[i].additionalVertexData[j].unknown6 = reader.ReadByte();
                    objects[i].additionalVertexData[j].unknown7 = reader.ReadByte();
                    objects[i].additionalVertexData[j].unknown8 = reader.ReadByte();
                    objects[i].additionalVertexData[j].unknown9 = reader.ReadUInt16();
                    objects[i].additionalVertexData[j].unknown10 = reader.ReadUInt16();
                    objects[i].additionalVertexData[j].unknown11 = reader.ReadUInt16();
                    objects[i].additionalVertexData[j].unknown12 = reader.ReadUInt16();

                    position += 12;
                } //if

                if (position != length)
                {
                    UnityEngine.Debug.Log("i: " + i);
                    UnityEngine.Debug.Log("j: " + j);
                    UnityEngine.Debug.Log("Section0BlockACount: " + section0BlockACount);
                    UnityEngine.Debug.Log("Expected Length: " + length);
                    UnityEngine.Debug.Log("Calculated Length: " + position);
                } //if
                position = 0;
            } //for

            //align the stream.
            if (reader.BaseStream.Position % 0x10 != 0)
                reader.BaseStream.Position += (0x10 - reader.BaseStream.Position % 0x10);

            section0BlockACount++;
        } //for

        /****************************************************************
         *
         * FACES
         *
         ****************************************************************/

        for (int i = 0; i < objects.Length; i++)
        {
            reader.BaseStream.Position = section0BlockEEntries[2].offset + section1Offset + section1Info[meshDataPosition].offset + section0Block3Entries[i].numPrecedingFaceVertices * 2;

            objects[i].faces = new Face[section0Block3Entries[i].numFaceVertices / 3];

            for (int j = 0; j < objects[i].faces.Length; j++)
            {
                objects[i].faces[j].vertex1Id = reader.ReadUInt16();
                objects[i].faces[j].vertex2Id = reader.ReadUInt16();
                objects[i].faces[j].vertex3Id = reader.ReadUInt16();
            } //for
        } //for

        /****************************************************************
         *
         * STRINGS
         *
         ****************************************************************/
        if (stringTablePosition != -1)
        {
            for(int i = 0; i < section0BlockCEntries.Length; i++)
            {
                reader.BaseStream.Position = section1Offset + section1Info[stringsPosition].offset + section0BlockCEntries[i].offset;
                strings[i] = Encoding.Default.GetString(reader.ReadBytes(section0BlockCEntries[i].length));
            } //for
        } //if
    } //Read

    [Conditional("DEBUG")]
    public void OutputSection0Block0Info()
    {
        for (int i = 0; i < section0Block0Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry ID: " + i);
            Console.WriteLine("Bone Name: " + Hashing.TryGetName(section0Block16Entries[section0Block0Entries[i].nameId]));
            Console.Write("Parent Bone: ");

            if (section0Block0Entries[i].parentId != 0xFFFF)
                Console.WriteLine(Hashing.TryGetName(section0Block16Entries[section0Block0Entries[section0Block0Entries[i].parentId].nameId]));
            else
                Console.WriteLine("Root");
        } //for
    } //OutputSection0Block0Info

    public void OutputSection0Block2Info()
    {
        for (int i = 0; i < section0Block2Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry ID: " + section0Block2Entries[i].id);
            Console.WriteLine("Mesh Group: " + Hashing.TryGetName(section0Block16Entries[section0Block1Entries[section0Block2Entries[i].meshGroupId].nameId]));
            Console.WriteLine("Number of Objects: " + section0Block2Entries[i].numObjects);
            Console.WriteLine("Number of Preceding Objects: " + section0Block2Entries[i].numPrecedingObjects);
            Console.WriteLine("Material ID: " + section0Block2Entries[i].materialId);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block3Info()
    {
        for (int i = 0; i < section0Block3Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Unknown 0: " + section0Block3Entries[i].unknown0);
            Console.WriteLine("Material Id: " + section0Block3Entries[i].materialId);
            Console.WriteLine("Bone Group Id: " + section0Block3Entries[i].boneGroupId);
            Console.WriteLine("Id: " + section0Block3Entries[i].id);
            Console.WriteLine("Num Vertices " + section0Block3Entries[i].numVertices);
            Console.WriteLine("Face Offset: " + section0Block3Entries[i].numPrecedingFaceVertices);
            Console.WriteLine("Num Face Vertices: " + section0Block3Entries[i].numFaceVertices);
            Console.WriteLine("Unknown 2: " + section0Block3Entries[i].unknown2);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block5Info()
    {
        ushort greatestUnknown0 = 0;
        ushort greatestEntry = 0;
        for (int i = 0; i < section0Block5Entries.Length; i++)
        {
            if (section0Block5Entries[i].unknown0 > greatestUnknown0)
                greatestUnknown0 = section0Block5Entries[i].unknown0;

            for (int j = 0; j < section0Block5Entries[i].entries.Length; j++)
                if (section0Block5Entries[i].entries[j] > greatestEntry)
                    greatestEntry = section0Block5Entries[i].entries[j];
        } //for

        Console.WriteLine("The greatest unknown0 is: " + greatestUnknown0.ToString("x"));
        Console.WriteLine("The greatest entry is: " + greatestEntry.ToString("x"));
    } //OutputSection2Info

    public void OutputSection0Block7Info()
    {
        for (int i = 0; i < section0Block7Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Texture Type/Material Parameter Hash: " + section0Block16Entries[section0Block7Entries[i].nameId].ToString("x"));
            Console.WriteLine("Reference Id: " + section0Block7Entries[i].referenceId);
        } //for
    } //OutputSection2Info

    public void OutputSection0Block8Info()
    {
        for (int i = 0; i < section0Block8Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Unknown Hash: " + (section0Block16Entries[section0Block8Entries[i].nameId]).ToString("x"));
            Console.WriteLine("Material Hash: " + (section0Block16Entries[section0Block8Entries[i].materialNameId]).ToString("x"));
        } //for
    } //OutputSection2Info

    public void OutputSection0BlockDInfo()
    {
        for (int i = 0; i < section0BlockDEntries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.Write("Floats: [");

            for (int j = 0; j < section0BlockDEntries[i].entries.Length; j++)
            {
                Console.Write(section0BlockDEntries[i].entries[j]);

                if (j != section0BlockDEntries[i].entries.Length - 1)
                    Console.Write(", ");
            } //for

            Console.WriteLine("]");
        } //for
    } //OutputSection2Info

    public void OutputSection0Block16Info()
    {
        for (int i = 0; i < section0Block16Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Hash: " + section0Block16Entries[i].ToString("x"));
        } //for
    } //OutputSection2Info

    public void OutputObjectInfo2()
    {
        int greatest = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
            {
                if (objects[i].additionalVertexData[j].boneGroup0Id > greatest)
                    greatest = objects[i].additionalVertexData[j].boneGroup0Id;
                if (objects[i].additionalVertexData[j].boneGroup1Id > greatest)
                    greatest = objects[i].additionalVertexData[j].boneGroup1Id;
                if (objects[i].additionalVertexData[j].boneGroup2Id > greatest)
                    greatest = objects[i].additionalVertexData[j].boneGroup2Id;
                if (objects[i].additionalVertexData[j].boneGroup3Id > greatest)
                    greatest = objects[i].additionalVertexData[j].boneGroup3Id;
            } //for
        } //for

        UnityEngine.Debug.Log("Greatest Bone Group Id: " + greatest);
    } //OutputObjectInfo

    public void OutputStringInfo()
    {
        for(int i = 0; i < strings.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("String: " + strings[i]);
        } //for
    } //OutputStringInfo

    public void MeshReader(string filepath)
    {
        //Testing. Need a more permanent solution.
        if(File.Exists(@"D:\Games\MGSV Research\Mods\FmdlTool\FmdlTool\FmdlTool\bin\Debug\dictionary.txt"))
            Hashing.ReadDictionary(Path.GetDirectoryName(Assembly.GetExecutingAssembly().ToString()) + "fmdl_dictionary.txt");

        GameObject fmdlGameObject = new GameObject();
        string fmdlName = Path.GetFileNameWithoutExtension(filepath);
        fmdlGameObject.name = fmdlName;
        GameObject[] subFmdlGameObjects = new GameObject[objects.Length];
        Transform[] bones;
        Matrix4x4[] bindPoses;

        if (bonesPosition != -1)
        {
            bones = new Transform[section0Block0Entries.Length];
            bindPoses = new Matrix4x4[section0Block0Entries.Length];
        } //if
        else
        {
            bones = new Transform[0];
            bindPoses = new Matrix4x4[0];
        } //else

        //UnityEngine.Debug.Log("This is the bone definition section's end.");

        AssetDatabase.CreateFolder("Assets", fmdlName + "Materials");

        for (int i = 0; i < section0Block8Entries.Length; i++)
        {
            Material material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, "Assets/" + fmdlName + "Materials/" + Hashing.TryGetName(section0Block16Entries[section0Block8Entries[i].nameId]) + " (" + Hashing.TryGetName(section0Block16Entries[section0Block8Entries[i].materialNameId]) + ")"+ ".mat");
        }

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject(Hashing.TryGetName(section0Block16Entries[section0Block0Entries[i].nameId])).transform;
            bones[i].position = new Vector3(section0Block0Entries[i].worldPositionZ, section0Block0Entries[i].worldPositionY, section0Block0Entries[i].worldPositionX);

            if (section0Block0Entries[i].parentId == 0xFFFF)
                bones[i].parent = fmdlGameObject.transform;
            else
            {
                bones[i].parent = bones[section0Block0Entries[i].parentId];

                /*Matrix4x4 matrix0 = bones[i].localToWorldMatrix;
                Matrix4x4 matrix1 = bones[i].parent.localToWorldMatrix;

                matrix0 *= matrix1;

                bones[i].position = new Vector3(matrix0.m03, matrix0.m13, matrix0.m23);*/
            } //else ends
        } //for

        //UnityEngine.Debug.Log("This is the bone section's end.");

        for (int i = 0; i < objects.Length; i++)
        {
            unityMesh[i].vertices = new Vector3[objects[i].vertices.Length];
            unityMesh[i].normals = new Vector3[objects[i].additionalVertexData.Length];
            unityMesh[i].UVs = new Vector2[objects[i].additionalVertexData.Length];
            unityMesh[i].faces = new int[objects[i].faces.Length * 3];
            unityMesh[i].boneWeights = new BoneWeight[objects[i].additionalVertexData.Length];

            //Position
            for (int j = 0; j < objects[i].vertices.Length; j++)
                unityMesh[i].vertices[j] = new Vector3(objects[i].vertices[j].z, objects[i].vertices[j].y, objects[i].vertices[j].x);
            //UnityEngine.Debug.Log("This is the position section's end.");

            //Normals, Bone Weights, Bone Group Ids and UVs
            for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
            {
                unityMesh[i].normals[j] = new Vector3(objects[i].additionalVertexData[j].normalZ, objects[i].additionalVertexData[j].normalY, objects[i].additionalVertexData[j].normalX);

                if (bonesPosition != -1)
                {
                    unityMesh[i].boneWeights[j].weight0 = objects[i].additionalVertexData[j].boneWeightX;
                    unityMesh[i].boneWeights[j].weight1 = objects[i].additionalVertexData[j].boneWeightY;
                    unityMesh[i].boneWeights[j].weight2 = objects[i].additionalVertexData[j].boneWeightZ;
                    unityMesh[i].boneWeights[j].weight3 = objects[i].additionalVertexData[j].boneWeightW;

                    /*UnityEngine.Debug.Log("i: " + i + " j: " + j);
                    UnityEngine.Debug.Log("Block 5 Entries: " + section0Block5Entries.Length);
                    UnityEngine.Debug.Log("Block 3 Bone Group Id: " + section0Block3Entries[i].boneGroupId);
                    UnityEngine.Debug.Log("Block 5 subentries length: " + section0Block5Entries[section0Block3Entries[i].boneGroupId].entries.Length);
                    UnityEngine.Debug.Log("Bone Group 0 Id: " + objects[i].additionalVertexData[j].boneGroup0Id);*/
                    unityMesh[i].boneWeights[j].boneIndex0 = section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[objects[i].additionalVertexData[j].boneGroup0Id];
                    unityMesh[i].boneWeights[j].boneIndex1 = section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[objects[i].additionalVertexData[j].boneGroup1Id];
                    unityMesh[i].boneWeights[j].boneIndex2 = section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[objects[i].additionalVertexData[j].boneGroup2Id];
                    unityMesh[i].boneWeights[j].boneIndex3 = section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[objects[i].additionalVertexData[j].boneGroup3Id];
                } //if

                unityMesh[i].UVs[j] = new Vector2(objects[i].additionalVertexData[j].textureU, objects[i].additionalVertexData[j].textureV);
            } //for

            //UnityEngine.Debug.Log("This is the additional vertex data section's end.");

            //Faces
            for (int j = 0, h = 0; j < objects[i].faces.Length; j++, h += 3)
            {
                unityMesh[i].faces[h] = objects[i].faces[j].vertex1Id;
                unityMesh[i].faces[h + 1] = objects[i].faces[j].vertex2Id;
                unityMesh[i].faces[h + 2] = objects[i].faces[j].vertex3Id;
            } //for
            //UnityEngine.Debug.Log("This is the face section's end.");

            //Render the mesh in Unity.
            subFmdlGameObjects[i] = new GameObject();

            //Get the mesh name.
            for (int j = 0; j < section0Block2Entries.Length; j++)
            {
                if (i >= section0Block2Entries[j].numPrecedingObjects && i < section0Block2Entries[j].numPrecedingObjects + section0Block2Entries[j].numObjects)
                {
                    subFmdlGameObjects[i].name = i + " (" + Hashing.TryGetName(section0Block16Entries[section0Block1Entries[section0Block2Entries[j].meshGroupId].nameId]) + ")";
                    break;
                } //if
            } //for

            subFmdlGameObjects[i].transform.parent = fmdlGameObject.transform;
            //MeshFilter meshFilter = subFmdlGameObjects[i].AddComponent<MeshFilter>();
            SkinnedMeshRenderer meshRenderer = subFmdlGameObjects[i].AddComponent<SkinnedMeshRenderer>();

            //meshRenderer.rootBone = bones[0];
            string fmdlMaterial = "Assets/" + fmdlName + "Materials/" + Hashing.TryGetName(section0Block16Entries[section0Block3Entries[i].materialId]) + ".mat";
            Material material = (Material)AssetDatabase.LoadMainAssetAtPath(fmdlMaterial);
            meshRenderer.material = material;
            //UnityEngine.Debug.Log("This is the mesh creation section's end.");

            Mesh mesh = new Mesh();
            mesh.vertices = unityMesh[i].vertices;
            mesh.uv = unityMesh[i].UVs;
            mesh.normals = unityMesh[i].normals;
            mesh.triangles = unityMesh[i].faces;
            mesh.boneWeights = unityMesh[i].boneWeights;

            for (int j = 0; j < bones.Length; j++)
            {
                bindPoses[j] = bones[j].worldToLocalMatrix * subFmdlGameObjects[i].transform.localToWorldMatrix;
            } //for

            mesh.bindposes = bindPoses;
            //UnityEngine.Debug.Log("This is the mesh section's end.");

            //meshFilter.mesh = mesh;
            meshRenderer.bones = bones;
            meshRenderer.sharedMesh = mesh;
            subFmdlGameObjects[i].AddComponent<MeshCollider>();
        } //for
    } //MeshReader
} //class