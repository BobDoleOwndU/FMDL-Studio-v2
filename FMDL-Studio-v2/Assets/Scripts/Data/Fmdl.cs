using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Half;
using System.Collections.Generic;
using UnityEngine;

public class Fmdl
{
    private enum Section0BlockType
    {
        Bones = 0,
        MeshGroups = 1,
        MeshGroupAssignment = 2,
        MeshInfo = 3,
        MaterialInstances = 4,
        BoneGroups = 5,
        Textures = 6,
        TextureTypes = 7,
        Materials = 8,
        MeshFormatAssignment = 9,
        MeshFormats = 10,
        VertexFormats = 11,
        Strings = 12,
        BoundingBoxes = 13,
        BufferOffsets = 14,
        LodInfo = 16,
        FaceIndices = 17,
        Type12 = 18,
        Type14 = 20,
        TexturePaths = 21,
        StringHashes = 22
    }; //Section0BlockType

    private enum Section1BlockType
    {
        MaterialParameters = 0,
        MeshData = 2,
        Strings = 3
    }; //Section1BlockType

    public struct Section0Info
    {
        public ushort id;
        public ushort numEntries;
        public uint offset;
    } //struct

    public struct Section1Info
    {
        public uint id;
        public uint offset;
        public uint length;
    } //struct

    public struct Section0Block0Entry
    {
        public ushort stringId;
        public ushort parentId;
        public ushort boundingBoxId;
        public ushort unknown0; //always 0x1?
        public float localPositionX;
        public float localPositionY;
        public float localPositionZ;
        public float locaPositionlW;
        public float worldPositionX;
        public float worldPositionY;
        public float worldPositionZ;
        public float worldPositionW;
    } //struct

    public struct Section0Block1Entry
    {
        public ushort stringId;
        public ushort invisibilityFlag;
        public ushort parentId;
        public ushort unknown0; //always 0xFF
    } //struct

    public struct Section0Block2Entry
    {
        public ushort meshGroupId;
        public ushort numObjects;
        public ushort firstObjectId;
        public ushort id;
        public ushort unknown0;
    } //struct

    public struct Section0Block3Entry
    {
        public uint unknown0;
        public ushort materialInstanceId;
        public ushort boneGroupId;
        public ushort id;
        public ushort numVertices;
        public uint firstFaceVertexId;
        public uint numFaceVertices;
        public ulong firstMeshFormatId;
    } //struct

    public struct Section0Block4Entry
    {
        public ushort stringId;
        public ushort unknown0; //might just be padding.
        public ushort materialId;
        public byte numTextures;
        public byte numParameters;
        public ushort firstTextureId; //starts at this entry in 0x7 and grabs all textures between (inclusive) it and the lastTexture.
        public ushort firstParameterId;
    } //struct

    public struct Section0Block5Entry
    {
        public ushort unknown0;
        public ushort numEntries;
        public ushort[] entries;
    } //struct

    public struct Section0Block6Entry
    {
        public ushort stringId;
        public ushort pathId;
    } //struct

    public struct Section0Block7Entry
    {
        public ushort stringId;
        public ushort referenceId;
    } //struct

    public struct Section0Block8Entry
    {
        public ushort stringId;
        public ushort typeId;
    } //struct

    public struct Section0Block9Entry
    {
        public byte numMeshFormatEntries;
        public byte numVertexFormatEntries;
        public ushort unknown0;
        public ushort firstMeshFormatId;
        public ushort firstVertexFormatId;
    } //Section0Block9Entry

    public struct Section0BlockAEntry
    {
        public byte bufferOffsetId;
        public byte numVertexFormatEntries;
        public byte length;
        public byte type;
        public uint offset;
    } //struct

    public struct Section0BlockBEntry
    {
        public byte usage;
        public byte format;
        public ushort offset;

        // format
        // 1 - float
        // 4 - uint16
        // 6 - half float
        // 7 - half float
        // 8 - uint8 (normalized)
        // 9 - uint8

        // usage
        // 0 - position
        // 1 - joint weights 0
        // 2 - normal
        // 3 - diffuse
        // 7 - joint indices 0
        // 8 - tex coord 0...
        // 9 - tex coord 2
        // B - ...tex coord 3
        // C - joint weights 1
        // D - joint indices 1
        // E - tangent
    } //struct

    public struct Section0BlockCEntry
    {
        public ushort section1BlockId;
        public ushort length;
        public uint offset;
    } //struct

    public struct Section0BlockDEntry
    {
        public float maxX;
        public float maxY;
        public float maxZ;
        public float maxW;
        public float minX;
        public float minY;
        public float minZ;
        public float minW;
    } //struct

    public struct Section0BlockEEntry
    {
        public uint unknown0; //Flag of some sort?
        public uint length;
        public uint offset;
    } //struct

    public struct Section0Block10Entry
    {
        //variables here are assumptions. may not be correct.
        public uint unknown0;
        public float highDetailDistance;
        public float midDetailDistance;
        public float lowDetailDistance;
    } //struct

    public struct Section0Block11Entry
    {
        public uint firstFaceVertexId;
        public uint numFaceVertices;
    } //struct

    public struct Section0Block12Entry
    {
        public ulong unknown0; //Always 0.
    } //struct

    public struct Section0Block14Entry
    {
        public float unknown0; //Nulling triggers lowest LOD faces.
        public float unknown1;
        public float unknown2;
        public float unknown3; //Always a whole number?
        public uint unknown4;
        public uint unknown5;
    } //struct

    public struct MaterialParameter
    {
        public float[] values;
    } //struct

    public struct Vertex
    {
        public float x;
        public float y;
        public float z;
    } //struct

    public struct AdditionalVertexData
    {
        public Half normalX;
        public Half normalY;
        public Half normalZ;
        public Half normalW;

        public Half tangentX;
        public Half tangentY;
        public Half tangentZ;
        public Half tangentW;

        public byte colourR;
        public byte colourG;
        public byte colourB;
        public byte colourA;

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

        //unconfirmed
        public Half unknownU0;
        public Half unknownV0;

        //unconfirmed
        public Half unknownU1;
        public Half unknownV1;

        //unconfirmed
        public Half unknownU2;
        public Half unknownV2;

        //unconfirmed
        public byte unknownWeight0;
        public byte unknownWeight1;
        public byte unknownWeight2;
        public byte unknownWeight3;

        //unconfirmed
        public ushort unknownId0;
        public ushort unknownId1;
        public ushort unknownId2;
        public ushort unknownId3;
    } //struct

    public struct Face
    {
        public ushort vertex1Id;
        public ushort vertex2Id;
        public ushort vertex3Id;
    } //struct

    public struct Object
    {
        public Vertex[] vertices;
        public AdditionalVertexData[] additionalVertexData;
        public Face[] faces;
        public Face[][] lodFaces;
    } //struct

    //Importer Classes/Structs
    private class MeshGroup
    {
        public string name;
        public bool invisible;
    } //MeshGroup

    private class MeshGroupEntry
    {
        public int meshGroupIndex;
        public int numMeshes;
    } //MeshGroupEntry

    private struct FoxMaterial
    {
        public string name;
        public string type;
    } //Material

    private class MeshFormat
    {
        public byte meshFormat0Size = 1;
        public byte meshFormat1Size = 0;
        public byte meshFormat2Size = 0;
        public byte meshFormat3Size = 0;
        public uint zeroOffset;
        public uint additionalOffset;
        public byte size = 0;
        public bool normals;
        public bool tangents;
        public bool colour;
        public bool weights0;
        public bool indices0;
        public bool uv0;
        public bool uv1;
        public bool uv2;
        public bool uv3;
        public bool weights1;
        public bool indices1;
    } //MeshFormat

    //Instance Variables
    public string name { get; private set; }

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

    public int bonesIndex { get; private set; } = -1;
    public int meshGroupsIndex { get; private set; } = -1;
    public int meshGroupAssignmentIndex { get; private set; } = -1;
    public int meshInfoIndex { get; private set; } = -1;
    public int materialInstancesIndex { get; private set; } = -1;
    public int boneGroupsIndex { get; private set; } = -1;
    public int texturesIndex { get; private set; } = -1;
    public int textureTypesIndex { get; private set; } = -1;
    public int materialsIndex { get; private set; } = -1;
    public int meshFormatAssignmentIndex { get; private set; } = -1;
    public int meshFormatsIndex { get; private set; } = -1;
    public int vertexFormatsPosition { get; private set; } = -1;
    public int stringsIndex { get; private set; } = -1;
    public int boundingBoxesIndex { get; private set; } = -1;
    public int bufferOffsetsIndex { get; private set; } = -1;
    public int lodInfoIndex { get; private set; } = -1;
    public int faceIndicesIndex { get; private set; } = -1;
    public int type12Index { get; private set; } = -1;
    public int type14Index { get; private set; } = -1;
    public int texturePathsIndex { get; private set; } = -1;
    public int stringHashesIndex { get; private set; } = -1;

