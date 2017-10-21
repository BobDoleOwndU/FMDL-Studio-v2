using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Half;

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
        public ulong firstFaceIndexId;
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
    }

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

        //unconfirmed
        public Half tangentX;
        public Half tangentY;
        public Half tangentZ;
        public Half tangentW;

        public uint colour; //unconfirmed

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

    //Instance Variables
    private string name;

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

    private int bonesIndex = -1;
    private int meshGroupsIndex = -1;
    private int meshGroupAssignmentIndex = -1;
    private int meshInfoIndex = -1;
    private int materialInstancesIndex = -1;
    private int boneGroupsIndex = -1;
    private int texturesIndex = -1;
    private int textureTypesIndex = -1;
    private int materialsIndex = -1;
    private int meshFormatAssignmentIndex = -1;
    private int meshFormatsIndex = -1;
    private int vertexFormatsPosition = -1;
    private int stringsIndex = -1;
    private int boundingBoxesIndex = -1;
    private int bufferOffsetsIndex = -1;
    private int lodInfoIndex = -1;
    private int faceIndicesIndex = -1;
    private int type12Position = -1;
    private int type14Position = -1;
    private int texturePathsIndex = -1;
    private int stringHashesIndex = -1;

    private int section1MaterialParametersIndex = -1;
    private int section1MeshDataIndex = -1;
    private int section1StringsIndex = -1;

    private MaterialParameter[] materialParameters;
    private Object[] objects;
    private string[] strings;

    private Section0Info[] section0Info;
    private Section1Info[] section1Info;

    private Section0Block0Entry[] section0Block0Entries;
    private Section0Block1Entry[] section0Block1Entries;
    private Section0Block2Entry[] section0Block2Entries;
    private Section0Block3Entry[] section0Block3Entries;
    private Section0Block4Entry[] section0Block4Entries;
    private Section0Block5Entry[] section0Block5Entries;
    private Section0Block6Entry[] section0Block6Entries;
    private Section0Block7Entry[] section0Block7Entries;
    private Section0Block8Entry[] section0Block8Entries;
    private Section0Block9Entry[] section0Block9Entries;
    private Section0BlockAEntry[] section0BlockAEntries;
    private Section0BlockBEntry[] section0BlockBEntries;
    private Section0BlockCEntry[] section0BlockCEntries;
    private Section0BlockDEntry[] section0BlockDEntries;
    private Section0BlockEEntry[] section0BlockEEntries;
    private Section0Block10Entry[] section0Block10Entries;
    private Section0Block11Entry[] section0Block11Entries;
    private ulong[] section0Block15Entries;
    private ulong[] section0Block16Entries;

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
        if(File.Exists("fmdl_dictionary.txt"))
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
                    bonesIndex = i;
                    section0Block0Entries = new Section0Block0Entry[section0Info[bonesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshGroups:
                    meshGroupsIndex = i;
                    section0Block1Entries = new Section0Block1Entry[section0Info[meshGroupsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshGroupAssignment:
                    meshGroupAssignmentIndex = i;
                    section0Block2Entries = new Section0Block2Entry[section0Info[meshGroupAssignmentIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshInfo:
                    meshInfoIndex = i;
                    section0Block3Entries = new Section0Block3Entry[section0Info[meshInfoIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MaterialInstances:
                    materialInstancesIndex = i;
                    section0Block4Entries = new Section0Block4Entry[section0Info[materialInstancesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.BoneGroups:
                    boneGroupsIndex = i;
                    section0Block5Entries = new Section0Block5Entry[section0Info[boneGroupsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.Textures:
                    texturesIndex = i;
                    section0Block6Entries = new Section0Block6Entry[section0Info[texturesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.TextureTypes:
                    textureTypesIndex = i;
                    section0Block7Entries = new Section0Block7Entry[section0Info[textureTypesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.Materials:
                    materialsIndex = i;
                    section0Block8Entries = new Section0Block8Entry[section0Info[materialsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshFormatAssignment:
                    meshFormatAssignmentIndex = i;
                    section0Block9Entries = new Section0Block9Entry[section0Info[meshFormatAssignmentIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.MeshFormats:
                    meshFormatsIndex = i;
                    section0BlockAEntries = new Section0BlockAEntry[section0Info[meshFormatsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.VertexFormats:
                    vertexFormatsPosition = i;
                    section0BlockBEntries = new Section0BlockBEntry[section0Info[vertexFormatsPosition].numEntries];
                    break;
                case (ushort)Section0BlockType.Strings:
                    stringsIndex = i;
                    section0BlockCEntries = new Section0BlockCEntry[section0Info[stringsIndex].numEntries];
                    strings = new string[section0Info[stringsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.BoundingBoxes:
                    boundingBoxesIndex = i;
                    section0BlockDEntries = new Section0BlockDEntry[section0Info[boundingBoxesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.BufferOffsets:
                    bufferOffsetsIndex = i;
                    section0BlockEEntries = new Section0BlockEEntry[section0Info[bufferOffsetsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.LodInfo:
                    lodInfoIndex = i;
                    section0Block10Entries = new Section0Block10Entry[section0Info[lodInfoIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.FaceIndices:
                    faceIndicesIndex = i;
                    section0Block11Entries = new Section0Block11Entry[section0Info[faceIndicesIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.Type12:
                    type12Position = i;
                    break;
                case (ushort)Section0BlockType.Type14:
                    type14Position = i;
                    break;
                case (ushort)Section0BlockType.TexturePaths:
                    texturePathsIndex = i;
                    section0Block15Entries = new ulong[section0Info[texturePathsIndex].numEntries];
                    break;
                case (ushort)Section0BlockType.StringHashes:
                    stringHashesIndex = i;
                    section0Block16Entries = new ulong[section0Info[stringHashesIndex].numEntries];
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
                case (uint)Section1BlockType.MaterialParameters:
                    section1MaterialParametersIndex = i;
                    materialParameters = new MaterialParameter[(section1Info[section1MaterialParametersIndex].length / 4) / 4];
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

            for (int i = 0; i < section0Block0Entries.Length; i++)
            {
                section0Block0Entries[i].stringId = reader.ReadUInt16();
                section0Block0Entries[i].parentId = reader.ReadUInt16();
                section0Block0Entries[i].boundingBoxId = reader.ReadUInt16();
                section0Block0Entries[i].unknown0 = reader.ReadUInt16();
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
        if (meshGroupsIndex != -1)
        {
            //go to and get the section 0x1 entry info.
            reader.BaseStream.Position = section0Info[meshGroupsIndex].offset + section0Offset;

            for (int i = 0; i < section0Block1Entries.Length; i++)
            {
                section0Block1Entries[i].stringId = reader.ReadUInt16();
                section0Block1Entries[i].invisibilityFlag = reader.ReadUInt16();
                section0Block1Entries[i].parentId = reader.ReadUInt16();
                section0Block1Entries[i].unknown0 = reader.ReadUInt16();
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

            for (int i = 0; i < section0Block2Entries.Length; i++)
            {
                reader.BaseStream.Position += 0x4;
                section0Block2Entries[i].meshGroupId = reader.ReadUInt16();
                section0Block2Entries[i].numObjects = reader.ReadUInt16();
                section0Block2Entries[i].firstObjectId = reader.ReadUInt16();
                section0Block2Entries[i].id = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section0Block2Entries[i].unknown0 = reader.ReadUInt16();
                reader.BaseStream.Position += 0xE;
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

            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                section0Block3Entries[i].unknown0 = reader.ReadUInt32();
                section0Block3Entries[i].materialInstanceId = reader.ReadUInt16();
                section0Block3Entries[i].boneGroupId = reader.ReadUInt16();
                section0Block3Entries[i].id = reader.ReadUInt16();
                section0Block3Entries[i].numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section0Block3Entries[i].firstFaceVertexId = reader.ReadUInt32();
                section0Block3Entries[i].numFaceVertices = reader.ReadUInt32();
                section0Block3Entries[i].firstFaceIndexId = reader.ReadUInt64();
                reader.BaseStream.Position += 0x10;
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

            for (int i = 0; i < section0Block4Entries.Length; i++)
            {
                section0Block4Entries[i].stringId = reader.ReadUInt16();
                section0Block4Entries[i].unknown0 = reader.ReadUInt16();
                section0Block4Entries[i].materialId = reader.ReadUInt16();
                section0Block4Entries[i].numTextures = reader.ReadByte();
                section0Block4Entries[i].numParameters = reader.ReadByte();
                section0Block4Entries[i].firstTextureId = reader.ReadUInt16();
                section0Block4Entries[i].firstParameterId = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
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
         * SECTION 0 BLOCK 0x6 - TEXTURE DEFINITIONS
         *
         ****************************************************************/
        if (texturesIndex != -1)
        {
            //go to and get the section 0x6 entry info.
            reader.BaseStream.Position = section0Info[texturesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block6Entries.Length; i++)
            {
                section0Block6Entries[i].stringId = reader.ReadUInt16();
                section0Block6Entries[i].pathId = reader.ReadUInt16();
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

            for (int i = 0; i < section0Block7Entries.Length; i++)
            {
                section0Block7Entries[i].stringId = reader.ReadUInt16();
                section0Block7Entries[i].referenceId = reader.ReadUInt16();
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

            for (int i = 0; i < section0Block8Entries.Length; i++)
            {
                section0Block8Entries[i].stringId = reader.ReadUInt16();
                section0Block8Entries[i].typeId = reader.ReadUInt16();
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

            for (int i = 0; i < section0Block9Entries.Length; i++)
            {
                section0Block9Entries[i].numMeshFormatEntries = reader.ReadByte();
                section0Block9Entries[i].numVertexFormatEntries = reader.ReadByte();
                section0Block9Entries[i].unknown0 = reader.ReadUInt16();
                section0Block9Entries[i].firstMeshFormatId = reader.ReadUInt16();
                section0Block9Entries[i].firstVertexFormatId = reader.ReadUInt16();
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

            for (int i = 0; i < section0BlockAEntries.Length; i++)
            {
                section0BlockAEntries[i].bufferOffsetId = reader.ReadByte();
                section0BlockAEntries[i].numVertexFormatEntries = reader.ReadByte();
                section0BlockAEntries[i].length = reader.ReadByte();
                section0BlockAEntries[i].type = reader.ReadByte();
                section0BlockAEntries[i].offset = reader.ReadUInt32();
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

            for (int i = 0; i < section0BlockBEntries.Length; i++)
            {
                section0BlockBEntries[i].usage = reader.ReadByte();
                section0BlockBEntries[i].format = reader.ReadByte();
                section0BlockBEntries[i].offset = reader.ReadUInt16();
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

            for (int i = 0; i < section0BlockCEntries.Length; i++)
            {
                section0BlockCEntries[i].section1BlockId = reader.ReadUInt16();
                section0BlockCEntries[i].length = reader.ReadUInt16();
                section0BlockCEntries[i].offset = reader.ReadUInt32();
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

            for (int i = 0; i < section0BlockDEntries.Length; i++)
            {
                section0BlockDEntries[i].maxX = reader.ReadSingle();
                section0BlockDEntries[i].maxY = reader.ReadSingle();
                section0BlockDEntries[i].maxZ = reader.ReadSingle();
                section0BlockDEntries[i].maxW = reader.ReadSingle();
                section0BlockDEntries[i].minX = reader.ReadSingle();
                section0BlockDEntries[i].minY = reader.ReadSingle();
                section0BlockDEntries[i].minZ = reader.ReadSingle();
                section0BlockDEntries[i].minW = reader.ReadSingle();
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
         * SECTION 0 BLOCK 0x10 - LOD INFO
         *
         ****************************************************************/
        if (lodInfoIndex != -1)
        {
            //go to and get the section 0x10 entry info.
            reader.BaseStream.Position = section0Info[lodInfoIndex].offset + section0Offset;

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
         * SECTION 0 BLOCK 0x11 - LOD INFO
         *
         ****************************************************************/
        if (faceIndicesIndex != -1)
        {
            //go to and get the section 0x11 entry info.
            reader.BaseStream.Position = section0Info[faceIndicesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block11Entries.Length; i++)
            {
                section0Block11Entries[i].firstFaceVertexId = reader.ReadUInt32();
                section0Block11Entries[i].numFaceVertices = reader.ReadUInt32();
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

            for (int i = 0; i < section0Block15Entries.Length; i++)
            {
                section0Block15Entries[i] = reader.ReadUInt64();
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

            for (int i = 0; i < section0Block16Entries.Length; i++)
            {
                section0Block16Entries[i] = reader.ReadUInt64();
            } //for
        } //if

        /****************************************************************
         *
         * NUMERICAL PARAMETERS
         *
         ****************************************************************/
        reader.BaseStream.Position = section1Info[section1MaterialParametersIndex].offset + section1Offset;

        for(int i = 0; i < materialParameters.Length; i++)
        {
            materialParameters[i].values = new float[4];

            for (int j = 0; j < materialParameters[i].values.Length; j++)
                materialParameters[i].values[j] = reader.ReadSingle();
        } //for

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

        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].additionalVertexData = new AdditionalVertexData[section0Block3Entries[i].numVertices];

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
                            objects[i].additionalVertexData[j].colour = reader.ReadUInt32();
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
            for (int i = 0; i < section0BlockCEntries.Length; i++)
            {
                reader.BaseStream.Position = section1Offset + section1Info[section1StringsIndex].offset + section0BlockCEntries[i].offset;
                strings[i] = Encoding.Default.GetString(reader.ReadBytes(section0BlockCEntries[i].length));
            } //for
        } //if
    } //Read

    /****************************************************************
     * 
     * GETTERS
     * 
     ****************************************************************/
    public string GetName()
    {
        return name;
    } //GetName

    public int GetBonesPosition()
    {
        return bonesIndex;
    } //GetBonesPosition

    public int GetStringTablePosition()
    {
        return stringsIndex;
    } //GetBonesPosition

    public int GetTextureListPosition()
    {
        return texturePathsIndex;
    } //GetTextureListPosition

    public Object[] GetObjects()
    {
        return objects;
    } //GetObjects

    public string[] GetStrings()
    {
        return strings;
    } //GetStrings

    public Section0Block0Entry[] GetSection0Block0Entries()
    {
        return section0Block0Entries;
    } //GetSection0Block0Entries

    public Section0Block1Entry[] GetSection0Block1Entries()
    {
        return section0Block1Entries;
    } //GetSection0Block1Entries

    public Section0Block2Entry[] GetSection0Block2Entries()
    {
        return section0Block2Entries;
    } //GetSection0Block2Entries

    public Section0Block3Entry[] GetSection0Block3Entries()
    {
        return section0Block3Entries;
    } //GetSection0Block3Entries

    public Section0Block4Entry[] GetSection0Block4Entries()
    {
        return section0Block4Entries;
    } //GetSection0Block4Entries

    public Section0Block5Entry[] GetSection0Block5Entries()
    {
        return section0Block5Entries;
    } //GetSection0Block5Entries

    public Section0Block6Entry[] GetSection0Block6Entries()
    {
        return section0Block6Entries;
    } //GetSection0Block6Entries

    public Section0Block7Entry[] GetSection0Block7Entries()
    {
        return section0Block7Entries;
    } //GetSection0Block7Entries

    public Section0Block8Entry[] GetSection0Block8Entries()
    {
        return section0Block8Entries;
    } //GetSection0Block8Entries

    public Section0Block9Entry[] GetSection0Block9Entries()
    {
        return section0Block9Entries;
    } //GetSection0Block9Entries

    public Section0BlockDEntry[] GetSection0BlockDEntries()
    {
        return section0BlockDEntries;
    } //GetSection0BlockDEntries

    public Section0Block11Entry[] GetSection0Block11Entries()
    {
        return section0Block11Entries;
    } //GetSection0Block11Entries

    public ulong[] GetSection0Block15Entries()
    {
        return section0Block15Entries;
    } //GetSection0Block16Entries

    public ulong[] GetSection0Block16Entries()
    {
        return section0Block16Entries;
    } //GetSection0Block16Entries

    /****************************************************************
     * 
     * DEBUG/LOGGING FUNCTIONS
     * 
     ****************************************************************/
    [Conditional("DEBUG")]
    public void OutputSection0Block0Info()
    {
        for (int i = 0; i < section0Block0Entries.Length; i++)
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
        for (int i = 0; i < section0Block2Entries.Length; i++)
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
        for (int i = 0; i < section0Block3Entries.Length; i++)
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
            Console.WriteLine("Unknown 2: " + section0Block3Entries[i].firstFaceIndexId);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block4Info()
    {
        for (int i = 0; i < section0Block4Entries.Length; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block4Entries[i].stringId]));
            UnityEngine.Debug.Log("Material: " + Hashing.TryGetStringName(section0Block16Entries[section0Block8Entries[section0Block4Entries[i].materialId].stringId]));
            UnityEngine.Debug.Log("First Texture: " + Hashing.TryGetPathName(section0Block15Entries[section0Block7Entries[section0Block4Entries[i].firstTextureId].referenceId]));
            UnityEngine.Debug.Log("First Texture Type: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[section0Block4Entries[i].firstTextureId].stringId]));
        } //for
    } //OutputSection0Block4Info

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

    [Conditional("DEBUG")]
    public void OutputSection0Block6Info()
    {
        for(int i = 0; i < section0Block6Entries.Length; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block6Entries[i].stringId]));
            UnityEngine.Debug.Log("Texture: " + Hashing.TryGetPathName(section0Block15Entries[section0Block6Entries[i].pathId]));
        } //for
    } //OutputSection0Block6Info

    [Conditional("DEBUG")]
    public void OutputSection0Block7Info()
    {
        for (int i = 0; i < section0Block7Entries.Length; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Texture Type/Material Parameter: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[i].stringId]));
            if (section0Block7Entries[i].referenceId < section0Block15Entries.Length)
                UnityEngine.Debug.Log("Texture: " + Hashing.TryGetPathName(section0Block15Entries[section0Block7Entries[i].referenceId]));
            else
                UnityEngine.Debug.Log("Material Reference: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[i].referenceId]));
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block8Info()
    {
        for (int i = 0; i < section0Block8Entries.Length; i++)
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
        for (int i = 0; i < section0BlockDEntries.Length; i++)
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
        for (int i = 0; i < section0Block16Entries.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("Hash: " + section0Block16Entries[i].ToString("x"));
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputParamIds()
    {
        for(int i = 0; i < section0Block4Entries.Length; i++)
        {
            for(int j = section0Block4Entries[i].firstParameterId; j < section0Block4Entries[i].firstParameterId + section0Block4Entries[i].numParameters; j++)
            {
                UnityEngine.Debug.Log("Param Name: " + Hashing.TryGetStringName(section0Block16Entries[section0Block7Entries[j].stringId]));
                UnityEngine.Debug.Log("Parameters: [" + materialParameters[section0Block7Entries[j].referenceId].values[0] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[1] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[2] + ", " + materialParameters[section0Block7Entries[j].referenceId].values[3] + "]");
            } //for
        } //for ends
    } //OutputObjectInfo

    [Conditional("DEBUG")]
    public void OutputStringInfo()
    {
        for (int i = 0; i < strings.Length; i++)
        {
            Console.WriteLine("================================");
            Console.WriteLine("Entry No: " + i);
            Console.WriteLine("String: " + strings[i]);
        } //for
    } //OutputStringInfo
} //class