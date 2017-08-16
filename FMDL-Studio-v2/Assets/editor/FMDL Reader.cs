using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Half;
using UnityEngine;
using System.Collections.Generic;

public class Fmdl
{
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
        public float positionX;
        public float positionY;
        public float positionZ;
        public float positionW;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float rotationW;
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
        public ushort unknown1; //probably related to section 0x4
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
        public ushort textureId;
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

        public float unknown4;

        public Half textureU; //UV U coordinate
        public Half textureV; //UV V coordinate

        public float unknown5;
        public float unknown6;
        public float unknown7;
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
        //public BoneWeight[] boneWeights;
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

    private Object[] objects;
    private UnityMesh[] unityMesh;

    /*
     * There are 20 (0x14) sections in The Phantom Pain's models. Sections 0xC, 0xF and 0x13 do not exist.
     * 0 = 0x0
     * 1 = 0x1
     * 2 = 0x2
     * 3 = 0x3
     * 4 = 0x4
     * 5 = 0x5
     * 6 = 0x6
     * 7 = 0x7
     * 8 = 0x8
     * 9 = 0x9
     * 10 = 0xA
     * 11 = 0xB
     * 12 = 0xD
     * 13 = 0xE
     * 14 = 0x10
     * 15 = 0x11
     * 16 = 0x12
     * 17 = 0x14
     * 18 = 0x15
     * 19 = 0x16
     */
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
    private Section0BlockDEntry[] section0BlockDEntries;
    private Section0BlockEEntry[] section0BlockEEntries;
    private Section0Block10Entry[] section0Block10Entries;
    private ulong[] section0Block15Entries;
    private ulong[] section0Block16Entries;

    public void Read(FileStream stream)
    {
        BinaryReader reader = new BinaryReader(stream, Encoding.Default, true);

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
        } //for

        section1Info = new Section1Info[numSection1Blocks];

        //get the section1 info.
        for (int i = 0; i < section1Info.Length; i++)
        {
            section1Info[i].id = reader.ReadUInt32();
            section1Info[i].offset = reader.ReadUInt32();
            section1Info[i].length = reader.ReadUInt32();
        } //for

        section0Block0Entries = new Section0Block0Entry[section0Info[0].numEntries];
        section0Block1Entries = new Section0Block1Entry[section0Info[1].numEntries];
        section0Block2Entries = new Section0Block2Entry[section0Info[2].numEntries];
        section0Block3Entries = new Section0Block3Entry[section0Info[3].numEntries];
        section0Block5Entries = new Section0Block5Entry[section0Info[5].numEntries];
        section0Block6Entries = new Section0Block6Entry[section0Info[6].numEntries];
        section0Block7Entries = new Section0Block7Entry[section0Info[7].numEntries];
        section0Block8Entries = new Section0Block8Entry[section0Info[8].numEntries];
        section0BlockAEntries = new Section0BlockAEntry[section0Info[10].numEntries];
        section0BlockDEntries = new Section0BlockDEntry[section0Info[12].numEntries];
        section0BlockEEntries = new Section0BlockEEntry[section0Info[13].numEntries];
        section0Block10Entries = new Section0Block10Entry[section0Info[14].numEntries];
        section0Block15Entries = new ulong[section0Info[18].numEntries];
        section0Block16Entries = new ulong[section0Info[19].numEntries];

        objects = new Object[section0Info[3].numEntries];
        unityMesh = new UnityMesh[section0Info[3].numEntries];

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x0 - BONE DEFINITIONS
         *
         ****************************************************************/
        //go to and get the section 0x0 entry info.
        reader.BaseStream.Position = section0Info[0].offset + section0Offset;