    private int section1MaterialParametersIndex = -1;
    private int section1MeshDataIndex = -1;
    private int section1StringsIndex = -1;

    public MaterialParameter[] materialParameters { get; private set; }
    public Object[] objects { get; private set; }
    public List<string> strings { get; private set; } = new List<string>(0);

    private List<Section0Info> section0Info = new List<Section0Info>(0);
    private List<Section1Info> section1Info = new List<Section1Info>(0);

    public List<Section0Block0Entry> section0Block0Entries { get; private set; } = new List<Section0Block0Entry>(0);
    public List<Section0Block1Entry> section0Block1Entries { get; private set; } = new List<Section0Block1Entry>(0);
    public List<Section0Block2Entry> section0Block2Entries { get; private set; } = new List<Section0Block2Entry>(0);
    public List<Section0Block3Entry> section0Block3Entries { get; private set; } = new List<Section0Block3Entry>(0);
    public List<Section0Block4Entry> section0Block4Entries { get; private set; } = new List<Section0Block4Entry>(0);
    public List<Section0Block5Entry> section0Block5Entries { get; private set; } = new List<Section0Block5Entry>(0);
    public List<Section0Block6Entry> section0Block6Entries { get; private set; } = new List<Section0Block6Entry>(0);
    public List<Section0Block7Entry> section0Block7Entries { get; private set; } = new List<Section0Block7Entry>(0);
    public List<Section0Block8Entry> section0Block8Entries { get; private set; } = new List<Section0Block8Entry>(0);
    public List<Section0Block9Entry> section0Block9Entries { get; private set; } = new List<Section0Block9Entry>(0);
    public List<Section0BlockAEntry> section0BlockAEntries { get; private set; } = new List<Section0BlockAEntry>(0);
    public List<Section0BlockBEntry> section0BlockBEntries { get; private set; } = new List<Section0BlockBEntry>(0);
    public List<Section0BlockCEntry> section0BlockCEntries { get; private set; } = new List<Section0BlockCEntry>(0);
    public List<Section0BlockDEntry> section0BlockDEntries { get; private set; } = new List<Section0BlockDEntry>(0);
    public List<Section0BlockEEntry> section0BlockEEntries { get; private set; } = new List<Section0BlockEEntry>(0);
    public List<Section0Block10Entry> section0Block10Entries { get; private set; } = new List<Section0Block10Entry>(0);
    public List<Section0Block11Entry> section0Block11Entries { get; private set; } = new List<Section0Block11Entry>(0);
    public List<Section0Block12Entry> section0Block12Entries { get; private set; } = new List<Section0Block12Entry>(0);
    public List<Section0Block14Entry> section0Block14Entries { get; private set; } = new List<Section0Block14Entry>(0);
    public List<ulong> section0Block15Entries { get; private set; } = new List<ulong>(0);
    public List<ulong> section0Block16Entries { get; private set; } = new List<ulong>(0);

    /*
     * Constructor
     * Initializes the Fmdl with the name passed to it.
     */
    public Fmdl(string name)
    {
        this.name = name;
    } //constructor