        for (int i = 0; i < section0Block0Entries.Length; i++)
        {
            section0Block0Entries[i].nameId = reader.ReadUInt16();
            section0Block0Entries[i].parentId = reader.ReadUInt16();
            section0Block0Entries[i].unknown0 = reader.ReadUInt16();
            section0Block0Entries[i].unknown1 = reader.ReadUInt16();
            reader.BaseStream.Position += 0x8;
            section0Block0Entries[i].positionX = reader.ReadSingle();
            section0Block0Entries[i].positionY = reader.ReadSingle();
            section0Block0Entries[i].positionZ = reader.ReadSingle();
            section0Block0Entries[i].positionW = reader.ReadSingle();
            section0Block0Entries[i].rotationX = reader.ReadSingle();
            section0Block0Entries[i].rotationY = reader.ReadSingle();
            section0Block0Entries[i].rotationZ = reader.ReadSingle();
            section0Block0Entries[i].rotationW = reader.ReadSingle();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x1 - MESH GROUP DEFINITIONS
         *
         ****************************************************************/
        //go to and get the section 0x1 entry info.
        reader.BaseStream.Position = section0Info[1].offset + section0Offset;

        for (int i = 0; i < section0Block1Entries.Length; i++)
        {
            section0Block1Entries[i].nameId = reader.ReadUInt16();
            section0Block1Entries[i].invisibilityFlag = reader.ReadUInt16();
            section0Block1Entries[i].parentId = reader.ReadUInt16();
            section0Block1Entries[i].unknown0 = reader.ReadUInt16();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x2 - OBJECT ASSIGNMENT
         *
         ****************************************************************/
        //go to and get the section 0x2 entry info.
        reader.BaseStream.Position = section0Info[2].offset + section0Offset;

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

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x3 - OBJECT DATA
         *
         ****************************************************************/
        //go to and get the section 0x3 entry info.
        reader.BaseStream.Position = section0Info[3].offset + section0Offset;

        for (int i = 0; i < section0Block3Entries.Length; i++)
        {
            section0Block3Entries[i].unknown0 = reader.ReadUInt32();
            section0Block3Entries[i].unknown1 = reader.ReadUInt16();
            section0Block3Entries[i].boneGroupId = reader.ReadUInt16();
            section0Block3Entries[i].id = reader.ReadUInt16();
            section0Block3Entries[i].numVertices = reader.ReadUInt16();
            reader.BaseStream.Position += 0x4;
            section0Block3Entries[i].numPrecedingFaceVertices = reader.ReadUInt32();
            section0Block3Entries[i].numFaceVertices = reader.ReadUInt32();
            section0Block3Entries[1].unknown2 = reader.ReadUInt64();
            reader.BaseStream.Position += 0x10;
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x5 - BONE GROUPS
         *
         ****************************************************************/
        //go to and get the section 0x5 entry info.
        reader.BaseStream.Position = section0Info[5].offset + section0Offset;

        for (int i = 0; i < section0Block5Entries.Length; i++)
        {
            section0Block5Entries[i].unknown0 = reader.ReadUInt16();
            section0Block5Entries[i].numEntries = reader.ReadUInt16();
            section0Block5Entries[i].entries = new ushort[section0Block5Entries[i].numEntries];

            for (int j = 0; j < section0Block5Entries[i].entries.Length; j++)
                section0Block5Entries[i].entries[j] = reader.ReadUInt16();

            reader.BaseStream.Position += 0x40 - section0Block5Entries[i].numEntries * 2;
        } //for ends

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x6 - UNKNOWN - TEXTURE RELATED
         *
         ****************************************************************/
        //go to and get the section 0x6 entry info.
        reader.BaseStream.Position = section0Info[6].offset + section0Offset;

        for (int i = 0; i < section0Block6Entries.Length; i++)
        {
            section0Block6Entries[i].nameId = reader.ReadUInt16();
            section0Block6Entries[i].textureId = reader.ReadUInt16();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x7 - TEXTURE TYPE ASSIGNMENT
         *
         ****************************************************************/
        //go to and get the section 0x7 entry info.
        reader.BaseStream.Position = section0Info[7].offset + section0Offset;

        for (int i = 0; i < section0Block7Entries.Length; i++)
        {
            section0Block7Entries[i].nameId = reader.ReadUInt16();
            section0Block7Entries[i].textureId = reader.ReadUInt16();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x8 - UNKNOWN - MATERIAL RELATED
         *
         ****************************************************************/
        //go to and get the section 0x8 entry info.
        reader.BaseStream.Position = section0Info[8].offset + section0Offset;

        for (int i = 0; i < section0Block8Entries.Length; i++)
        {
            section0Block8Entries[i].nameId = reader.ReadUInt16();
            section0Block8Entries[i].materialNameId = reader.ReadUInt16();
        } //for


        /****************************************************************
         *
         * SECTION 0 BLOCK 0xA - UNKNOWN - VERTEX DEFINITION RELATED
         *
         ****************************************************************/
        //go to and get the section 0xA entry info.
        reader.BaseStream.Position = section0Info[10].offset + section0Offset;

        for (int i = 0; i < section0BlockAEntries.Length; i++)
        {
            section0BlockAEntries[i].unknown0 = reader.ReadByte();
            section0BlockAEntries[i].unknown1 = reader.ReadByte();
            section0BlockAEntries[i].length = reader.ReadByte();
            section0BlockAEntries[i].type = reader.ReadByte();
            section0BlockAEntries[i].offset = reader.ReadUInt32();
        } //for


        /****************************************************************
         *
         * SECTION 0 BLOCK 0xD - UNKNOWN - FLOATS
         *
         ****************************************************************/
        //go to and get the section 0xD entry info.
        reader.BaseStream.Position = section0Info[12].offset + section0Offset;

        for (int i = 0; i < section0BlockDEntries.Length; i++)
        {
            section0BlockDEntries[i].entries = new float[8];

            for (int j = 0; j < section0BlockDEntries[i].entries.Length; j++)
                section0BlockDEntries[i].entries[j] = reader.ReadSingle();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xE - BUFFER OFFSETS
         *
         ****************************************************************/
        //go to and get the section 0xE entry info.
        reader.BaseStream.Position = section0Info[13].offset + section0Offset;

        for (int i = 0; i < section0BlockEEntries.Length; i++)
        {
            section0BlockEEntries[i].unknown0 = reader.ReadUInt32();
            section0BlockEEntries[i].length = reader.ReadUInt32();
            section0BlockEEntries[i].offset = reader.ReadUInt32();
            reader.BaseStream.Position += 0x4;
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x10 - LOD Camera Distances
         *
         ****************************************************************/
        //go to and get the section 0x10 entry info.
        reader.BaseStream.Position = section0Info[14].offset + section0Offset;

        for (int i = 0; i < section0Block10Entries.Length; i++)
        {
            section0Block10Entries[i].unknown0 = reader.ReadUInt32();
            section0Block10Entries[i].highDetailDistance = reader.ReadSingle();
            section0Block10Entries[i].midDetailDistance = reader.ReadSingle();
            section0Block10Entries[i].lowDetailDistance = reader.ReadSingle();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK Ox15 - TEXTURE HASH LIST
         *
         ****************************************************************/
        //go to and get the section 0x15 entry info.
        reader.BaseStream.Position = section0Info[18].offset + section0Offset;

        for (int i = 0; i < section0Block15Entries.Length; i++)
        {
            section0Block15Entries[i] = reader.ReadUInt64();
        } //for

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x16 - NAME HASH LIST
         *
         ****************************************************************/
        //go to and get the section 0x16 entry info.
        reader.BaseStream.Position = section0Info[19].offset + section0Offset;

        for (int i = 0; i < section0Block16Entries.Length; i++)
        {
            section0Block16Entries[i] = reader.ReadUInt64();
        } //for

        /****************************************************************
         *
         * POSITION
         *
         ****************************************************************/
        reader.BaseStream.Position = section1Info[1].offset + section1Offset;

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
        reader.BaseStream.Position = section0BlockEEntries[1].offset + section1Offset + section1Info[1].offset;

        int section0BlockACount = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].additionalVertexData = new AdditionalVertexData[section0Block3Entries[i].numVertices];

            while (section0BlockAEntries[section0BlockACount].length != 1)
                section0BlockACount++;

            for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
            {
                objects[i].additionalVertexData[j].normalX = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalY = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalZ = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].normalW = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].unknown0 = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].unknown1 = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].unknown2 = ToHalf(reader.ReadUInt16());
                objects[i].additionalVertexData[j].unknown3 = ToHalf(reader.ReadUInt16());

                if (section0BlockAEntries[section0BlockACount].length == 0x1C ||
                    section0BlockAEntries[section0BlockACount].length == 0x20 ||
                    section0BlockAEntries[section0BlockACount].length == 0x24 ||
                    section0BlockAEntries[section0BlockACount].length == 0x28 ||
                    section0BlockAEntries[section0BlockACount].length == 0x2C)
                {
                    objects[i].additionalVertexData[j].boneWeightX = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneWeightY = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneWeightZ = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneWeightW = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup0Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup1Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup2Id = reader.ReadByte();
                    objects[i].additionalVertexData[j].boneGroup3Id = reader.ReadByte();

                    if (section0BlockAEntries[section0BlockACount].length == 0x20 ||
                        section0BlockAEntries[section0BlockACount].length == 0x2C)
                    {
                        objects[i].additionalVertexData[j].unknown4 = reader.ReadSingle();
                    } //if

                    objects[i].additionalVertexData[j].textureU = ToHalf(reader.ReadUInt16());
                    objects[i].additionalVertexData[j].textureV = ToHalf(reader.ReadUInt16()) * -1; //value is negated.

                    if (section0BlockAEntries[section0BlockACount].length == 0x24)
                    {
                        objects[i].additionalVertexData[j].unknown5 = reader.ReadSingle();
                        objects[i].additionalVertexData[j].unknown6 = reader.ReadSingle();
                    } //if

                    if (section0BlockAEntries[section0BlockACount].length == 0x28 ||
                        section0BlockAEntries[section0BlockACount].length == 0x2C)
                    {
                        objects[i].additionalVertexData[j].unknown5 = reader.ReadSingle();
                        objects[i].additionalVertexData[j].unknown6 = reader.ReadSingle();
                        objects[i].additionalVertexData[j].unknown7 = reader.ReadSingle();
                    } //if
                } //if
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
            reader.BaseStream.Position = section0BlockEEntries[2].offset + section1Offset + section1Info[1].offset + section0Block3Entries[i].numPrecedingFaceVertices * 2;

            objects[i].faces = new Face[section0Block3Entries[i].numFaceVertices / 3];

            for (int j = 0; j < objects[i].faces.Length; j++)
            {
                objects[i].faces[j].vertex1Id = reader.ReadUInt16();
                objects[i].faces[j].vertex2Id = reader.ReadUInt16();
                objects[i].faces[j].vertex3Id = reader.ReadUInt16();
            } //for
        } //for
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
            Console.WriteLine("Unknown 1: " + section0Block3Entries[i].unknown1);
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
            Console.WriteLine("Texture Type Hash: " + section0Block16Entries[section0Block7Entries[i].nameId].ToString("x"));
            Console.WriteLine("Texture Hash: " + (section0Block15Entries[section0Block7Entries[i].textureId] - 0x1568000000000000).ToString("x"));
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

    public void MeshReader()
    {
        //Finds the right section 0xA entry for use with the entryLength and entryType elements.
        int section0BlockACount = 0;

        //Position
        for (int i = 0; i < objects.Length; i++)
        {
            unityMesh[i].vertices = new Vector3[section0Block3Entries[i].numVertices];

            for (int j = 0; j < section0Block3Entries[i].numVertices; j++)
            {
                unityMesh[i].vertices[j] = new Vector3(objects[i].vertices[j].x, objects[i].vertices[j].y, objects[i].vertices[j].z);
            } //for
        } //for

        //Normals, Bone Weights, Bone Group Ids and UVs
        for (int i = 0; i < objects.Length; i++)
        {
            while (section0BlockAEntries[section0BlockACount].type != 1)
            {
                section0BlockACount++;
            } //while

            unityMesh[i].normals = new Vector3[section0Block3Entries[i].numVertices];

            unityMesh[i].UVs = new Vector2[section0Block3Entries[i].numVertices];

            for (int j = 0; j < section0Block3Entries[i].numVertices; j++)
            {

                unityMesh[i].normals[j] = new Vector3(objects[i].additionalVertexData[j].normalX, objects[i].additionalVertexData[j].normalY, objects[i].additionalVertexData[j].normalZ);

                if (section0BlockAEntries[section0BlockACount].length == 0x20 || section0BlockAEntries[section0BlockACount].length == 0x1C || section0BlockAEntries[section0BlockACount].length == 0x2C || section0BlockAEntries[section0BlockACount].length == 0x28 || section0BlockAEntries[section0BlockACount].length == 0x24)
                {
                    //unityBoneWeights[i].weight0 = vbuffer[j].boneWeightX;
                    //unityBoneWeights[i].weight1 = vbuffer[j].boneWeightY;
                    //unityBoneWeights[i].weight2 = vbuffer[j].boneWeightZ;
                    //unityBoneWeights[i].weight3 = vbuffer[j].boneWeightW;
                    //unityBoneWeights[i].boneIndex0 = vbuffer[j].boneID1;
                    //unityBoneWeights[i].boneIndex1 = vbuffer[j].boneID2;
                    //unityBoneWeights[i].boneIndex2 = vbuffer[j].boneID3;
                    //unityBoneWeights[i].boneIndex3 = vbuffer[j].boneID4;

                    unityMesh[i].UVs[j] = new Vector2(objects[i].additionalVertexData[j].textureU, objects[i].additionalVertexData[j].textureV);
                } //if
            } //for
        } //for

        //Faces
        for (int i = 0; i < objects.Length; i++)
        {
            unityMesh[i].faces = new int[section0Block3Entries[i].numFaceVertices];

            for (int j = 0, h = 0; j < (section0Block3Entries[i].numFaceVertices / 3); j++, h += 3)
            {
                //UnityEngine.Debug.Log("Index: " + j + ", array length: " + unityFaces.Length);
                unityMesh[i].faces[h] = (int)objects[i].faces[j].vertex1Id;
                //UnityEngine.Debug.Log("Index: " + j + ", array length: " + unityFaces.Length);
                unityMesh[i].faces[h + 1] = (int)objects[i].faces[j].vertex2Id;
                //UnityEngine.Debug.Log("Index: " + j + ", array length: " + unityFaces.Length);
                unityMesh[i].faces[h + 2] = (int)objects[i].faces[j].vertex3Id;
            } //for
        } //for

        //Instances a new GameObject and also a new MeshFilter and MeshRenderer 
        GameObject fmdlGameObject = new GameObject();
        MeshFilter meshFilter = fmdlGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = fmdlGameObject.AddComponent<MeshRenderer>(); //This will be replaced with SkinnedMeshRenderer later

        var meshes = new List<Mesh>();
        for (int i = 0; i < objects.Length; i++)
        {
            Mesh fmdl = new Mesh();
            fmdl.vertices = unityMesh[i].vertices;
            fmdl.triangles = unityMesh[i].faces;
            fmdl.normals = unityMesh[i].normals;
            //mesh.boneWeights = unityBoneWeights;
            fmdl.uv = unityMesh[i].UVs;

            meshes.Add(fmdl);
            meshFilter.mesh = fmdl;
        }

        fmdlGameObject.AddComponent<MeshCollider>();
    } //MeshReader
} //class