    /*
     * Read
     * Reads the data from the stream linked to the .fmdl file.
     */
    public void Read(FileStream stream)
    {
        if (File.Exists("fmdl_dictionary.txt"))
            Hashing.ReadStringDictionary("fmdl_dictionary.txt");

        if (File.Exists("qar_dictionary.txt"))
            Hashing.ReadPathDictionary("qar_dictionary.txt");

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

        //get the section0 info.
        for (int i = 0; i < numSection0Blocks; i++)
        {
            Section0Info s = new Section0Info();

            s.id = reader.ReadUInt16();
            s.numEntries = reader.ReadUInt16();
            s.offset = reader.ReadUInt32();

            switch (s.id)
            {
                case (ushort)Section0BlockType.Bones:
                    bonesIndex = i;
                    break;
                case (ushort)Section0BlockType.MeshGroups:
                    meshGroupsIndex = i;
                    break;
                case (ushort)Section0BlockType.MeshGroupAssignment:
                    meshGroupAssignmentIndex = i;
                    break;
                case (ushort)Section0BlockType.MeshInfo:
                    meshInfoIndex = i;
                    break;
                case (ushort)Section0BlockType.MaterialInstances:
                    materialInstancesIndex = i;
                    break;
                case (ushort)Section0BlockType.BoneGroups:
                    boneGroupsIndex = i;
                    break;
                case (ushort)Section0BlockType.Textures:
                    texturesIndex = i;
                    break;
                case (ushort)Section0BlockType.TextureTypes:
                    textureTypesIndex = i;
                    break;
                case (ushort)Section0BlockType.Materials:
                    materialsIndex = i;
                    break;
                case (ushort)Section0BlockType.MeshFormatAssignment:
                    meshFormatAssignmentIndex = i;
                    break;
                case (ushort)Section0BlockType.MeshFormats:
                    meshFormatsIndex = i;
                    break;
                case (ushort)Section0BlockType.VertexFormats:
                    vertexFormatsPosition = i;
                    break;
                case (ushort)Section0BlockType.Strings:
                    stringsIndex = i;
                    break;
                case (ushort)Section0BlockType.BoundingBoxes:
                    boundingBoxesIndex = i;
                    break;
                case (ushort)Section0BlockType.BufferOffsets:
                    bufferOffsetsIndex = i;
                    break;
                case (ushort)Section0BlockType.LodInfo:
                    lodInfoIndex = i;
                    break;
                case (ushort)Section0BlockType.FaceIndices:
                    faceIndicesIndex = i;
                    break;
                case (ushort)Section0BlockType.Type12:
                    type12Index = i;
                    break;
                case (ushort)Section0BlockType.Type14:
                    type14Index = i;
                    break;
                case (ushort)Section0BlockType.TexturePaths:
                    texturePathsIndex = i;
                    break;
                case (ushort)Section0BlockType.StringHashes:
                    stringHashesIndex = i;
                    break;
                default:
                    break;
            } //switch

            section0Info.Add(s);
        } //for

        //get the section1 info.
        for (int i = 0; i < numSection1Blocks; i++)
        {
            Section1Info s = new Section1Info();

            s.id = reader.ReadUInt32();
            s.offset = reader.ReadUInt32();
            s.length = reader.ReadUInt32();

            switch (s.id)
            {
                case (uint)Section1BlockType.MaterialParameters:
                    section1MaterialParametersIndex = i;
                    materialParameters = new MaterialParameter[(s.length / 4) / 4];
                    break;
                case (uint)Section1BlockType.MeshData:
                    section1MeshDataIndex = i;
                    break;
                case (uint)Section1BlockType.Strings:
                    section1StringsIndex = i;
                    break;
                default:
                    break;
            } //switch

            section1Info.Add(s);
        } //for

        objects = new Object[section0Info[meshInfoIndex].numEntries];

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x0 - BONE DEFINITIONS
         *
         ****************************************************************/
        if (bonesIndex != -1)
        {
            //go to and get the section 0x0 entry info.
            reader.BaseStream.Position = section0Info[bonesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[bonesIndex].numEntries; i++)
            {
                Section0Block0Entry s = new Section0Block0Entry();

                s.stringId = reader.ReadUInt16();
                s.parentId = reader.ReadUInt16();
                s.boundingBoxId = reader.ReadUInt16();
                s.unknown0 = reader.ReadUInt16();
                reader.BaseStream.Position += 0x8;
                s.localPositionX = reader.ReadSingle();
                s.localPositionY = reader.ReadSingle();
                s.localPositionZ = reader.ReadSingle();
                s.locaPositionlW = reader.ReadSingle();
                s.worldPositionX = reader.ReadSingle();
                s.worldPositionY = reader.ReadSingle();
                s.worldPositionZ = reader.ReadSingle();
                s.worldPositionW = reader.ReadSingle();

                section0Block0Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x1 - MESH GROUP DEFINITIONS
         *
         ****************************************************************/
        if (meshGroupsIndex != -1)
        {
            //go to and get the section 0x1 entry info.
            reader.BaseStream.Position = section0Info[meshGroupsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[meshGroupsIndex].numEntries; i++)
            {
                Section0Block1Entry s = new Section0Block1Entry();

                s.stringId = reader.ReadUInt16();
                s.invisibilityFlag = reader.ReadUInt16();
                s.parentId = reader.ReadUInt16();
                s.unknown0 = reader.ReadUInt16();

                section0Block1Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x2 - MESH GROUP ASSIGNMENTS
         *
         ****************************************************************/
        if (meshGroupAssignmentIndex != -1)
        {
            //go to and get the section 0x2 entry info.
            reader.BaseStream.Position = section0Info[meshGroupAssignmentIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[meshGroupAssignmentIndex].numEntries; i++)
            {
                Section0Block2Entry s = new Section0Block2Entry();

                reader.BaseStream.Position += 0x4;
                s.meshGroupId = reader.ReadUInt16();
                s.numObjects = reader.ReadUInt16();
                s.firstObjectId = reader.ReadUInt16();
                s.id = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                s.unknown0 = reader.ReadUInt16();
                reader.BaseStream.Position += 0xE;

                section0Block2Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x3 - MESH INFO
         *
         ****************************************************************/
        if (meshInfoIndex != -1)
        {
            //go to and get the section 0x3 entry info.
            reader.BaseStream.Position = section0Info[meshInfoIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[meshInfoIndex].numEntries; i++)
            {
                Section0Block3Entry s = new Section0Block3Entry();

                s.unknown0 = reader.ReadUInt32();
                s.materialInstanceId = reader.ReadUInt16();
                s.boneGroupId = reader.ReadUInt16();
                s.id = reader.ReadUInt16();
                s.numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                s.firstFaceVertexId = reader.ReadUInt32();
                s.numFaceVertices = reader.ReadUInt32();
                s.firstMeshFormatId = reader.ReadUInt64();
                reader.BaseStream.Position += 0x10;

                section0Block3Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x4 - MATERIAL INSTANCE DEFINITIONS
         *
         ****************************************************************/
        if (materialInstancesIndex != -1)
        {
            //go to and get the section 0x3 entry info.
            reader.BaseStream.Position = section0Info[materialInstancesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[materialInstancesIndex].numEntries; i++)
            {
                Section0Block4Entry s = new Section0Block4Entry();

                s.stringId = reader.ReadUInt16();
                s.unknown0 = reader.ReadUInt16();
                s.materialId = reader.ReadUInt16();
                s.numTextures = reader.ReadByte();
                s.numParameters = reader.ReadByte();
                s.firstTextureId = reader.ReadUInt16();
                s.firstParameterId = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;

                section0Block4Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x5 - BONE GROUP DEFINITIONS
         *
         ****************************************************************/
        if (boneGroupsIndex != -1)
        {
            //go to and get the section 0x5 entry info.
            reader.BaseStream.Position = section0Info[boneGroupsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[boneGroupsIndex].numEntries; i++)
            {
                Section0Block5Entry s = new Section0Block5Entry();

                s.unknown0 = reader.ReadUInt16();
                s.numEntries = reader.ReadUInt16();
                s.entries = new ushort[s.numEntries];

                for (int j = 0; j < s.entries.Length; j++)
                    s.entries[j] = reader.ReadUInt16();

                reader.BaseStream.Position += 0x40 - s.numEntries * 2;

                section0Block5Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x6 - TEXTURE DEFINITIONS
         *
         ****************************************************************/
        if (texturesIndex != -1)
        {
            //go to and get the section 0x6 entry info.
            reader.BaseStream.Position = section0Info[texturesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[texturesIndex].numEntries; i++)
            {
                Section0Block6Entry s = new Section0Block6Entry();

                s.stringId = reader.ReadUInt16();
                s.pathId = reader.ReadUInt16();

                section0Block6Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x7 - TEXTURE TYPE/MATERIAL PARAMETER ASSIGNMENT
         *
         ****************************************************************/
        if (textureTypesIndex != -1)
        {
            //go to and get the section 0x7 entry info.
            reader.BaseStream.Position = section0Info[textureTypesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[textureTypesIndex].numEntries; i++)
            {
                Section0Block7Entry s = new Section0Block7Entry();

                s.stringId = reader.ReadUInt16();
                s.referenceId = reader.ReadUInt16();

                section0Block7Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x8 - MATERIAL/MATERIAL TYPE ASSIGNMENT
         *
         ****************************************************************/
        if (materialsIndex != -1)
        {
            //go to and get the section 0x8 entry info.
            reader.BaseStream.Position = section0Info[materialsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[materialsIndex].numEntries; i++)
            {
                Section0Block8Entry s = new Section0Block8Entry();

                s.stringId = reader.ReadUInt16();
                s.typeId = reader.ReadUInt16();

                section0Block8Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x9 - MESH FORMAT ASSIGNMENT
         *
         ****************************************************************/
        if (meshFormatAssignmentIndex != -1)
        {
            //go to and get the section 0x8 entry info.
            reader.BaseStream.Position = section0Info[meshFormatAssignmentIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[meshFormatAssignmentIndex].numEntries; i++)
            {
                Section0Block9Entry s = new Section0Block9Entry();

                s.numMeshFormatEntries = reader.ReadByte();
                s.numVertexFormatEntries = reader.ReadByte();
                s.unknown0 = reader.ReadUInt16();
                s.firstMeshFormatId = reader.ReadUInt16();
                s.firstVertexFormatId = reader.ReadUInt16();

                section0Block9Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xA - MESH FORMAT DEFINITIONS
         *
         ****************************************************************/
        if (meshFormatsIndex != -1)
        {
            //go to and get the section 0xA entry info.
            reader.BaseStream.Position = section0Info[meshFormatsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[meshFormatsIndex].numEntries; i++)
            {
                Section0BlockAEntry s = new Section0BlockAEntry();

                s.bufferOffsetId = reader.ReadByte();
                s.numVertexFormatEntries = reader.ReadByte();
                s.length = reader.ReadByte();
                s.type = reader.ReadByte();
                s.offset = reader.ReadUInt32();

                section0BlockAEntries.Add(s);
            } //for
        } //if ends

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xB - VERTEX FORMAT DEFINITIONS
         *
         ****************************************************************/
        if (vertexFormatsPosition != -1)
        {
            //go to and get the section 0xB entry info.
            reader.BaseStream.Position = section0Info[vertexFormatsPosition].offset + section0Offset;

            for (int i = 0; i < section0Info[vertexFormatsPosition].numEntries; i++)
            {
                Section0BlockBEntry s = new Section0BlockBEntry();

                s.usage = reader.ReadByte();
                s.format = reader.ReadByte();
                s.offset = reader.ReadUInt16();

                section0BlockBEntries.Add(s);
            } //for
        } //if ends

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xC - STRING DEFINITIONS
         *
         ****************************************************************/
        if (stringsIndex != -1)
        {
            //go to and get the section 0xC entry info.
            reader.BaseStream.Position = section0Info[stringsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[stringsIndex].numEntries; i++)
            {
                Section0BlockCEntry s = new Section0BlockCEntry();

                s.section1BlockId = reader.ReadUInt16();
                s.length = reader.ReadUInt16();
                s.offset = reader.ReadUInt32();

                section0BlockCEntries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xD - BOUNDING BOX/SHADOW(?) DEFINITIONS
         *
         ****************************************************************/
        if (boundingBoxesIndex != -1)
        {
            //go to and get the section 0xD entry info.
            reader.BaseStream.Position = section0Info[boundingBoxesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[boundingBoxesIndex].numEntries; i++)
            {
                Section0BlockDEntry s = new Section0BlockDEntry();

                s.maxX = reader.ReadSingle();
                s.maxY = reader.ReadSingle();
                s.maxZ = reader.ReadSingle();
                s.maxW = reader.ReadSingle();
                s.minX = reader.ReadSingle();
                s.minY = reader.ReadSingle();
                s.minZ = reader.ReadSingle();
                s.minW = reader.ReadSingle();

                section0BlockDEntries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0xE - BUFFER OFFSET DEFINITIONS
         *
         ****************************************************************/
        if (bufferOffsetsIndex != -1)
        {
            //go to and get the section 0xE entry info.
            reader.BaseStream.Position = section0Info[bufferOffsetsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[bufferOffsetsIndex].numEntries; i++)
            {
                Section0BlockEEntry s = new Section0BlockEEntry();

                s.unknown0 = reader.ReadUInt32();
                s.length = reader.ReadUInt32();
                s.offset = reader.ReadUInt32();
                reader.BaseStream.Position += 0x4;

                section0BlockEEntries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x10 - LOD INFO
         *
         ****************************************************************/
        if (lodInfoIndex != -1)
        {
            //go to and get the section 0x10 entry info.
            reader.BaseStream.Position = section0Info[lodInfoIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[lodInfoIndex].numEntries; i++)
            {
                Section0Block10Entry s = new Section0Block10Entry();

                s.unknown0 = reader.ReadUInt32();
                s.highDetailDistance = reader.ReadSingle();
                s.midDetailDistance = reader.ReadSingle();
                s.lowDetailDistance = reader.ReadSingle();

                section0Block10Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x11 - LOD INFO
         *
         ****************************************************************/
        if (faceIndicesIndex != -1)
        {
            //go to and get the section 0x11 entry info.
            reader.BaseStream.Position = section0Info[faceIndicesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[faceIndicesIndex].numEntries; i++)
            {
                Section0Block11Entry s = new Section0Block11Entry();

                s.firstFaceVertexId = reader.ReadUInt32();
                s.numFaceVertices = reader.ReadUInt32();

                section0Block11Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x12 - UNKNOWN
         *
         ****************************************************************/
        if (type12Index != -1)
        {
            //go to and get the section 0x12 entry info.
            reader.BaseStream.Position = section0Info[type12Index].offset + section0Offset;

            for (int i = 0; i < section0Info[faceIndicesIndex].numEntries; i++)
            {
                Section0Block12Entry s = new Section0Block12Entry();

                s.unknown0 = reader.ReadUInt64();

                section0Block12Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x14 - UNKNOWN
         *
         ****************************************************************/
        if (type14Index != -1)
        {
            //go to and get the section 0x14 entry info.
            reader.BaseStream.Position = section0Info[type14Index].offset + section0Offset;

            for (int i = 0; i < section0Info[faceIndicesIndex].numEntries; i++)
            {
                Section0Block14Entry s = new Section0Block14Entry();

                reader.BaseStream.Position += 0x4;
                s.unknown0 = reader.ReadSingle();
                s.unknown1 = reader.ReadSingle();
                s.unknown2 = reader.ReadSingle();
                s.unknown3 = reader.ReadSingle();
                reader.BaseStream.Position += 0x8;
                s.unknown4 = reader.ReadUInt32();
                s.unknown5 = reader.ReadUInt32();
                reader.BaseStream.Position += 0x5C;

                section0Block14Entries.Add(s);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK Ox15 - TEXTURE PATH DEFINITIONS
         *
         ****************************************************************/
        if (texturePathsIndex != -1)
        {
            //go to and get the section 0x15 entry info.
            reader.BaseStream.Position = section0Info[texturePathsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[texturePathsIndex].numEntries; i++)
            {
                ulong u = reader.ReadUInt64();

                section0Block15Entries.Add(u);
            } //for
        } //if

        /****************************************************************
         *
         * SECTION 0 BLOCK 0x16 - STRING HASH DEFINITIONS
         *
         ****************************************************************/
        if (stringHashesIndex != -1)
        {
            //go to and get the section 0x16 entry info.
            reader.BaseStream.Position = section0Info[stringHashesIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[stringHashesIndex].numEntries; i++)
            {
                ulong u = reader.ReadUInt64();

                section0Block16Entries.Add(u);
            } //for
        } //if

        /****************************************************************
         *
         * NUMERICAL PARAMETERS
         *
         ****************************************************************/
        if (section1MaterialParametersIndex != -1)
        {
            reader.BaseStream.Position = section1Info[section1MaterialParametersIndex].offset + section1Offset;

            for (int i = 0; i < materialParameters.Length; i++)
            {
                materialParameters[i].values = new float[4];

                for (int j = 0; j < materialParameters[i].values.Length; j++)
                    materialParameters[i].values[j] = reader.ReadSingle();
            } //for
        } //if

        /****************************************************************
         *
         * POSITION
         *
         ****************************************************************/
        reader.BaseStream.Position = section1Info[section1MeshDataIndex].offset + section1Offset;

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
        reader.BaseStream.Position = section0BlockEEntries[1].offset + section1Offset + section1Info[section1MeshDataIndex].offset;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].additionalVertexData = new AdditionalVertexData[section0Block3Entries[i].numVertices];
            //reader.BaseStream.Position = section0BlockEEntries[1].offset + section1Offset + section1Info[section1MeshDataIndex].offset + section0BlockAEntries[section0Block9Entries[i].firstMeshFormatId + 1].offset;

            for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
            {
                long position = reader.BaseStream.Position;

                for (int h = section0Block9Entries[i].firstVertexFormatId; h < section0Block9Entries[i].firstVertexFormatId + section0Block9Entries[i].numVertexFormatEntries; h++)
                {
                    reader.BaseStream.Position = position + section0BlockBEntries[h].offset;

                    switch (section0BlockBEntries[h].usage)
                    {
                        case 0: //vertex positions.
                            break;
                        case 1: //bone weights.
                            objects[i].additionalVertexData[j].boneWeightX = reader.ReadByte() / 255.0f;
                            objects[i].additionalVertexData[j].boneWeightY = reader.ReadByte() / 255.0f;
                            objects[i].additionalVertexData[j].boneWeightZ = reader.ReadByte() / 255.0f;
                            objects[i].additionalVertexData[j].boneWeightW = reader.ReadByte() / 255.0f;
                            break;
                        case 2: //normals.
                            objects[i].additionalVertexData[j].normalX = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].normalY = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].normalZ = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].normalW = ToHalf(reader.ReadUInt16());
                            break;
                        case 3: //diffuse.
                            objects[i].additionalVertexData[j].colourR = reader.ReadByte();
                            objects[i].additionalVertexData[j].colourG = reader.ReadByte();
                            objects[i].additionalVertexData[j].colourB = reader.ReadByte();
                            objects[i].additionalVertexData[j].colourA = reader.ReadByte();
                            break;
                        case 7: //bone indices.
                            objects[i].additionalVertexData[j].boneGroup0Id = reader.ReadByte();
                            objects[i].additionalVertexData[j].boneGroup1Id = reader.ReadByte();
                            objects[i].additionalVertexData[j].boneGroup2Id = reader.ReadByte();
                            objects[i].additionalVertexData[j].boneGroup3Id = reader.ReadByte();
                            break;
                        case 8: //UV.
                            objects[i].additionalVertexData[j].textureU = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].textureV = ToHalf(reader.ReadUInt16());
                            break;
                        case 9: //UV 2?
                            objects[i].additionalVertexData[j].unknownU0 = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].unknownV0 = ToHalf(reader.ReadUInt16());
                            break;
                        case 10: //UV 3?
                            objects[i].additionalVertexData[j].unknownU1 = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].unknownV1 = ToHalf(reader.ReadUInt16());
                            break;
                        case 11: //UV 4?
                            objects[i].additionalVertexData[j].unknownU2 = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].unknownV2 = ToHalf(reader.ReadUInt16());
                            break;
                        case 12: //bone weights 2?
                            objects[i].additionalVertexData[j].unknownWeight0 = reader.ReadByte();
                            objects[i].additionalVertexData[j].unknownWeight1 = reader.ReadByte();
                            objects[i].additionalVertexData[j].unknownWeight2 = reader.ReadByte();
                            objects[i].additionalVertexData[j].unknownWeight3 = reader.ReadByte();
                            break;
                        case 13: //bone indices 2?
                            objects[i].additionalVertexData[j].unknownId0 = reader.ReadUInt16();
                            objects[i].additionalVertexData[j].unknownId1 = reader.ReadUInt16();
                            objects[i].additionalVertexData[j].unknownId2 = reader.ReadUInt16();
                            objects[i].additionalVertexData[j].unknownId3 = reader.ReadUInt16();
                            break;
                        case 14: //tangent.
                            objects[i].additionalVertexData[j].tangentX = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].tangentY = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].tangentZ = ToHalf(reader.ReadUInt16());
                            objects[i].additionalVertexData[j].tangentW = ToHalf(reader.ReadUInt16());
                            break;
                        default:
                            UnityEngine.Debug.Log("Usage: " + section0BlockBEntries[h].usage);
                            UnityEngine.Debug.Log("h: " + h);
                            break;
                    } //switch
                } //for

                reader.BaseStream.Position = position + section0BlockAEntries[section0Block9Entries[i].firstMeshFormatId + 1].length;
            } //for

            //align the stream.
            if (reader.BaseStream.Position % 0x10 != 0)
                reader.BaseStream.Position += (0x10 - reader.BaseStream.Position % 0x10);
        } //for

        /****************************************************************
         *
         * FACES
         *
         ****************************************************************/
        for (int i = 0; i < objects.Length; i++)
        {
            reader.BaseStream.Position = section0BlockEEntries[2].offset + section1Offset + section1Info[section1MeshDataIndex].offset + section0Block3Entries[i].firstFaceVertexId * 2;

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
         * LOD FACES
         *
         ****************************************************************/
        {
            int section0Block11Counter = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].lodFaces = new Face[section0Block10Entries[0].unknown0][];

                for (int j = 0; j < objects[i].lodFaces.Length; j++)
                {
                    reader.BaseStream.Position = section0BlockEEntries[2].offset + section1Offset + section1Info[section1MeshDataIndex].offset + section0Block3Entries[i].firstFaceVertexId * 2 + section0Block11Entries[section0Block11Counter].firstFaceVertexId * 2;

                    objects[i].lodFaces[j] = new Face[section0Block11Entries[section0Block11Counter].numFaceVertices / 3];

                    for (int h = 0; h < objects[i].lodFaces[j].Length; h++)
                    {
                        objects[i].lodFaces[j][h].vertex1Id = reader.ReadUInt16();
                        objects[i].lodFaces[j][h].vertex2Id = reader.ReadUInt16();
                        objects[i].lodFaces[j][h].vertex3Id = reader.ReadUInt16();
                    } //for

                    section0Block11Counter++;
                } //for
            } //for
        } //code block


        /****************************************************************
         *
         * STRINGS
         *
         ****************************************************************/
        if (stringsIndex != -1)
        {
            for (int i = 0; i < section0BlockCEntries.Count; i++)
            {
                reader.BaseStream.Position = section1Offset + section1Info[section1StringsIndex].offset + section0BlockCEntries[i].offset;
                string s = Encoding.Default.GetString(reader.ReadBytes(section0BlockCEntries[i].length));

                strings.Add(s);
            } //for
        } //if
    } //Read

    /*
     * Write
     * Writes data from a Unity model to an fmdl file.
     */
    public void Write(GameObject gameObject, FileStream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream, Encoding.Default, true);
        List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
        List<Material> materialInstances = new List<Material>(0);
        List<Texture> textures = new List<Texture>(0);
        List<Transform> bones = new List<Transform>(0);
        List<MeshGroup> meshGroups = new List<MeshGroup>(0);
        List<MeshGroupEntry> meshGroupEntries = new List<MeshGroupEntry>(0);
        List<FoxMaterial> materials = GetMaterials(gameObject.transform);
        List<MeshFormat> meshFormats;

        GetObjects(gameObject.transform, meshes, materialInstances, textures, bones);
        GetMeshGroups(gameObject.transform, meshGroups, meshGroupEntries);
        meshFormats = GetMeshFormats(meshes);

        signature = 0x4c444d46;
        unknown0 = 0x40028f5c;
        unknown1 = 0x40;
        unknown2 = 0x776fff;
        unknown3 = 0x5;
        //numSection0Blocks = 0x14; //Temporary. Number can vary.
        //numSection1Blocks = 0x2; //Temporary. Should utilize GZ format.

        //Block 0 - Bones
        for(int i = 0; i < bones.Count; i++)
        {
            Section0Block0Entry s = new Section0Block0Entry();

            s.stringId = (ushort)strings.Count;
            strings.Add(bones[i].gameObject.name);

            if (bones[i].parent.gameObject.name == "[Root]")
                s.parentId = 0xFFFF;
            else
            {
                for (int j = 0; j < bones.Count; j++)
                    if (bones[i].parent == bones[j])
                    {
                        s.parentId = (ushort)j;
                        break;
                    } //if
            } //else

            s.boundingBoxId = (ushort)(i + 1); //Should work for now.
            s.unknown0 = 0x1;

            //Unity uses left-handed coordinates so x and z get flipped.
            s.localPositionX = bones[i].localPosition.z;
            s.localPositionY = bones[i].localPosition.y;
            s.localPositionZ = bones[i].localPosition.x;
            s.worldPositionX = bones[i].position.z;
            s.worldPositionY = bones[i].position.y;
            s.worldPositionZ = bones[i].position.x;

            section0Block0Entries.Add(s);
        } //for

        //Block 1 - Mesh Groups
        for(int i = 0; i < meshGroups.Count; i++)
        {
            Section0Block1Entry s = new Section0Block1Entry();

            s.stringId = (ushort)strings.Count;
            strings.Add(meshGroups[i].name);

            if (meshGroups[i].invisible)
                s.invisibilityFlag = 1;
            else
                s.invisibilityFlag = 0;

            if (i == 0)
                s.parentId = 0xFFFF;
            else
                s.parentId = 0;

            s.unknown0 = 0xFF;
        } //for

        //Block 2 - Mesh Group Assignments
        for(int i = 0; i < meshGroupEntries.Count; i++)
        {
            Section0Block2Entry s = new Section0Block2Entry();

            s.meshGroupId = (ushort)meshGroupEntries[i].meshGroupIndex;
            s.numObjects = (ushort)meshGroupEntries[i].numMeshes;

            if (i == 0)
                s.firstObjectId = 0;
            else
                s.firstObjectId = (ushort)(section0Block2Entries[i - 1].firstObjectId + section0Block2Entries[i - 1].numObjects);

            s.id = (ushort)i;
            s.unknown0 = 0;
        } //for

        //Block 3 - Meshes
        for (int i = 0; i < meshes.Count; i++)
        {
            Section0Block3Entry s = new Section0Block3Entry();

            s.unknown0 = 0x80;

            for (int j = 0; j < materialInstances.Count; j++)
                if (meshes[i].sharedMaterial = materialInstances[j])
                {
                    s.materialInstanceId = (ushort)j;
                    break;
                } //if

            s.boneGroupId = (ushort)i; //Might have to change if bone groups actually matter.
            s.id = (ushort)i;
            s.numVertices = (ushort)meshes[i].sharedMesh.vertexCount;

            if (i != 0)
                s.firstFaceVertexId = section0Block3Entries[i - 1].firstFaceVertexId + section0Block3Entries[i - 1].numFaceVertices;
            else
                s.firstFaceVertexId = 0;

            s.numFaceVertices = (ushort)meshes[i].sharedMesh.triangles.Length;
            s.firstMeshFormatId = (ushort)(i * 4); //might have to change the 4 depending on how many 0xA entries we end up having per mesh. It'll always be i * something though.

            section0Block3Entries.Add(s);
        } //for

        //Block 4 - Material Instances
        for (int i = 0; i < materialInstances.Count; i++)
        {
            Section0Block4Entry s = new Section0Block4Entry();

            s.stringId = (ushort)strings.Count;
            strings.Add(materialInstances[i].name);

            s.unknown0 = 0; //Probably just padding. Should remove.
            //s.materialId = 0;
            s.numTextures = 0;

            if (materialInstances[i].GetTexture("_MainTex"))
                s.numTextures++;
            if (materialInstances[i].GetTexture("_BumpMap"))
                s.numTextures++;

            //s.numParameters;

            if (i == 0)
                s.firstTextureId = 0;
            else
                s.firstTextureId = (ushort)(section0Block4Entries[i - 1].firstTextureId + section0Block4Entries[i - 1].numTextures);
            //s.firstParameterId;

            section0Block4Entries.Add(s);
        } //for

        //Block 5 - Bone Groups
        for (int i = 0; i < meshes.Count; i++)
        {
            Section0Block5Entry s = new Section0Block5Entry();
            List<int> indices = GetBoneGroup(meshes[i].sharedMesh);

            s.unknown0 = 0x4; //Most bone groups use 0x4. Dunno if it matters.
            s.numEntries = (ushort)indices.Count;
            s.entries = new ushort[indices.Count];

            for (int j = 0; j < indices.Count; j++)
            {
                s.entries[j] = (ushort)indices[j];
            } //for

            section0Block5Entries.Add(s);
        } //for

        UnityEngine.Debug.Log(textures.Count);
        //Block 6 - Textures
        for (int i = 0; i < textures.Count; i++)
        {
            Section0Block6Entry s = new Section0Block6Entry();

            string name = Path.GetFileNameWithoutExtension(textures[i].name);
            s.stringId = (ushort)strings.Count;
            strings.Add(name);

            string path = Path.GetDirectoryName(textures[i].name);
            bool add = true;

            for(int j = 0; j < strings.Count; j++)
            {
                if (path == strings[j])
                {
                    add = false;
                    s.pathId = (ushort)j;
                    break;
                } //if
            } //for

            if(add)
            {
                s.pathId = (ushort)strings.Count;
                strings.Add(path);
            } //if

            section0Block6Entries.Add(s);
        } //for

        //Block 7 - Texture Type/Material Parameter Assignments
        //To do....

        //Block 8 - Materials
        for(int i = 0; i < materials.Count; i++)
        {
            Section0Block8Entry s = new Section0Block8Entry();

            s.stringId = (ushort)strings.Count;
            strings.Add(materials[i].name);

            s.typeId = (ushort)strings.Count;
            strings.Add(materials[i].type);

            section0Block8Entries.Add(s);
        } //for

        //Block 9 - Mesh Format Assignments
        for(int i = 0; i < meshFormats.Count; i++)
        {
            Section0Block9Entry s = new Section0Block9Entry();

            byte numMeshFormatEntries = 0;
            byte numVertexFormatEntries = 0;

            if (meshFormats[i].meshFormat0Size > 0)
            {
                numMeshFormatEntries++;
                numVertexFormatEntries += meshFormats[i].meshFormat0Size;
            } //if

            if (meshFormats[i].meshFormat1Size > 0)
            {
                numMeshFormatEntries++;
                numVertexFormatEntries += meshFormats[i].meshFormat1Size;
            } //if

            if (meshFormats[i].meshFormat2Size > 0)
            {
                numMeshFormatEntries++;
                numVertexFormatEntries += meshFormats[i].meshFormat2Size;
            } //if

            if (meshFormats[i].meshFormat3Size > 0)
            {
                numMeshFormatEntries++;
                numVertexFormatEntries += meshFormats[i].meshFormat3Size;
            } //if

            s.numMeshFormatEntries = numMeshFormatEntries;
            s.numVertexFormatEntries = numVertexFormatEntries;
            s.unknown0 = 0x100;

            if (i == 0)
            {
                s.firstMeshFormatId = 0;
                s.firstVertexFormatId = 0;
            } //if
            else
            {
                s.firstMeshFormatId = (ushort)(section0Block9Entries[i - 1].firstMeshFormatId + section0Block9Entries[i - 1].numMeshFormatEntries);
                s.firstVertexFormatId = (ushort)(section0Block9Entries[i - 1].firstVertexFormatId + section0Block9Entries[i - 1].numVertexFormatEntries);
            } //else

            section0Block9Entries.Add(s);
        } //for

        //Block A - Mesh Formats
        for(int i = 0; i < meshFormats.Count; i++)
        {
            if(meshFormats[i].meshFormat0Size > 0)
            {
                Section0BlockAEntry s = new Section0BlockAEntry();
                s.bufferOffsetId = 0;
                s.numVertexFormatEntries = meshFormats[i].meshFormat0Size;
                s.length = 0xC;
                s.type = 0;
                s.offset = meshFormats[i].zeroOffset;
                section0BlockAEntries.Add(s);
            } //if

            if (meshFormats[i].meshFormat1Size > 0)
            {
                Section0BlockAEntry s = new Section0BlockAEntry();
                s.bufferOffsetId = 1;
                s.numVertexFormatEntries = meshFormats[i].meshFormat1Size;
                s.length = meshFormats[i].size;
                s.type = 1;
                s.offset = meshFormats[i].additionalOffset;
                section0BlockAEntries.Add(s);
            } //if

            if (meshFormats[i].meshFormat2Size > 0)
            {
                Section0BlockAEntry s = new Section0BlockAEntry();
                s.bufferOffsetId = 1;
                s.numVertexFormatEntries = meshFormats[i].meshFormat2Size;
                s.length = meshFormats[i].size;
                s.type = 2;
                s.offset = meshFormats[i].additionalOffset;
                section0BlockAEntries.Add(s);
            } //if

            if (meshFormats[i].meshFormat3Size > 0)
            {
                Section0BlockAEntry s = new Section0BlockAEntry();
                s.bufferOffsetId = 1;
                s.numVertexFormatEntries = meshFormats[i].meshFormat3Size;
                s.length = meshFormats[i].size;
                s.type = 3;
                s.offset = meshFormats[i].additionalOffset;
                section0BlockAEntries.Add(s);
            } //if
        } //for

        //Block B - Vertex Formats
        for(int i = 0; i < meshFormats.Count; i++)
        {
            ushort offset = 0;

            Section0BlockBEntry s = new Section0BlockBEntry();

            s.usage = 0;
            s.format = 1;
            s.offset = offset;

            section0BlockBEntries.Add(s);

            if(meshFormats[i].normals)
            {
                s = new Section0BlockBEntry();
                s.usage = 2;
                s.format = 6;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 8;
            } //if

            if (meshFormats[i].tangents)
            {
                s = new Section0BlockBEntry();
                s.usage = 0xE;
                s.format = 6;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 8;
            } //if

            if (meshFormats[i].colour)
            {
                s = new Section0BlockBEntry();
                s.usage = 3;
                s.format = 8;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].weights0)
            {
                s = new Section0BlockBEntry();
                s.usage = 1;
                s.format = 8;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].indices0)
            {
                s = new Section0BlockBEntry();
                s.usage = 7;
                s.format = 9;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].uv0)
            {
                s = new Section0BlockBEntry();
                s.usage = 8;
                s.format = 7;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].uv1)
            {
                s = new Section0BlockBEntry();
                s.usage = 9;
                s.format = 7;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].uv2)
            {
                s = new Section0BlockBEntry();
                s.usage = 0xA;
                s.format = 7;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].uv3)
            {
                s = new Section0BlockBEntry();
                s.usage = 0xB;
                s.format = 7;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].weights1)
            {
                s = new Section0BlockBEntry();
                s.usage = 0xC;
                s.format = 8;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 4;
            } //if

            if (meshFormats[i].indices1)
            {
                s = new Section0BlockBEntry();
                s.usage = 0xD;
                s.format = 4;
                s.offset = offset;
                section0BlockBEntries.Add(s);
                offset += 8;
            } //if
        } //for

        //Block C - Strings
        for(int i = 0; i < strings.Count; i++)
        {
            Section0BlockCEntry s = new Section0BlockCEntry();
            s.section1BlockId = 3;
            s.length = (ushort)strings[i].Length;

            if(i == 0)
            {
                s.offset = 0;
            } //if
            else
            {
                s.offset = section0BlockCEntries[i].offset + section0BlockCEntries[i].length + 1;
            } //else

            section0BlockCEntries.Add(s);
        } //for

        //Block D - Bounding Boxes
        //To do....

        //Block E - Buffer Offsets
        //Doing during actual file writing might be better.

        //Block 10 - LOD Info
        {
            Section0Block10Entry s = new Section0Block10Entry();
            s.unknown0 = 1;
            s.highDetailDistance = 1.0f;
            s.highDetailDistance = 1.0f;
            s.highDetailDistance = 1.0f;
            section0Block10Entries.Add(s);
        } //code block

        //Block 11 - Face Indices
        for(int i = 0; i < meshes.Count; i++)
        {
            Section0Block11Entry s = new Section0Block11Entry();
            s.firstFaceVertexId = 0;
            s.numFaceVertices = (uint)meshes[i].sharedMesh.triangles.Length;
            section0Block11Entries.Add(s);
        } //for

        //Block 12 - Unknown
        {
            Section0Block12Entry s = new Section0Block12Entry();
            s.unknown0 = 0;
            section0Block12Entries.Add(s);
        } //code block

        //Block 14 - Unknown
        {
            Section0Block14Entry s = new Section0Block14Entry();
            s.unknown0 = 3.33850384f;
            s.unknown1 = 0.8753322f;
            s.unknown2 = 0.200000048f;
            s.unknown3 = 5f;
            s.unknown4 = 5;
            s.unknown5 = 1;
        } //code block
    } //Write

    private void GetObjects(Transform transform, List<SkinnedMeshRenderer> meshes, List<Material> materialInstances, List<Texture> textures, List<Transform> bones)
    {
        GetMeshes(transform, meshes, materialInstances, textures);

        bones.AddRange(meshes[0].bones);
    } //GetObjects

    private void GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes, List<Material> materialInstances, List<Texture> textures)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>());

                bool add = true;

                for (int i = 0; i < materialInstances.Count; i++)
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial == materialInstances[i])
                        add = false;

                if (add)
                {
                    materialInstances.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial);

                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture)
                    {
                        for (int i = 0; i < textures.Count; i++)
                            if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture == textures[i])
                                add = false;

                        if (add)
                            textures.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture);
                    } //if

                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("_BumpMap"))
                    {
                        add = true;

                        for (int i = 0; i < textures.Count; i++)
                            if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("_BumpMap") == textures[i])
                                add = false;

                        if (add)
                            textures.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("_BumpMap"));
                    } //if
                } //if
            } //if

            GetMeshes(t, meshes, materialInstances, textures);
        } //foreach
    } //GetMeshes

    private void GetMeshGroups(Transform transform, List<MeshGroup> meshGroups, List<MeshGroupEntry> meshGroupEntries)
    {
        meshGroups = new List<MeshGroup>(0);
        meshGroupEntries = new List<MeshGroupEntry>(0);
        FoxModel foxModel = transform.GetComponent<FoxModel>();

        for(int i = 0; i < foxModel.definitions.Length; i++)
        {
            if (i != 0)
            {
                MeshGroup meshGroup = new MeshGroup();
                meshGroup.name = foxModel.definitions[i].meshGroup;
                meshGroup.invisible = false;
                meshGroups.Add(meshGroup);
            } //if
            else
            {
                MeshGroup meshGroup;

                if (foxModel.definitions[i].meshGroup != "MESH_ROOT")
                {
                    meshGroup = new MeshGroup();
                    meshGroup.name = "MESH_ROOT";
                    meshGroup.invisible = false;
                    meshGroups.Add(meshGroup);
                } //if

                meshGroup = new MeshGroup();
                meshGroup.name = foxModel.definitions[i].meshGroup;
                meshGroup.invisible = false;
                meshGroups.Add(meshGroup);
            } //else
        } //for

        for (int i = 0; i < foxModel.definitions.Length; i++)
        {
            if(i != 0)
            {
                if (foxModel.definitions[i].meshGroup == meshGroups[meshGroupEntries[meshGroupEntries.Count - 1].meshGroupIndex].name)
                    meshGroupEntries[meshGroupEntries.Count - 1].numMeshes++;
                else
                    for(int j = 0; j < meshGroups.Count; j++)
                        if(foxModel.definitions[i].meshGroup == meshGroups[j].name)
                        {
                            MeshGroupEntry meshGroupEntry = new MeshGroupEntry();
                            meshGroupEntry.meshGroupIndex = j;
                            meshGroupEntry.numMeshes = 1;
                            meshGroupEntries.Add(meshGroupEntry);
                            break;
                        } //if
            } //if
            else
            {
                for (int j = 0; j < meshGroups.Count; j++)
                    if (foxModel.definitions[i].meshGroup == meshGroups[j].name)
                    {
                        MeshGroupEntry meshGroupEntry = new MeshGroupEntry();
                        meshGroupEntry.meshGroupIndex = j;
                        meshGroupEntry.numMeshes = 1;
                        meshGroupEntries.Add(meshGroupEntry);
                        break;
                    } //if
            } //else
        } //for
    } //GetMeshGroups

    private List<int> GetBoneGroup(Mesh mesh)
    {
        List<int> indices = new List<int>(0);

        for (int i = 0; i < mesh.boneWeights.Length; i++)
        {
            int[] meshIndices = { mesh.boneWeights[i].boneIndex0, mesh.boneWeights[i].boneIndex1, mesh.boneWeights[i].boneIndex2, mesh.boneWeights[i].boneIndex3 };

            for (int j = 0; j < meshIndices.Length; j++)
            {
                bool add = true;

                for (int h = 0; h < indices.Count; h++)
                    if (meshIndices[j] == indices[h])
                    {
                        add = false;
                        break;
                    } //if

                if (add)
                    indices.Add(meshIndices[j]);
            } //for
        } //for

        return indices;
    } //GetBoneGroup

    private List<FoxMaterial> GetMaterials(Transform transform)
    {
        List<FoxMaterial> materials = new List<FoxMaterial>(0);
        FoxModel foxModel = transform.GetComponent<FoxModel>();

        for(int i = 0; i < foxModel.definitions.Length; i++)
        {
            bool add = true;

            for(int j = 0; j < materials.Count; j++)
            {
                if (foxModel.definitions[i].material == materials[j].name)
                {
                    add = false;
                    break;
                } //if
            } //for

            if(add)
            {
                FoxMaterial f = new FoxMaterial();
                f.name = foxModel.definitions[i].material;
                f.type = foxModel.definitions[i].materialType;
            } //add
        } //for

        return materials;
    } //GetMaterials

    private List<MeshFormat> GetMeshFormats(List<SkinnedMeshRenderer> meshes)
    {
        List<MeshFormat> meshFormats = new List<MeshFormat>(0);

        for(int i = 0; i < meshes.Count; i++)
        {
            MeshFormat meshFormat = new MeshFormat();

            if (i == 0)
            {
                meshFormat.zeroOffset = 0;
                meshFormat.additionalOffset = 0;
            } //if
            else
            {
                meshFormat.zeroOffset = (uint)meshes[i - 1].sharedMesh.vertices.Length * 0xC;
                meshFormat.additionalOffset = (uint)(meshFormats[i - 1].additionalOffset + meshFormats[i - 1].size * meshes[i - 1].sharedMesh.vertices.Length);
            } //else

            if (meshes[i].sharedMesh.normals.Length > 0)
            {
                meshFormat.normals = true;
                meshFormat.meshFormat1Size++;
                meshFormat.size += 8;
            } //if

            if (meshes[i].sharedMesh.tangents.Length > 0)
            {
                meshFormat.tangents = true;
                meshFormat.meshFormat1Size++;
                meshFormat.size += 8;
            } //if

            if (meshes[i].sharedMesh.colors.Length > 0)
            {
                meshFormat.colour = true;
                meshFormat.meshFormat2Size++;
                meshFormat.size += 4;
            } //if

            if (meshes[i].sharedMesh.boneWeights.Length > 0)
            {
                meshFormat.weights0 = true;
                meshFormat.indices0 = true;
                meshFormat.meshFormat3Size += 2;
                meshFormat.size += 8;
            } //if

            if (meshes[i].sharedMesh.uv.Length > 0)
            {
                meshFormat.uv0 = true;
                meshFormat.meshFormat3Size++;
                meshFormat.size += 4;
            } //if

            if (meshes[i].sharedMesh.uv2.Length > 0)
            {
                meshFormat.uv1 = true;
                meshFormat.meshFormat3Size++;
                meshFormat.size += 4;
            } //if

            if (meshes[i].sharedMesh.uv3.Length > 0)
            {
                meshFormat.uv2 = true;
                meshFormat.meshFormat3Size++;
                meshFormat.size += 4;
            } //if

            if (meshes[i].sharedMesh.uv4.Length > 0)
            {
                meshFormat.uv3 = true;
                meshFormat.meshFormat3Size++;
                meshFormat.size += 4;
            } //if

            meshFormat.weights1 = false;
            meshFormat.indices1 = false;

            meshFormats.Add(meshFormat);
        } //for

        return meshFormats;
    } //GetMeshFormats

    /*
        int numModelObjects = 1;
        Utils.GetNumObjects(fmdlObject.transform, ref numModelObjects);

        List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
        Utils.GetMeshes(fmdlObject.transform, meshes);

        UnityEngine.Debug.Log(meshes.Count);

        int[] faceCount = new int[meshes.Count];

        //Writes VBuffer Data
        for (int i = 0; i < meshes.Count; i++)
        {
            //Vertices
            for (int j = 0; j < meshes[i].sharedMesh.vertices.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.vertices[j].x);
                writer.Write(meshes[i].sharedMesh.vertices[j].y);
                writer.Write(meshes[i].sharedMesh.vertices[j].z);
            }

            //Normals
            for (int j = 0; j < meshes[i].sharedMesh.normals.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.normals[j].x);
                writer.Write(meshes[i].sharedMesh.normals[j].y);
                writer.Write(meshes[i].sharedMesh.normals[j].z);
            }

            //Tangents
            for (int j = 0; j < meshes[i].sharedMesh.tangents.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.tangents[j].x);
                writer.Write(meshes[i].sharedMesh.tangents[j].y);
                writer.Write(meshes[i].sharedMesh.tangents[j].z);
            }

            //Vertex Colour
            for (int j = 0; j < meshes[i].sharedMesh.colors.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.colors[j].r);
                writer.Write(meshes[i].sharedMesh.colors[j].g);
                writer.Write(meshes[i].sharedMesh.colors[j].b);
                writer.Write(meshes[i].sharedMesh.colors[j].a);
            }

            //Bone Weights
            for (int j = 0; j < meshes[i].sharedMesh.boneWeights.Length; j++)
            {
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].weight0);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].weight1);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].weight2);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].weight3);
            }

            //Bone Indices
            for (int j = 0; j < meshes[i].sharedMesh.boneWeights.Length; j++)
            {
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].boneIndex0);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].boneIndex1);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].boneIndex2);
                writer.Write(0xFF * meshes[i].sharedMesh.boneWeights[j].boneIndex3);
            }

            //UV1
            for (int j = 0; j < meshes[i].sharedMesh.uv.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.uv[j].x);
                writer.Write(meshes[i].sharedMesh.uv[j].y);
            }

            //UV2
            for (int j = 0; j < meshes[i].sharedMesh.uv2.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.uv2[j].x);
                writer.Write(meshes[i].sharedMesh.uv2[j].y);
            }

            //UV3
            for (int j = 0; j < meshes[i].sharedMesh.uv3.Length; j++)
            {
                writer.Write(meshes[i].sharedMesh.uv3[j].x);
                writer.Write(meshes[i].sharedMesh.uv3[j].y);
            }

            //Faces
            for (int j = 0; j < meshes[i].sharedMesh.triangles.Length; j++)
            {
                writer.Write((ushort)meshes[i].sharedMesh.triangles[j]);
            }
        }*/

    /****************************************************************
     * 
     * DEBUG/LOGGING FUNCTIONS
     * 
     ****************************************************************/
    [Conditional("DEBUG")]
    public void OutputSection0Block0Info()
    {
        for (int i = 0; i < section0Block0Entries.Count; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry ID: " + i);
            Console.WriteLine("Bone Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block0Entries[i].stringId]));
            Console.Write("Parent Bone: ");

            if (section0Block0Entries[i].parentId != 0xFFFF)
                Console.WriteLine(Hashing.TryGetStringName(section0Block16Entries[section0Block0Entries[section0Block0Entries[i].parentId].stringId]));
            else
                Console.WriteLine("Root");
        } //for
    } //OutputSection0Block0Info

    [Conditional("DEBUG")]
    public void OutputSection0Block2Info()
    {
        for (int i = 0; i < section0Block2Entries.Count; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry ID: " + section0Block2Entries[i].id);
            Console.WriteLine("Mesh Group: " + Hashing.TryGetStringName(section0Block16Entries[section0Block1Entries[section0Block2Entries[i].meshGroupId].stringId]));
            Console.WriteLine("Number of Objects: " + section0Block2Entries[i].numObjects);
            Console.WriteLine("Number of Preceding Objects: " + section0Block2Entries[i].firstObjectId);
            Console.WriteLine("Material ID: " + section0Block2Entries[i].unknown0);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block3Info()
    {
        for (int i = 0; i < section0Block3Entries.Count; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Unknown 0: " + section0Block3Entries[i].unknown0);
            Console.WriteLine("Material Id: " + section0Block3Entries[i].materialInstanceId);
            Console.WriteLine("Bone Group Id: " + section0Block3Entries[i].boneGroupId);
            Console.WriteLine("Id: " + section0Block3Entries[i].id);
            Console.WriteLine("Num Vertices " + section0Block3Entries[i].numVertices);
            Console.WriteLine("Face Offset: " + section0Block3Entries[i].firstFaceVertexId);
            Console.WriteLine("Num Face Vertices: " + section0Block3Entries[i].numFaceVertices);
            Console.WriteLine("Unknown 2: " + section0Block3Entries[i].firstMeshFormatId);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block4Info()
    {
        for (int i = 0; i < section0Block4Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block4Entries[i].stringId]));
            UnityEngine.Debug.Log("Material: " + Hashing.TryGetStringName(section0Block16Entries[section0Block8Entries[section0Block4Entries[i].materialId].stringId]));
            for (int j = section0Block4Entries[i].firstTextureId; j < section0Block4Entries[i].firstTextureId + section0Block4Entries[i].numTextures; j++)
            {
                UnityEngine.Debug.Log("Texture: " + Hashing.TryGetPathName(section0Block15Entries[section0Block7Entries[j].referenceId]));
                UnityEngine.Debug.Log("Texture Type: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[j].stringId]));
            } //for
        } //for
    } //OutputSection0Block4Info

    [Conditional("DEBUG")]
    public void OutputSection0Block5Info()
    {
        for (int i = 0; i < section0Block5Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);

            for (int j = 0; j < section0Block5Entries[i].entries.Length; j++)
            {
                UnityEngine.Debug.Log(Hashing.TryGetStringName(section0Block16Entries[section0Block0Entries[section0Block5Entries[i].entries[j]].stringId]));
            } //for
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block6Info()
    {
        for (int i = 0; i < section0Block6Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Name: " + strings[section0Block6Entries[i].stringId]);
            UnityEngine.Debug.Log("Texture: " + strings[section0Block6Entries[i].pathId]);
        } //for
    } //OutputSection0Block6Info

    [Conditional("DEBUG")]
    public void OutputSection0Block7Info()
    {
        for (int i = 0; i < section0Block7Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Texture Type/Material Parameter: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[i].stringId]));
            if (section0Block7Entries[i].referenceId < section0Block15Entries.Count)
                UnityEngine.Debug.Log("Texture: " + Hashing.TryGetPathName(section0Block15Entries[section0Block7Entries[i].referenceId]));
            else
                UnityEngine.Debug.Log("Material Reference: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[i].referenceId]));
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block8Info()
    {
        for (int i = 0; i < section0Block8Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Material: " + (Hashing.TryGetStringName(section0Block16Entries[section0Block8Entries[i].stringId])));
            UnityEngine.Debug.Log("Type: " + (Hashing.TryGetStringName(section0Block16Entries[section0Block8Entries[i].typeId])));
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0BlockDInfo()
    {
        for (int i = 0; i < section0BlockDEntries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Max Value: " + "X: " + section0BlockDEntries[i].maxX + " Y: " + section0BlockDEntries[i].maxY + " Z: " + section0BlockDEntries[i].maxZ + " W: " + section0BlockDEntries[i].maxW);
            UnityEngine.Debug.Log("Min Value: " + "X: " + section0BlockDEntries[i].minX + " Y: " + section0BlockDEntries[i].minY + " Z: " + section0BlockDEntries[i].minZ + " W: " + section0BlockDEntries[i].minW);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block16Info()
    {
        for (int i = 0; i < section0Block16Entries.Count; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Hash: " + section0Block16Entries[i].ToString("x"));
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputParamIds()
    {
        for (int i = 0; i < section0Block4Entries.Count; i++)
        {
            for (int j = section0Block4Entries[i].firstParameterId; j < section0Block4Entries[i].firstParameterId + section0Block4Entries[i].numParameters; j++)
            {
                UnityEngine.Debug.Log("Param Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[j].stringId]));
                UnityEngine.Debug.Log("Parameters: [" + materialParameters[section0Block7Entries[j].referenceId].values[0] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[1] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[2] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[3] + "]");
            } //for
        } //for ends
    } //OutputObjectInfo

    [Conditional("DEBUG")]
    public void OutputStringInfo()
    {
        for (int i = 0; i < strings.Count; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("String: " + strings[i]);
        } //for
    } //OutputStringInfo
} //class