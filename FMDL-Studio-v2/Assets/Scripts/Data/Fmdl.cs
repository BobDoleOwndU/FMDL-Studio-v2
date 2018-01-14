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

    public class Section0Info
    {
        public ushort id;
        public ushort numEntries;
        public uint offset;
    } //struct

    public class Section1Info
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
        public float localPositionW;
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
        public ushort firstFaceIndexId;
    } //struct

    public struct Section0Block3Entry
    {
        public byte unknown0;
        public byte noShadowFlag;
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
        public List<ushort> entries;
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

    public class Section0BlockEEntry
    {
        public uint unknown0; //Flag of some sort?
        public uint length;
        public uint offset;
    } //struct

    public struct Section0Block10Entry
    {
        //variables here are assumptions. may not be correct.
        public uint numLods;
        public float unknown0;
        public float unknown1;
        public float unknown2;
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

    public class Object
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

    public struct FoxMaterialParameter
    {
        public string name;
        public float[] values;
    } //class

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
    private float versionNum;
    private ulong blocksOffset;
    private ulong section0BlockFlags;
    private ulong section1BlockFlags;
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
    public int vertexFormatsIndex { get; private set; } = -1;
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
    public List<Object> objects { get; private set; }
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
        versionNum = reader.ReadSingle();
        blocksOffset = reader.ReadUInt64();
        section0BlockFlags = reader.ReadUInt64();
        section1BlockFlags = reader.ReadUInt64();
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
                    vertexFormatsIndex = i;
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
                s.localPositionW = reader.ReadSingle();
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
                s.firstFaceIndexId = reader.ReadUInt16();
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

                s.unknown0 = reader.ReadByte();
                s.noShadowFlag = reader.ReadByte();
                reader.BaseStream.Position += 0x2;
                s.materialInstanceId = reader.ReadUInt16();
                s.boneGroupId = reader.ReadUInt16();
                s.id = reader.ReadUInt16();
                s.numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                s.firstFaceVertexId = reader.ReadUInt32();
                s.numFaceVertices = reader.ReadUInt32();
                s.firstFaceIndexId = reader.ReadUInt64();
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
                reader.BaseStream.Position += 0x2;
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
                s.entries = new List<ushort>(0);

                for (int j = 0; j < s.numEntries; j++)
                    s.entries.Add(reader.ReadUInt16());

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
        if (vertexFormatsIndex != -1)
        {
            //go to and get the section 0xB entry info.
            reader.BaseStream.Position = section0Info[vertexFormatsIndex].offset + section0Offset;

            for (int i = 0; i < section0Info[vertexFormatsIndex].numEntries; i++)
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

                s.numLods = reader.ReadUInt32();
                s.unknown0 = reader.ReadSingle();
                s.unknown1 = reader.ReadSingle();
                s.unknown2 = reader.ReadSingle();

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
        objects = new List<Object>(0);

        for (int i = 0; i < section0Block3Entries.Count; i++)
        {
            reader.BaseStream.Position = section1Info[section1MeshDataIndex].offset + section1Offset + section0BlockAEntries[section0Block9Entries[section0Block3Entries[i].id].firstMeshFormatId].offset;
            
            Object o = new Object();

            o.vertices = new Vertex[section0Block3Entries[i].numVertices];

            for (int j = 0; j < o.vertices.Length; j++)
            {
                o.vertices[j].x = reader.ReadSingle();
                o.vertices[j].y = reader.ReadSingle();
                o.vertices[j].z = reader.ReadSingle();
            } //for

            objects.Add(o);

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

        for (int i = 0; i < objects.Count; i++)
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
        for (int i = 0; i < objects.Count; i++)
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

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].lodFaces = new Face[section0Block10Entries[0].numLods][];

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
        List<List<FoxMaterialParameter>> meshParameters = new List<List<FoxMaterialParameter>>(0);

        GetObjects(gameObject.transform, meshes, materialInstances, textures, bones);
        GetMeshGroups(gameObject.transform, meshGroups, meshGroupEntries);
        meshFormats = GetMeshFormats(meshes);
        bones.Sort((x, y) => x.gameObject.name.CompareTo(y.gameObject.name));

        for (int i = 0; i < gameObject.GetComponent<FoxModel>().definitions.Length; i++)
        {
            List<FoxMaterialParameter> m = GetMaterialParameters(gameObject.GetComponent<FoxModel>().definitions[i].material);
            meshParameters.Add(m);
        } //for

        signature = 0x4c444d46;
        versionNum = 2.03f;
        blocksOffset = 0x40;
        section0BlockFlags = 0;
        section1BlockFlags = 0;
        section0Offset = 0;
        section0Length = 0;
        section1Offset = 0;
        section1Length = 0;

        strings.Add("");

        //Block 0 - Bones
        for (int i = 0; i < bones.Count; i++)
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
            s.localPositionW = 1f;
            s.worldPositionX = bones[i].position.z;
            s.worldPositionY = bones[i].position.y;
            s.worldPositionZ = bones[i].position.x;
            s.worldPositionW = 1f;

            section0Block0Entries.Add(s);
        } //for

        //Block 1 - Mesh Groups
        for (int i = 0; i < meshGroups.Count; i++)
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

            s.unknown0 = 0xFFFF;

            section0Block1Entries.Add(s);
        } //for

        //Block 2 - Mesh Group Assignments
        for (int i = 0; i < meshGroupEntries.Count; i++)
        {
            Section0Block2Entry s = new Section0Block2Entry();

            s.meshGroupId = (ushort)meshGroupEntries[i].meshGroupIndex;
            s.numObjects = (ushort)meshGroupEntries[i].numMeshes;

            if (i == 0)
                s.firstObjectId = 0;
            else
                s.firstObjectId = (ushort)(section0Block2Entries[i - 1].firstObjectId + section0Block2Entries[i - 1].numObjects);

            s.id = (ushort)i;
            s.firstFaceIndexId = s.firstObjectId;

            section0Block2Entries.Add(s);
        } //for

        //section0Block5Entries = DebugBoneGroups(bones); //DEBUG!

        //Block 3 - Meshes
        for (int i = 0; i < meshes.Count; i++)
        {
            Section0Block3Entry s = new Section0Block3Entry();

            s.unknown0 = 0;
            s.noShadowFlag = 0;

            for (int j = 0; j < materialInstances.Count; j++)
                if (meshes[i].sharedMaterial == materialInstances[j])
                {
                    s.materialInstanceId = (ushort)j;
                    break;
                } //if

            s.boneGroupId = (ushort)i;

            /*for(int j = 0; j < section0Block5Entries.Count; j++)
            {
                bool containsAll = true;
                List<int> boneIndices = GetBoneGroup(meshes[i], bones);

                for(int h = 0; h < boneIndices.Count; h++)
                {
                    if (!section0Block5Entries[j].entries.Contains((ushort)boneIndices[h]))
                    {
                        containsAll = false;
                        break;
                    } //if
                } //for

                if(containsAll)
                {
                    s.boneGroupId = (ushort)j;
                    break;
                } //if
            } //for*/

            s.id = (ushort)i;
            s.numVertices = (ushort)meshes[i].sharedMesh.vertexCount;

            if (i != 0)
                s.firstFaceVertexId = section0Block3Entries[i - 1].firstFaceVertexId + section0Block3Entries[i - 1].numFaceVertices;
            else
                s.firstFaceVertexId = 0;

            s.numFaceVertices = (ushort)meshes[i].sharedMesh.triangles.Length;
            s.firstFaceIndexId = (ushort)(i * 1); //might have to change the 4 depending on how many 0xA entries we end up having per mesh. It'll always be i * something though.

            section0Block3Entries.Add(s);
        } //for

        //Block 8 - Materials
        for (int i = 0; i < materials.Count; i++)
        {
            Section0Block8Entry s = new Section0Block8Entry();

            s.stringId = (ushort)strings.Count;
            strings.Add(materials[i].name);

            s.typeId = (ushort)strings.Count;
            strings.Add(materials[i].type);

            section0Block8Entries.Add(s);
        } //for

        //Block 4 - Material Instances
        for (int i = 0; i < materialInstances.Count; i++)
        {
            Section0Block4Entry s = new Section0Block4Entry();

            s.stringId = (ushort)strings.Count;

            if (materialInstances[i].name.Contains(" (UnityEngine.Material)"))
                materialInstances[i].name = materialInstances[i].name.Substring(0, materialInstances[i].name.IndexOf(" (UnityEngine.Material)"));

            strings.Add(materialInstances[i].name);

            s.numTextures = 0;

            if (materialInstances[i].GetTexture("_MainTex"))
                s.numTextures++;
            if (materialInstances[i].GetTexture("_BumpMap"))
                s.numTextures++;

            if (i == 0)
                s.firstTextureId = 0;
            else
            {
                if (section0Block4Entries[i - 1].firstParameterId >= section0Block4Entries[i - 1].firstTextureId)
                    s.firstTextureId = (ushort)(section0Block4Entries[i - 1].firstParameterId + section0Block4Entries[i - 1].numParameters);
                else
                    s.firstTextureId = (ushort)(section0Block4Entries[i - 1].firstTextureId + section0Block4Entries[i - 1].numTextures);
            } //else

            for (int j = 0; j < meshes.Count; j++)
            {
                if (meshes[j].sharedMaterial.name == materialInstances[i].name)
                {
                    string materialName = gameObject.GetComponent<FoxModel>().definitions[i].material;

                    for (int h = 0; h < section0Block8Entries.Count; h++)
                    {
                        if (materialName == strings[section0Block8Entries[h].stringId])
                        {
                            s.materialId = (ushort)h;
                            s.numParameters = (byte)meshParameters[h].Count;

                            if (i != 0)
                            {
                                if (section0Block4Entries[i - 1].firstParameterId + section0Block4Entries[i - 1].numParameters >= s.firstTextureId + s.numTextures)
                                    s.firstParameterId = (ushort)(section0Block4Entries[i - 1].firstParameterId + section0Block4Entries[i - 1].numParameters);
                                else
                                    s.firstParameterId = (ushort)(s.firstTextureId + s.numTextures);
                            } //if
                            else
                                s.firstParameterId = (ushort)(s.firstTextureId + s.numTextures);

                            break;
                        } //if
                    } //for

                    break;
                } //if
            } //for

            section0Block4Entries.Add(s);
        } //for

        //Block 5 - Bone Groups
        if (bones.Count > 0)
            for (int i = 0; i < meshes.Count; i++)
            {
                Section0Block5Entry s = new Section0Block5Entry();
                List<int> indices = GetBoneGroup(meshes[i], bones);

                s.unknown0 = 4; //Most bone groups use 0x4. Dunno if it matters.
                s.numEntries = (ushort)indices.Count;
                s.entries = new List<ushort>(0);

                for (int j = 0; j < indices.Count; j++)
                {
                    s.entries.Add((ushort)indices[j]);
                } //for

                section0Block5Entries.Add(s);
            } //for

        //Block 6 - Textures
        for (int i = 0; i < textures.Count; i++)
        {
            Section0Block6Entry s = new Section0Block6Entry();

            string fileName = textures[i].name.Replace('$', '/');

            string name = Path.GetFileNameWithoutExtension(fileName) + ".tga";
            s.stringId = (ushort)strings.Count;
            strings.Add(name);

            string path = Path.GetDirectoryName(fileName) + "/";
            bool add = true;

            for (int j = 0; j < strings.Count; j++)
            {
                if (path == strings[j])
                {
                    add = false;
                    s.pathId = (ushort)j;
                    break;
                } //if
            } //for

            if (add)
            {
                s.pathId = (ushort)strings.Count;
                strings.Add(path);
            } //if

            section0Block6Entries.Add(s);
        } //for

        //Block 7 - Texture Type/Material Parameter Assignments
        for (int i = 0; i < materialInstances.Count; i++)
        {
            if (materialInstances[i].GetTexture("_MainTex"))
            {
                Section0Block7Entry s = new Section0Block7Entry();

                for (int j = 0; j < textures.Count; j++)
                {
                    if (materialInstances[i].GetTexture("_MainTex") == textures[j])
                    {
                        s.referenceId = (ushort)j;
                        break;
                    } //if
                } //for

                bool add = true;

                for (int j = 0; j < strings.Count; j++)
                {
                    if (strings[j] == "Base_Tex_SRGB")
                    {
                        s.stringId = (ushort)j;
                        add = false;
                        break;
                    } //if
                } //for

                if (add)
                {
                    s.stringId = (ushort)strings.Count;
                    strings.Add("Base_Tex_SRGB");
                } //if

                section0Block7Entries.Add(s);
            } //if

            if (materialInstances[i].GetTexture("_BumpMap"))
            {
                Section0Block7Entry s = new Section0Block7Entry();

                for (int j = 0; j < textures.Count; j++)
                {
                    if (materialInstances[i].GetTexture("_BumpMap") == textures[j])
                    {
                        s.referenceId = (ushort)j;
                        break;
                    } //if
                } //for

                bool add = true;

                for (int j = 0; j < strings.Count; j++)
                {
                    if (strings[j] == "NormalMap_Tex_NRM")
                    {
                        s.stringId = (ushort)j;
                        add = false;
                        break;
                    } //if
                } //for

                if (add)
                {
                    s.stringId = (ushort)strings.Count;
                    strings.Add("NormalMap_Tex_NRM");
                } //if

                section0Block7Entries.Add(s);
            } //if

            for (int j = 0; j < meshParameters[section0Block4Entries[i].materialId].Count; j++)
            {
                Section0Block7Entry s = new Section0Block7Entry();

                bool add = true;

                for (int h = 0; h < strings.Count; h++)
                {
                    if (strings[h] == meshParameters[section0Block4Entries[i].materialId][j].name)
                    {
                        s.stringId = (ushort)h;
                        add = false;
                        break;
                    } //if
                } //for

                if (add)
                {
                    s.stringId = (ushort)strings.Count;
                    strings.Add(meshParameters[section0Block4Entries[i].materialId][j].name);
                } //if

                s.referenceId = (ushort)j;

                section0Block7Entries.Add(s);
            } //for
        } //for

        //Block 9 - Mesh Format Assignments
        for (int i = 0; i < meshFormats.Count; i++)
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
        for (int i = 0; i < meshFormats.Count; i++)
        {
            if (meshFormats[i].meshFormat0Size > 0)
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
        for (int i = 0; i < meshFormats.Count; i++)
        {
            ushort offset = 0;

            Section0BlockBEntry s = new Section0BlockBEntry();

            s.usage = 0;
            s.format = 1;
            s.offset = offset;

            section0BlockBEntries.Add(s);

            if (meshFormats[i].normals)
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
        for (int i = 0; i < strings.Count; i++)
        {
            Section0BlockCEntry s = new Section0BlockCEntry();
            s.section1BlockId = 3;
            s.length = (ushort)strings[i].Length;

            if (i == 0)
            {
                s.offset = 0;
            } //if
            else
            {
                s.offset = section0BlockCEntries[i - 1].offset + section0BlockCEntries[i - 1].length + 1;
            } //else

            section0BlockCEntries.Add(s);
        } //for

        //Block D - Bounding Boxes
        {
            Section0BlockDEntry s = new Section0BlockDEntry();

            foreach (Transform t in gameObject.transform)
            {
                if (t.gameObject.name == "[Root]")
                {
                    s.minX = t.gameObject.GetComponent<BoxCollider>().bounds.min.z;
                    s.minY = t.gameObject.GetComponent<BoxCollider>().bounds.min.y;
                    s.minZ = t.gameObject.GetComponent<BoxCollider>().bounds.min.x;
                    s.minW = 1f;
                    s.maxX = t.gameObject.GetComponent<BoxCollider>().bounds.max.z;
                    s.maxY = t.gameObject.GetComponent<BoxCollider>().bounds.max.y;
                    s.maxZ = t.gameObject.GetComponent<BoxCollider>().bounds.max.x;
                    s.maxW = 1f;
                    section0BlockDEntries.Add(s);
                    break;
                } //if
            } //foreach
        } //code block

        for (int i = 0; i < bones.Count; i++)
        {
            Section0BlockDEntry s = new Section0BlockDEntry();
            s.minX = bones[i].gameObject.GetComponent<BoxCollider>().bounds.min.z;
            s.minY = bones[i].gameObject.GetComponent<BoxCollider>().bounds.min.y;
            s.minZ = bones[i].gameObject.GetComponent<BoxCollider>().bounds.min.x;
            s.minW = 1f;
            s.maxX = bones[i].gameObject.GetComponent<BoxCollider>().bounds.max.z;
            s.maxY = bones[i].gameObject.GetComponent<BoxCollider>().bounds.max.y;
            s.maxZ = bones[i].gameObject.GetComponent<BoxCollider>().bounds.max.x;
            s.maxW = 1f;
            section0BlockDEntries.Add(s);
        } //for

        //Block E - Buffer Offsets
        for (int i = 0; i < 3; i++)
        {
            Section0BlockEEntry s = new Section0BlockEEntry();

            if (i != 2)
                s.unknown0 = 0;
            else
                s.unknown0 = 1;

            s.length = 0;
            s.offset = 0;

            section0BlockEEntries.Add(s);
        } //for

        //Block 10 - LOD Info
        {
            Section0Block10Entry s = new Section0Block10Entry();
            s.numLods = 1;
            s.unknown0 = 1.0f;
            s.unknown1 = 1.0f;
            s.unknown2 = 1.0f;
            section0Block10Entries.Add(s);
        } //code block

        //Block 11 - Face Indices
        for (int i = 0; i < meshes.Count; i++)
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
            section0Block14Entries.Add(s);
        } //code block

        //Objects
        objects = new List<Object>(0);

        for (int i = 0; i < meshes.Count; i++)
        {
            Object o = new Object();
            o.vertices = new Vertex[meshes[i].sharedMesh.vertices.Length];
            o.additionalVertexData = new AdditionalVertexData[meshes[i].sharedMesh.vertices.Length];
            o.faces = new Face[meshes[i].sharedMesh.triangles.Length / 3];

            for (int j = 0; j < o.vertices.Length; j++)
            {
                o.vertices[j].x = meshes[i].sharedMesh.vertices[j].z;
                o.vertices[j].y = meshes[i].sharedMesh.vertices[j].y;
                o.vertices[j].z = meshes[i].sharedMesh.vertices[j].x;
                o.additionalVertexData[j].normalX = new Half(meshes[i].sharedMesh.normals[j].z);
                o.additionalVertexData[j].normalY = new Half(meshes[i].sharedMesh.normals[j].y);
                o.additionalVertexData[j].normalZ = new Half(meshes[i].sharedMesh.normals[j].x);
                o.additionalVertexData[j].normalW = new Half(1f);

                if (meshes[i].sharedMesh.tangents.Length > 0)
                {
                    o.additionalVertexData[j].tangentX = new Half(meshes[i].sharedMesh.tangents[j].z);
                    o.additionalVertexData[j].tangentY = new Half(meshes[i].sharedMesh.tangents[j].y);
                    o.additionalVertexData[j].tangentZ = new Half(meshes[i].sharedMesh.tangents[j].x);
                    o.additionalVertexData[j].tangentW = new Half(meshes[i].sharedMesh.tangents[j].w);
                } //if

                //o.additionalVertexData[j].colourR = meshes[i].sharedMesh.colors[j].r;
                //o.additionalVertexData[j].colourG = meshes[i].sharedMesh.colors[j].g;
                //o.additionalVertexData[j].colourB = meshes[i].sharedMesh.colors[j].b;
                //o.additionalVertexData[j].colourA = meshes[i].sharedMesh.colors[j].a;
                
                if (meshes[i].sharedMesh.boneWeights.Length > 0)
                {
                    o.additionalVertexData[j].boneWeightX = meshes[i].sharedMesh.boneWeights[j].weight0;
                    o.additionalVertexData[j].boneWeightY = meshes[i].sharedMesh.boneWeights[j].weight1;
                    o.additionalVertexData[j].boneWeightZ = meshes[i].sharedMesh.boneWeights[j].weight2;
                    o.additionalVertexData[j].boneWeightW = meshes[i].sharedMesh.boneWeights[j].weight3;


                    for (int h = 0; h < section0Block5Entries[section0Block3Entries[i].boneGroupId].entries.Count; h++)
                    {
                        if (meshes[i].bones[meshes[i].sharedMesh.boneWeights[j].boneIndex0] == bones[section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[h]])
                        {
                            o.additionalVertexData[j].boneGroup0Id = (byte)h;
                            break;
                        } //if
                    } //for

                    for (int h = 0; h < section0Block5Entries[section0Block3Entries[i].boneGroupId].entries.Count; h++)
                    {
                        if ((meshes[i].bones[meshes[i].sharedMesh.boneWeights[j].boneIndex1] == bones[section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[h]]))
                        {
                            o.additionalVertexData[j].boneGroup1Id = (byte)h;
                            break;
                        } //if
                    } //for


                    for (int h = 0; h < section0Block5Entries[section0Block3Entries[i].boneGroupId].entries.Count; h++)
                    {
                        if ((meshes[i].bones[meshes[i].sharedMesh.boneWeights[j].boneIndex2] == bones[section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[h]]))
                        {
                            o.additionalVertexData[j].boneGroup2Id = (byte)h;
                            break;
                        } //if
                    } //for

                    for (int h = 0; h < section0Block5Entries[section0Block3Entries[i].boneGroupId].entries.Count; h++)
                    {
                        if ((meshes[i].bones[meshes[i].sharedMesh.boneWeights[j].boneIndex3] == bones[section0Block5Entries[section0Block3Entries[i].boneGroupId].entries[h]]))
                        {
                            o.additionalVertexData[j].boneGroup3Id = (byte)h;
                            break;
                        } //if
                    } //for
                } //if

                o.additionalVertexData[j].textureU = new Half(meshes[i].sharedMesh.uv[j].x);
                o.additionalVertexData[j].textureV = new Half(-meshes[i].sharedMesh.uv[j].y);

                if (meshes[i].sharedMesh.uv2.Length > 0)
                {
                    o.additionalVertexData[j].unknownU0 = new Half(meshes[i].sharedMesh.uv2[j].x);
                    o.additionalVertexData[j].unknownV0 = new Half(meshes[i].sharedMesh.uv2[j].y);

                    if (meshes[i].sharedMesh.uv3.Length > 0)
                    {
                        o.additionalVertexData[j].unknownU1 = new Half(meshes[i].sharedMesh.uv3[j].x);
                        o.additionalVertexData[j].unknownV1 = new Half(meshes[i].sharedMesh.uv3[j].y);

                        if (meshes[i].sharedMesh.uv4.Length > 0)
                        {
                            o.additionalVertexData[j].unknownU2 = new Half(meshes[i].sharedMesh.uv4[j].x);
                            o.additionalVertexData[j].unknownV2 = new Half(meshes[i].sharedMesh.uv4[j].y);
                        } //if
                    } //if
                } //if
            } //for

            for (int j = 0, h = 0; j < o.faces.Length; j++, h += 3)
            {
                o.faces[j].vertex1Id = (ushort)meshes[i].sharedMesh.triangles[h];
                o.faces[j].vertex2Id = (ushort)meshes[i].sharedMesh.triangles[h + 1];
                o.faces[j].vertex3Id = (ushort)meshes[i].sharedMesh.triangles[h + 2];
            } //for

            objects.Add(o);
        } //for

        //Section 0 Info
        if (section0Block0Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0;
            s.numEntries = (ushort)section0Block0Entries.Count;
            s.offset = 0;
            bonesIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 1;
        } //if

        if (section0Block1Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 1;
            s.numEntries = (ushort)section0Block1Entries.Count;
            s.offset = 0;
            meshGroupsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 2;
        } //if

        if (section0Block2Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 2;
            s.numEntries = (ushort)section0Block2Entries.Count;
            s.offset = 0;
            meshGroupAssignmentIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 4;
        } //if

        if (section0Block3Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 3;
            s.numEntries = (ushort)section0Block3Entries.Count;
            s.offset = 0;
            meshInfoIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 8;
        } //if

        if (section0Block4Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 4;
            s.numEntries = (ushort)section0Block4Entries.Count;
            s.offset = 0;
            materialInstancesIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 16;
        } //if

        if (section0Block5Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 5;
            s.numEntries = (ushort)section0Block5Entries.Count;
            s.offset = 0;
            boneGroupsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 32;
        } //if

        if (section0Block6Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 6;
            s.numEntries = (ushort)section0Block6Entries.Count;
            s.offset = 0;
            texturePathsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 64;
        } //if

        if (section0Block7Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 7;
            s.numEntries = (ushort)section0Block7Entries.Count;
            s.offset = 0;
            textureTypesIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 128;
        } //if

        if (section0Block8Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 8;
            s.numEntries = (ushort)section0Block8Entries.Count;
            s.offset = 0;
            materialsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 256;
        } //if

        if (section0Block9Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 9;
            s.numEntries = (ushort)section0Block9Entries.Count;
            s.offset = 0;
            meshFormatAssignmentIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 512;
        } //if

        if (section0BlockAEntries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0xA;
            s.numEntries = (ushort)section0BlockAEntries.Count;
            s.offset = 0;
            meshFormatsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 1024;
        } //if

        if (section0BlockBEntries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0xB;
            s.numEntries = (ushort)section0BlockBEntries.Count;
            s.offset = 0;
            vertexFormatsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 2048;
        } //if

        if (section0BlockCEntries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0xC;
            s.numEntries = (ushort)section0BlockCEntries.Count;
            s.offset = 0;
            stringsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 4096;
        } //if

        if (section0BlockDEntries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0xD;
            s.numEntries = (ushort)section0BlockDEntries.Count;
            s.offset = 0;
            boundingBoxesIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 8192;
        } //if

        if (section0BlockEEntries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0xE;
            s.numEntries = (ushort)section0BlockEEntries.Count;
            s.offset = 0;
            bufferOffsetsIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 16384;
        } //if

        if (section0Block10Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0x10;
            s.numEntries = (ushort)section0Block10Entries.Count;
            s.offset = 0;
            lodInfoIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 65536;
        } //if

        if (section0Block11Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0x11;
            s.numEntries = (ushort)section0Block11Entries.Count;
            s.offset = 0;
            faceIndicesIndex = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 131072;
        } //if

        if (section0Block12Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0x12;
            s.numEntries = (ushort)section0Block12Entries.Count;
            s.offset = 0;
            type12Index = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 262144;
        } //if

        if (section0Block14Entries.Count > 0)
        {
            Section0Info s = new Section0Info();
            s.id = 0x14;
            s.numEntries = (ushort)section0Block14Entries.Count;
            s.offset = 0;
            type14Index = section0Info.Count;
            section0Info.Add(s);
            section0BlockFlags += 1048576;
        } //if

        numSection0Blocks = (uint)section0Info.Count;

        //Section 1 Info
        if (meshParameters.Count > 0)
        {
            Section1Info s = new Section1Info();
            s.id = 0;
            s.offset = 0;
            s.length = 0;
            section1MaterialParametersIndex = section1Info.Count;
            section1Info.Add(s);
            section1BlockFlags += 1;
        } //if

        if (objects.Count > 0)
        {
            Section1Info s = new Section1Info();
            s.id = 2;
            s.offset = 0;
            s.length = 0;
            section1MeshDataIndex = section1Info.Count;
            section1Info.Add(s);
            section1BlockFlags += 4;
        } //if

        if (strings.Count > 0)
        {
            Section1Info s = new Section1Info();
            s.id = 3;
            s.offset = 0;
            s.length = 0;
            section1StringsIndex = section1Info.Count;
            section1Info.Add(s);
            section1BlockFlags += 8;
        } //if

        numSection1Blocks = (uint)section1Info.Count;

        //Time to Write!
        //Header
        writer.Write(signature);
        writer.Write(versionNum);
        writer.Write(blocksOffset);
        writer.Write(section0BlockFlags);
        writer.Write(section1BlockFlags);
        writer.Write(numSection0Blocks);
        writer.Write(numSection1Blocks);
        writer.Write(section0Offset);
        writer.Write(section0Length);
        writer.Write(section1Offset);
        writer.Write(section1Length);
        writer.WriteZeroes(8);

        //Section 0 Info
        for (int i = 0; i < section0Info.Count; i++)
        {
            writer.Write(section0Info[i].id);
            writer.Write(section0Info[i].numEntries);
            writer.Write(section0Info[i].offset);
        } //for

        //Section 1 Info
        for (int i = 0; i < section1Info.Count; i++)
        {
            writer.Write(section1Info[i].id);
            writer.Write(section1Info[i].offset);
            writer.Write(section1Info[i].length);
        } //for

        if (writer.BaseStream.Position % 0x10 != 0)
            writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));

        //write the section 0 offset.
        section0Offset = (uint)writer.BaseStream.Position;
        writer.BaseStream.Position = 0x28;
        writer.Write(section0Offset);
        writer.BaseStream.Position = section0Offset;

        if (bonesIndex != -1)
        {
            section0Info[bonesIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * bonesIndex;
            writer.Write(section0Info[bonesIndex].offset);
            writer.BaseStream.Position = section0Info[bonesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block0Entries.Count; i++)
            {
                writer.Write(section0Block0Entries[i].stringId);
                writer.Write(section0Block0Entries[i].parentId);
                writer.Write(section0Block0Entries[i].boundingBoxId);
                writer.Write(section0Block0Entries[i].unknown0);
                writer.WriteZeroes(8);
                writer.Write(section0Block0Entries[i].localPositionX);
                writer.Write(section0Block0Entries[i].localPositionY);
                writer.Write(section0Block0Entries[i].localPositionZ);
                writer.Write(section0Block0Entries[i].localPositionW);
                writer.Write(section0Block0Entries[i].worldPositionX);
                writer.Write(section0Block0Entries[i].worldPositionY);
                writer.Write(section0Block0Entries[i].worldPositionZ);
                writer.Write(section0Block0Entries[i].worldPositionW);
            } //for
        } //if

        if (meshGroupsIndex != -1)
        {
            section0Info[meshGroupsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * meshGroupsIndex;
            writer.Write(section0Info[meshGroupsIndex].offset);
            writer.BaseStream.Position = section0Info[meshGroupsIndex].offset + section0Offset;

            for (int i = 0; i < section0Block1Entries.Count; i++)
            {
                writer.Write(section0Block1Entries[i].stringId);
                writer.Write(section0Block1Entries[i].invisibilityFlag);
                writer.Write(section0Block1Entries[i].parentId);
                writer.Write(section0Block1Entries[i].unknown0);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (meshGroupAssignmentIndex != -1)
        {
            section0Info[meshGroupAssignmentIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * meshGroupAssignmentIndex;
            writer.Write(section0Info[meshGroupAssignmentIndex].offset);
            writer.BaseStream.Position = section0Info[meshGroupAssignmentIndex].offset + section0Offset;

            for (int i = 0; i < section0Block2Entries.Count; i++)
            {
                writer.WriteZeroes(4);
                writer.Write(section0Block2Entries[i].meshGroupId);
                writer.Write(section0Block2Entries[i].numObjects);
                writer.Write(section0Block2Entries[i].firstObjectId);
                writer.Write(section0Block2Entries[i].id);
                writer.WriteZeroes(4);
                writer.Write(section0Block2Entries[i].firstFaceIndexId);
                writer.WriteZeroes(0xE);
            } //for
        } //if

        if (meshInfoIndex != -1)
        {
            section0Info[meshInfoIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * meshInfoIndex;
            writer.Write(section0Info[meshInfoIndex].offset);
            writer.BaseStream.Position = section0Info[meshInfoIndex].offset + section0Offset;

            for (int i = 0; i < section0Block3Entries.Count; i++)
            {
                writer.Write(section0Block3Entries[i].unknown0);
                writer.Write(section0Block3Entries[i].noShadowFlag);
                writer.WriteZeroes(2);
                writer.Write(section0Block3Entries[i].materialInstanceId);
                writer.Write(section0Block3Entries[i].boneGroupId);
                writer.Write(section0Block3Entries[i].id);
                writer.Write(section0Block3Entries[i].numVertices);
                writer.WriteZeroes(4);
                writer.Write(section0Block3Entries[i].firstFaceVertexId);
                writer.Write(section0Block3Entries[i].numFaceVertices);
                writer.Write(section0Block3Entries[i].firstFaceIndexId);
                writer.WriteZeroes(0x10);
            } //for
        } //if

        if (materialInstancesIndex != -1)
        {
            section0Info[materialInstancesIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * materialInstancesIndex;
            writer.Write(section0Info[materialInstancesIndex].offset);
            writer.BaseStream.Position = section0Info[materialInstancesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block4Entries.Count; i++)
            {
                writer.Write(section0Block4Entries[i].stringId);
                writer.WriteZeroes(2);
                writer.Write(section0Block4Entries[i].materialId);
                writer.Write(section0Block4Entries[i].numTextures);
                writer.Write(section0Block4Entries[i].numParameters);
                writer.Write(section0Block4Entries[i].firstTextureId);
                writer.Write(section0Block4Entries[i].firstParameterId);
                writer.WriteZeroes(4);
            } //for
        } //if

        if (boneGroupsIndex != -1)
        {
            section0Info[boneGroupsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * boneGroupsIndex;
            writer.Write(section0Info[boneGroupsIndex].offset);
            writer.BaseStream.Position = section0Info[boneGroupsIndex].offset + section0Offset;

            for (int i = 0; i < section0Block5Entries.Count; i++)
            {
                writer.Write(section0Block5Entries[i].unknown0);

                if (section0Block5Entries[i].numEntries > 0)
                    writer.Write(section0Block5Entries[i].numEntries);
                else
                    writer.Write((ushort)1);

                for (int j = 0; j < section0Block5Entries[i].entries.Count; j++)
                    writer.Write(section0Block5Entries[i].entries[j]);

                writer.WriteZeroes(0x40 - 2 * section0Block5Entries[i].numEntries);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (texturePathsIndex != -1)
        {
            section0Info[texturePathsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * texturePathsIndex;
            writer.Write(section0Info[texturePathsIndex].offset);
            writer.BaseStream.Position = section0Info[texturePathsIndex].offset + section0Offset;

            for (int i = 0; i < section0Block6Entries.Count; i++)
            {
                writer.Write(section0Block6Entries[i].stringId);
                writer.Write(section0Block6Entries[i].pathId);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (textureTypesIndex != -1)
        {
            section0Info[textureTypesIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * textureTypesIndex;
            writer.Write(section0Info[textureTypesIndex].offset);
            writer.BaseStream.Position = section0Info[textureTypesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block7Entries.Count; i++)
            {
                writer.Write(section0Block7Entries[i].stringId);
                writer.Write(section0Block7Entries[i].referenceId);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (materialsIndex != -1)
        {
            section0Info[materialsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * materialsIndex;
            writer.Write(section0Info[materialsIndex].offset);
            writer.BaseStream.Position = section0Info[materialsIndex].offset + section0Offset;

            for (int i = 0; i < section0Block8Entries.Count; i++)
            {
                writer.Write(section0Block8Entries[i].stringId);
                writer.Write(section0Block8Entries[i].typeId);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (meshFormatAssignmentIndex != -1)
        {
            section0Info[meshFormatAssignmentIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * meshFormatAssignmentIndex;
            writer.Write(section0Info[meshFormatAssignmentIndex].offset);
            writer.BaseStream.Position = section0Info[meshFormatAssignmentIndex].offset + section0Offset;

            for (int i = 0; i < section0Block9Entries.Count; i++)
            {
                writer.Write(section0Block9Entries[i].numMeshFormatEntries);
                writer.Write(section0Block9Entries[i].numVertexFormatEntries);
                writer.Write(section0Block9Entries[i].unknown0);
                writer.Write(section0Block9Entries[i].firstMeshFormatId);
                writer.Write(section0Block9Entries[i].firstVertexFormatId);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (meshFormatsIndex != -1)
        {
            section0Info[meshFormatsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * meshFormatsIndex;
            writer.Write(section0Info[meshFormatsIndex].offset);
            writer.BaseStream.Position = section0Info[meshFormatsIndex].offset + section0Offset;

            for (int i = 0; i < section0BlockAEntries.Count; i++)
            {
                writer.Write(section0BlockAEntries[i].bufferOffsetId);
                writer.Write(section0BlockAEntries[i].numVertexFormatEntries);
                writer.Write(section0BlockAEntries[i].length);
                writer.Write(section0BlockAEntries[i].type);
                writer.Write(section0BlockAEntries[i].offset);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (vertexFormatsIndex != -1)
        {
            section0Info[vertexFormatsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * vertexFormatsIndex;
            writer.Write(section0Info[vertexFormatsIndex].offset);
            writer.BaseStream.Position = section0Info[vertexFormatsIndex].offset + section0Offset;

            for (int i = 0; i < section0BlockBEntries.Count; i++)
            {
                writer.Write(section0BlockBEntries[i].usage);
                writer.Write(section0BlockBEntries[i].format);
                writer.Write(section0BlockBEntries[i].offset);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (stringsIndex != -1)
        {
            section0Info[stringsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * stringsIndex;
            writer.Write(section0Info[stringsIndex].offset);
            writer.BaseStream.Position = section0Info[stringsIndex].offset + section0Offset;

            for (int i = 0; i < section0BlockCEntries.Count; i++)
            {
                writer.Write(section0BlockCEntries[i].section1BlockId);
                writer.Write(section0BlockCEntries[i].length);
                writer.Write(section0BlockCEntries[i].offset);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (boundingBoxesIndex != -1)
        {
            section0Info[boundingBoxesIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * boundingBoxesIndex;
            writer.Write(section0Info[boundingBoxesIndex].offset);
            writer.BaseStream.Position = section0Info[boundingBoxesIndex].offset + section0Offset;

            for (int i = 0; i < section0BlockDEntries.Count; i++)
            {
                writer.Write(section0BlockDEntries[i].maxX);
                writer.Write(section0BlockDEntries[i].maxY);
                writer.Write(section0BlockDEntries[i].maxZ);
                writer.Write(section0BlockDEntries[i].maxW);
                writer.Write(section0BlockDEntries[i].minX);
                writer.Write(section0BlockDEntries[i].minY);
                writer.Write(section0BlockDEntries[i].minZ);
                writer.Write(section0BlockDEntries[i].minW);
            } //for
        } //if

        if (bufferOffsetsIndex != -1)
        {
            section0Info[bufferOffsetsIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * bufferOffsetsIndex;
            writer.Write(section0Info[bufferOffsetsIndex].offset);
            writer.BaseStream.Position = section0Info[bufferOffsetsIndex].offset + section0Offset;

            for (int i = 0; i < section0BlockEEntries.Count; i++)
            {
                writer.Write(section0BlockEEntries[i].unknown0);
                writer.Write(section0BlockEEntries[i].length);
                writer.Write(section0BlockEEntries[i].offset);
                writer.WriteZeroes(4);
            } //for
        } //if

        if (lodInfoIndex != -1)
        {
            section0Info[lodInfoIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * lodInfoIndex;
            writer.Write(section0Info[lodInfoIndex].offset);
            writer.BaseStream.Position = section0Info[lodInfoIndex].offset + section0Offset;

            for (int i = 0; i < section0Block10Entries.Count; i++)
            {
                writer.Write(section0Block10Entries[i].numLods);
                writer.Write(section0Block10Entries[i].unknown0);
                writer.Write(section0Block10Entries[i].unknown1);
                writer.Write(section0Block10Entries[i].unknown2);
            } //for
        } //if

        if (faceIndicesIndex != -1)
        {
            section0Info[faceIndicesIndex].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * faceIndicesIndex;
            writer.Write(section0Info[faceIndicesIndex].offset);
            writer.BaseStream.Position = section0Info[faceIndicesIndex].offset + section0Offset;

            for (int i = 0; i < section0Block11Entries.Count; i++)
            {
                writer.Write(section0Block11Entries[i].firstFaceVertexId);
                writer.Write(section0Block11Entries[i].numFaceVertices);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        if (type12Index != -1)
        {
            section0Info[type12Index].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * type12Index;
            writer.Write(section0Info[type12Index].offset);
            writer.BaseStream.Position = section0Info[type12Index].offset + section0Offset;

            for (int i = 0; i < section0Block12Entries.Count; i++)
            {
                writer.Write(section0Block12Entries[i].unknown0);
            } //for
        } //if

        if (type14Index != -1)
        {
            section0Info[type14Index].offset = (uint)(writer.BaseStream.Position - section0Offset);
            writer.BaseStream.Position = 0x44 + 0x8 * type14Index;
            writer.Write(section0Info[type14Index].offset);
            writer.BaseStream.Position = section0Info[type14Index].offset + section0Offset;

            for (int i = 0; i < section0Block14Entries.Count; i++)
            {
                writer.WriteZeroes(4);
                writer.Write(section0Block14Entries[i].unknown0);
                writer.Write(section0Block14Entries[i].unknown1);
                writer.Write(section0Block14Entries[i].unknown2);
                writer.Write(section0Block14Entries[i].unknown3);
                writer.WriteZeroes(8);
                writer.Write(section0Block14Entries[i].unknown4);
                writer.Write(section0Block14Entries[i].unknown5);
                writer.WriteZeroes(0x5C);
            } //for

            //aligning will make it easier to check sections.
            if (writer.BaseStream.Position % 0x10 != 0)
                writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
        } //if

        //write section 0's length and section 1's offset.
        section0Length = (uint)(writer.BaseStream.Position - section0Offset);
        section1Offset = (uint)writer.BaseStream.Position;
        writer.BaseStream.Position = 0x2C;
        writer.Write(section0Length);
        writer.Write(section1Offset);
        writer.BaseStream.Position = section1Offset;

        //Section 1 Block 1
        if (section1MaterialParametersIndex != -1)
        {
            section1Info[section1MaterialParametersIndex].offset = (uint)(writer.BaseStream.Position - section1Offset);

            for (int i = 0; i < meshParameters.Count; i++)
                for (int j = 0; j < meshParameters[i].Count; j++)
                    for (int h = 0; h < meshParameters[i][j].values.Length; h++)
                        writer.Write(meshParameters[i][j].values[h]);

            section1Info[section1MaterialParametersIndex].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MaterialParametersIndex].offset);

            writer.BaseStream.Position = 0x44 + 0x8 * numSection0Blocks;
            writer.Write(section1Info[section1MaterialParametersIndex].offset);
            writer.Write(section1Info[section1MaterialParametersIndex].length);
            writer.BaseStream.Position = section1Offset + section1Info[section1MaterialParametersIndex].offset + section1Info[section1MaterialParametersIndex].length;
        } //if

        //Section 1 Block 2
        if (section1MeshDataIndex != -1)
        {
            section1Info[section1MeshDataIndex].offset = (uint)(writer.BaseStream.Position - section1Offset);
            section0BlockEEntries[0].offset = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset);

            //Positions
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = 0; j < objects[i].vertices.Length; j++)
                {
                    writer.Write(objects[i].vertices[j].x);
                    writer.Write(objects[i].vertices[j].y);
                    writer.Write(objects[i].vertices[j].z);
                } //for

                if (writer.BaseStream.Position % 0x10 != 0)
                    writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
            } //for

            section0BlockEEntries[0].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset - section0BlockEEntries[0].offset);

            section0BlockEEntries[1].offset = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset);

            //Additional Vertex Data.
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = 0; j < objects[i].additionalVertexData.Length; j++)
                    for (int h = section0Block9Entries[i].firstVertexFormatId; h < section0Block9Entries[i].firstVertexFormatId + section0Block9Entries[i].numVertexFormatEntries; h++)
                        switch (section0BlockBEntries[h].usage)
                        {
                            case 0: //vertex positions.
                                break;
                            case 1: //bone weights.
                                writer.Write((byte)Math.Round(objects[i].additionalVertexData[j].boneWeightX * 255f));
                                writer.Write((byte)Math.Round(objects[i].additionalVertexData[j].boneWeightY * 255f));
                                writer.Write((byte)Math.Round(objects[i].additionalVertexData[j].boneWeightZ * 255f));
                                writer.Write((byte)Math.Round(objects[i].additionalVertexData[j].boneWeightW * 255f));
                                break;
                            case 2: //normals.
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].normalX));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].normalY));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].normalZ));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].normalW));
                                break;
                            case 3: //diffuse.
                                writer.Write(objects[i].additionalVertexData[j].colourR);
                                writer.Write(objects[i].additionalVertexData[j].colourG);
                                writer.Write(objects[i].additionalVertexData[j].colourB);
                                writer.Write(objects[i].additionalVertexData[j].colourA);
                                break;
                            case 7: //bone indices.
                                writer.Write(objects[i].additionalVertexData[j].boneGroup0Id);
                                writer.Write(objects[i].additionalVertexData[j].boneGroup1Id);
                                writer.Write(objects[i].additionalVertexData[j].boneGroup2Id);
                                writer.Write(objects[i].additionalVertexData[j].boneGroup3Id);
                                break;
                            case 8: //UV.
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].textureU));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].textureV));
                                break;
                            case 9: //UV 2?
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownU0));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownV0));
                                break;
                            case 10: //UV 3?
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownU1));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownV1));
                                break;
                            case 11: //UV 4?
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownU2));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].unknownV2));
                                break;
                            case 12: //bone weights 2?
                                writer.Write(objects[i].additionalVertexData[j].unknownWeight0);
                                writer.Write(objects[i].additionalVertexData[j].unknownWeight1);
                                writer.Write(objects[i].additionalVertexData[j].unknownWeight2);
                                writer.Write(objects[i].additionalVertexData[j].unknownWeight3);
                                break;
                            case 13: //bone indices 2?
                                writer.Write(objects[i].additionalVertexData[j].unknownId0);
                                writer.Write(objects[i].additionalVertexData[j].unknownId1);
                                writer.Write(objects[i].additionalVertexData[j].unknownId2);
                                writer.Write(objects[i].additionalVertexData[j].unknownId3);
                                break;
                            case 14: //tangent.
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].tangentX));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].tangentY));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].tangentZ));
                                writer.Write(Half.GetBytes(objects[i].additionalVertexData[j].tangentW));
                                break;
                        } //switch

                if (writer.BaseStream.Position % 0x10 != 0)
                    writer.WriteZeroes((int)(0x10 - writer.BaseStream.Position % 0x10));
            } //for

            section0BlockEEntries[1].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset - section0BlockEEntries[1].offset);

            section0BlockEEntries[2].offset = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset);

            for(int i = 0; i < objects.Count; i++)
                for(int j = 0; j < objects[i].faces.Length; j++)
                {
                    writer.Write(objects[i].faces[j].vertex1Id);
                    writer.Write(objects[i].faces[j].vertex2Id);
                    writer.Write(objects[i].faces[j].vertex3Id);
                } //for

            section0BlockEEntries[2].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset - section0BlockEEntries[2].offset);
            section1Info[section1MeshDataIndex].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1MeshDataIndex].offset);

            //write offsets and lengths.
            writer.BaseStream.Position = writer.BaseStream.Position = 0x44 + 0x8 * numSection0Blocks + 0xC * section1MeshDataIndex;
            writer.Write(section1Info[section1MeshDataIndex].offset);
            writer.Write(section1Info[section1MeshDataIndex].length);

            for(int i = 0; i < section0BlockEEntries.Count; i++)
            {
                writer.BaseStream.Position = section0Offset + section0Info[bufferOffsetsIndex].offset + 0x10 * i + 0x4;
                writer.Write(section0BlockEEntries[i].length);
                writer.Write(section0BlockEEntries[i].offset);
            } //for

            writer.BaseStream.Position = section1Offset + section1Info[section1MeshDataIndex].offset + section1Info[section1MeshDataIndex].length;
        } //if

        //Section 1 Block 3
        if(section1StringsIndex != -1)
        {
            section1Info[section1StringsIndex].offset = (uint)(writer.BaseStream.Position - section1Offset);

            for(int i = 0; i < strings.Count; i++)
            {
                byte[] arr = Encoding.ASCII.GetBytes(Hashing.DenormalizeFilePath(strings[i]));

                writer.Write(arr);
                writer.WriteZeroes(1);
            } //for

            section1Info[section1StringsIndex].length = (uint)(writer.BaseStream.Position - section1Offset - section1Info[section1StringsIndex].offset);

            writer.BaseStream.Position = writer.BaseStream.Position = 0x44 + 0x8 * numSection0Blocks + 0xC * section1StringsIndex;
            writer.Write(section1Info[section1StringsIndex].offset);
            writer.Write(section1Info[section1StringsIndex].length);

            writer.BaseStream.Position = section1Offset + section1Info[section1StringsIndex].offset + section1Info[section1StringsIndex].length;
        } //if

        section1Length = (uint)(writer.BaseStream.Position - section1Offset);
        writer.BaseStream.Position = 0x34;
        writer.Write(section1Length);
    } //Write

    private void GetObjects(Transform transform, List<SkinnedMeshRenderer> meshes, List<Material> materialInstances, List<Texture> textures, List<Transform> bones)
    {
        GetMeshes(transform, meshes, materialInstances, textures);

        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "[Root]")
            {
                GetBones(t, bones);
                break;
            } //if
        } //foreach
    } //GetObjects

    public static void GetBones(Transform transform, List<Transform> bones)
    {
        foreach (Transform t in transform)
        {
            GetBones(t, bones);

            try
            {
                Convert.ToUInt64(t.gameObject.name, 16);
                //UnityEngine.Object.DestroyImmediate(t.gameObject);
            } //try
            catch
            {
                bones.Add(t);
            } //catch
        } //foreach
    } //GetBones

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
        FoxModel foxModel = transform.gameObject.GetComponent<FoxModel>();

        for (int i = 0; i < foxModel.definitions.Length; i++)
        {
            if (i != 0)
            {
                bool add = true;

                for (int j = 0; j < meshGroups.Count; j++)
                {
                    if (foxModel.definitions[i].meshGroup == meshGroups[j].name)
                    {
                        add = false;
                        break;
                    } //if
                } //for

                if (add)
                {
                    MeshGroup meshGroup = new MeshGroup();
                    meshGroup.name = foxModel.definitions[i].meshGroup;
                    meshGroup.invisible = false;
                    meshGroups.Add(meshGroup);
                } //if
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
            if (i != 0)
            {
                if (foxModel.definitions[i].meshGroup == meshGroups[meshGroupEntries[meshGroupEntries.Count - 1].meshGroupIndex].name)
                    meshGroupEntries[meshGroupEntries.Count - 1].numMeshes++;
                else
                    for (int j = 0; j < meshGroups.Count; j++)
                        if (foxModel.definitions[i].meshGroup == meshGroups[j].name)
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

    private List<int> GetBoneGroup(SkinnedMeshRenderer mesh, List<Transform> bones)
    {
        List<int> indices = new List<int>(0);

        for (int i = 0; i < mesh.bones.Length; i++)
            for (int j = 0; j < bones.Count; j++)
                if (mesh.bones[i] == bones[j])
                    indices.Add(j);

        /*for (int i = 0; i < bones.Count; i++)
            for (int j = 0; j < mesh.bones.Length; j++)
                if (bones[i] == mesh.bones[j])
                    indices.Add(i);*/

        return indices;
    } //GetBoneGroup

    private List<FoxMaterial> GetMaterials(Transform transform)
    {
        List<FoxMaterial> materials = new List<FoxMaterial>(0);
        FoxModel foxModel = transform.GetComponent<FoxModel>();

        for (int i = 0; i < foxModel.definitions.Length; i++)
        {
            bool add = true;

            for (int j = 0; j < materials.Count; j++)
            {
                if (foxModel.definitions[i].material == materials[j].name)
                {
                    add = false;
                    break;
                } //if
            } //for

            if (add)
            {
                FoxMaterial f = new FoxMaterial();
                f.name = foxModel.definitions[i].material;
                f.type = foxModel.definitions[i].materialType;
                materials.Add(f);
            } //add
        } //for

        return materials;
    } //GetMaterials

    private List<FoxMaterialParameter> GetMaterialParameters(string name)
    {
        List<FoxMaterialParameter> meshParameters = new List<FoxMaterialParameter>(0);

        FoxMaterialParameter meshParameter = new FoxMaterialParameter();

        switch (name)
        {
            case "fox_3ddf_skin_tension_dirty":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 140f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Roughness";
                meshParameter.values = new float[4] { 3f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Color";
                meshParameter.values = new float[4] { 1f, 1f, 1f, 0.1f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "TensionRate";
                meshParameter.values = new float[4] { 0f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "TensionShift";
                meshParameter.values = new float[4] { 0.5f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "TensionController";
                meshParameter.values = new float[4] { 0f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_skin_dirty":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 140f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Roughness";
                meshParameter.values = new float[4] { 3f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Color";
                meshParameter.values = new float[4] { 1f, 1f, 1f, 0.1f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_basic_multimt_dirty":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 116f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "MatParamIndex_1";
                meshParameter.values = new float[4] { 100f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "MatParamIndex_2";
                meshParameter.values = new float[4] { 256f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "MatParamIndex_3";
                meshParameter.values = new float[4] { 256f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "d75680ee296f":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 140f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_skin":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 116f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Roughness";
                meshParameter.values = new float[4] { 5f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Color";
                meshParameter.values = new float[4] { 0.8f, 0.8f, 0.8f, 0.3f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_eye":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 111f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "HeightScale";
                meshParameter.values = new float[4] { 0.6f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddc_basic":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 100f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_hair":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 119f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Anistropic_Diffusion";
                meshParameter.values = new float[4] { 16f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Anistropic_MainLightDir";
                meshParameter.values = new float[4] { 0f, 1f, -1f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Roughness";
                meshParameter.values = new float[4] { 4f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Color";
                meshParameter.values = new float[4] { 1f, 1f, 1f, 0.25f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "HairShiftScale";
                meshParameter.values = new float[4] { 1f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "URepeat_UV";
                meshParameter.values = new float[4] { 1f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "VRepeat_UV";
                meshParameter.values = new float[4] { 1f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3ddf_incidence_nrmuv_dirty":
                meshParameter.name = "MatParamIndex_0";
                meshParameter.values = new float[4] { 139f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Roughness";
                meshParameter.values = new float[4] { 1.5f, 0f, 0f, 0f };
                meshParameters.Add(meshParameter);

                meshParameter = new FoxMaterialParameter();
                meshParameter.name = "Incidence_Color";
                meshParameter.values = new float[4] { 1f, 1f, 1f, 0.15f };
                meshParameters.Add(meshParameter);
                break;

            case "fox_3dndw_shadow":
                break;
        } //switch

        return meshParameters;
    } //GetMeshParameters

    private List<MeshFormat> GetMeshFormats(List<SkinnedMeshRenderer> meshes)
    {
        List<MeshFormat> meshFormats = new List<MeshFormat>(0);

        for (int i = 0; i < meshes.Count; i++)
        {
            MeshFormat meshFormat = new MeshFormat();

            if (i == 0)
            {
                meshFormat.zeroOffset = 0;
                meshFormat.additionalOffset = 0;
            } //if
            else
            {
                meshFormat.zeroOffset = (uint)(meshFormats[i - 1].zeroOffset + meshes[i - 1].sharedMesh.vertices.Length * 0xC);

                if (meshFormat.zeroOffset % 0x10 != 0)
                    meshFormat.zeroOffset += 0x10 - meshFormat.zeroOffset % 0x10;

                meshFormat.additionalOffset = (uint)(meshFormats[i - 1].additionalOffset + meshFormats[i - 1].size * meshes[i - 1].sharedMesh.vertices.Length);

                if (meshFormat.additionalOffset % 0x10 != 0)
                    meshFormat.additionalOffset += 0x10 - meshFormat.additionalOffset % 0x10;
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

    /****************************************************************
     * 
     * DEBUG/LOGGING FUNCTIONS
     * 
     ****************************************************************/
    private List<Section0Block5Entry> DebugBoneGroups(List<Transform> bones)
    {
        List<Section0Block5Entry> section0Block5Entries = new List<Section0Block5Entry>(16);

        for(int i = 0; i < 16; i++)
        {
            Section0Block5Entry s = new Section0Block5Entry();
            List<string> names = new List<string>(0);

            switch (i)
            {
                case 0:
                    names.AddRange(new string[] { "SKL_003_NECK", "SKL_004_HEAD", "SKL_400_HEADROOT", "SKL_401_MOUTHROOT", "SKL_404_JAWS", "SKL_405_LPUL", "SKL_406_LPUR", "SKL_409_LPML", "SKL_410_LPMR", "SKL_411_LPNL", "SKL_413_LPNR", "SKL_415_LPUI", "SKL_420_THRT", "SKL_421_CHKL", "SKL_422_CHKR", "SKL_423_JAWL", "SKL_424_JAWR", "SKL_425_NOSE", "SKL_426_EBOL", "SKL_427_EBOR", "SKL_428_EBEL", "SKL_431_EYER", "SKL_432_EIUL", "SKL_434_EYLL", "SKL_436_MCKL", "SKL_530_CLVR_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for(int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));
                    
                    section0Block5Entries.Add(s);
                    break;
                case 1:
                    names.AddRange(new string[] { "SKL_004_HEAD", "SKL_400_HEADROOT", "SKL_405_LPUL", "SKL_406_LPUR", "SKL_409_LPML", "SKL_410_LPMR", "SKL_411_LPNL", "SKL_413_LPNR", "SKL_415_LPUI", "SKL_422_CHKR", "SKL_425_NOSE", "SKL_426_EBOL", "SKL_428_EBEL", "SKL_430_EYEL", "SKL_630_BRARD_SIM", "SKL_631_BRARD_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 2:
                    names.AddRange(new string[] { "SKL_003_NECK", "SKL_004_HEAD", "SKL_400_HEADROOT", "SKL_401_MOUTHROOT", "SKL_404_JAWS", "SKL_405_LPUL", "SKL_406_LPUR", "SKL_407_LPLL", "SKL_408_LPLR", "SKL_409_LPML", "SKL_410_LPMR", "SKL_411_LPNL", "SKL_412_LPCL", "SKL_413_LPNR", "SKL_414_LPCR", "SKL_416_LPLI", "SKL_417_MDBL", "SKL_418_MDBR", "SKL_420_THRT", "SKL_421_CHKL", "SKL_422_CHKR", "SKL_423_JAWL", "SKL_424_JAWR", "SKL_425_NOSE", "SKL_427_EBOR", "SKL_428_EBEL", "SKL_429_EBER", "SKL_430_EYEL", "SKL_433_EIUR", "SKL_435_EYLR", "SKL_436_MCKL", "SKL_437_MCKR" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 3:
                    names.AddRange(new string[] { "SKL_003_NECK", "SKL_004_HEAD", "SKL_400_HEADROOT", "SKL_401_MOUTHROOT", "SKL_404_JAWS", "SKL_405_LPUL", "SKL_406_LPUR", "SKL_407_LPLL", "SKL_408_LPLR", "SKL_409_LPML", "SKL_410_LPMR", "SKL_411_LPNL", "SKL_412_LPCL", "SKL_413_LPNR", "SKL_414_LPCR", "SKL_416_LPLI", "SKL_417_MDBL", "SKL_418_MDBR", "SKL_420_THRT", "SKL_421_CHKL", "SKL_422_CHKR", "SKL_423_JAWL", "SKL_424_JAWR", "SKL_425_NOSE", "SKL_428_EBEL", "SKL_436_MCKL", "SKL_437_MCKR", "SKL_530_CLVR_HLP", "SKL_630_BRARD_SIM", "SKL_631_BRARD_SIM", "SKL_632_BRARD_SIM", "SKL_633_BRARD_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 4:
                    names.AddRange(new string[] { "SKL_003_NECK", "SKL_004_HEAD", "SKL_400_HEADROOT", "SKL_404_JAWS", "SKL_413_LPNR", "SKL_419_TONE", "SKL_421_CHKL", "SKL_422_CHKR", "SKL_423_JAWL", "SKL_424_JAWR", "SKL_425_NOSE", "SKL_426_EBOL", "SKL_427_EBOR", "SKL_428_EBEL", "SKL_429_EBER", "SKL_431_EYER", "SKL_432_EIUL", "SKL_433_EIUR", "SKL_434_EYLL", "SKL_436_MCKL", "SKL_437_MCKR", "SKL_530_CLVR_HLP", "SKL_610_HAIR_SIM", "SKL_611_HAIR_SIM", "SKL_612_HAIR_SIM", "SKL_613_HAIR_SIM", "SKL_614_HAIR_SIM", "SKL_615_HAIR_SIM", "SKL_616_HAIR_SIM", "SKL_617_HAIR_SIM", "SKL_618_HAIR_SIM", "SKL_619_HAIR_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 5:
                    names.AddRange(new string[] { "SKL_011_LUARM", "SKL_012_LFARM", "SKL_013_LHAND", "SKL_101_LF10", "SKL_102_LF11", "SKL_103_LF12", "SKL_104_LF21", "SKL_105_LF22", "SKL_106_LF23", "SKL_107_LF31", "SKL_108_LF32", "SKL_109_LF33", "SKL_110_LF40", "SKL_111_LF41", "SKL_112_LF42", "SKL_113_LF43", "SKL_114_LF51", "SKL_502_LHMRS_HLP", "SKL_503_LUARL_HLP", "SKL_504_LELBW_HLP", "SKL_505_LFARL_HLP", "SKL_506_LTNB_HLP", "SKL_507_LTOW_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 6:
                    names.AddRange(new string[] { "SKL_002_CHEST", "SKL_010_LSHLD", "SKL_011_LUARM", "SKL_020_RSHLD", "SKL_021_RUARM", "SKL_500_LLRPZ_HLP", "SKL_502_LHMRS_HLP", "SKL_503_LUARL_HLP", "SKL_509_RLRPZ_HLP", "SKL_511_RHMRS_HLP", "SKL_512_RUARL_HLP", "SKL_530_CLVR_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 7:
                    names.AddRange(new string[] { "SKL_013_LHAND", "SKL_021_RUARM", "SKL_022_RFARM", "SKL_023_RHAND", "SKL_107_LF31", "SKL_110_LF40", "SKL_111_LF41", "SKL_114_LF51", "SKL_115_LF52", "SKL_116_LF53", "SKL_201_RF10", "SKL_202_RF11", "SKL_203_RF12", "SKL_204_RF21", "SKL_205_RF22", "SKL_206_RF23", "SKL_207_RF31", "SKL_208_RF32", "SKL_209_RF33", "SKL_210_RF40", "SKL_211_RF41", "SKL_212_RF42", "SKL_213_RF43", "SKL_214_RF51", "SKL_215_RF52", "SKL_216_RF53", "SKL_511_RHMRS_HLP", "SKL_512_RUARL_HLP", "SKL_513_RELBW_HLP", "SKL_514_RFARL_HLP", "SKL_515_RTNB_HLP", "SKL_516_RTOW_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 8:
                    names.AddRange(new string[] { "SKL_000_WAIST", "SKL_001_SPINE", "SKL_002_CHEST", "SKL_030_LTHIGH", "SKL_040_RTHIGH", "SKL_518_LBUD_HLP", "SKL_519_LMRF_HLP", "SKL_520_LPTL_HLP", "SKL_524_RBUD_HLP", "SKL_525_RMRF_HLP", "SKL_526_RPTL_HLP", "SKL_620_ITEM_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 9:
                    names.AddRange(new string[] { "SKL_030_LTHIGH", "SKL_031_LLEG", "SKL_032_LFOOT", "SKL_033_LTOE", "SKL_040_RTHIGH", "SKL_041_RLEG", "SKL_042_RFOOT", "SKL_043_RTOE", "SKL_518_LBUD_HLP", "SKL_519_LMRF_HLP", "SKL_520_LPTL_HLP", "SKL_524_RBUD_HLP", "SKL_525_RMRF_HLP", "SKL_526_RPTL_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 10:
                    names.AddRange(new string[] { "SKL_000_WAIST", "SKL_001_SPINE", "SKL_620_ITEM_SIM", "SKL_623_ITEM_SIM", "SKL_624_ITEM_SIM", "SKL_625_ITEM_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 11:
                    names.AddRange(new string[] { "SKL_620_ITEM_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 12:
                    names.AddRange(new string[] { "SKL_525_RMRF_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 13:
                    names.AddRange(new string[] { "SKL_000_WAIST", "SKL_001_SPINE", "SKL_620_ITEM_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 14:
                    names.AddRange(new string[] { "SKL_000_WAIST", "SKL_001_SPINE", "SKL_625_ITEM_SIM" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
                case 15:
                    names.AddRange(new string[] { "SKL_000_WAIST", "SKL_001_SPINE", "SKL_002_CHEST", "SKL_003_NECK", "SKL_004_HEAD", "SKL_010_LSHLD", "SKL_020_RSHLD", "SKL_040_RTHIGH", "SKL_420_THRT", "SKL_500_LLRPZ_HLP", "SKL_502_LHMRS_HLP", "SKL_503_LUARL_HLP", "SKL_509_RLRPZ_HLP", "SKL_511_RHMRS_HLP", "SKL_512_RUARL_HLP", "SKL_524_RBUD_HLP", "SKL_525_RMRF_HLP", "SKL_530_CLVR_HLP" });

                    s.unknown0 = 4;
                    s.numEntries = (ushort)names.Count;
                    s.entries = new List<ushort>(0);

                    for (int j = 0; j < names.Count; j++)
                        s.entries.Add((ushort)bones.IndexOf(bones.Find(x => x.gameObject.name == names[j])));

                    section0Block5Entries.Add(s);
                    break;
            } //switch
        } //for

        return section0Block5Entries;
    } //DebugBoneGroups


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
            Console.WriteLine("Material ID: " + section0Block2Entries[i].firstFaceIndexId);
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
            Console.WriteLine("Unknown 2: " + section0Block3Entries[i].firstFaceIndexId);
        } //for
    } //OutputSection2Info

    [Conditional("DEBUG")]
    public void OutputSection0Block4Info()
    {
        for (int i = 0; i < section0Block4Entries.Count; i++)
        {
            UnityEngine.Debug.Log("================================");
            UnityEngine.Debug.Log("Entry No: " + i);
            UnityEngine.Debug.Log("Name: " + strings[section0Block4Entries[i].stringId]);
            UnityEngine.Debug.Log("Material: " + strings[section0Block8Entries[section0Block4Entries[i].materialId].stringId]);

            for (int j = section0Block4Entries[i].firstTextureId; j < section0Block4Entries[i].firstTextureId + section0Block4Entries[i].numTextures; j++)
            {
                UnityEngine.Debug.Log("Texture Type: " + strings[section0Block7Entries[j].stringId]);
                UnityEngine.Debug.Log("Texture: " + strings[section0Block6Entries[section0Block7Entries[j].referenceId].pathId] + strings[section0Block6Entries[section0Block7Entries[j].referenceId].stringId]);
            } //for

            for (int j = section0Block4Entries[i].firstParameterId; j < section0Block4Entries[i].firstParameterId + section0Block4Entries[i].numParameters; j++)
            {
                UnityEngine.Debug.Log("Parameter Type: " + strings[section0Block7Entries[j].stringId]);
                UnityEngine.Debug.Log(string.Format("Parameters: {{{0}, {1}, {2}, {3}}}", materialParameters[section0Block7Entries[j].referenceId].values[0], materialParameters[section0Block7Entries[j].referenceId].values[1], materialParameters[section0Block7Entries[j].referenceId].values[2], materialParameters[section0Block7Entries[j].referenceId].values[3]));
            } //for
        } //for
    } //OutputSection0Block4Info

    [Conditional("DEBUG")]
    public void OutputSection0Block5Info()
    {
        using (FileStream stream = new FileStream("debug.txt", FileMode.Create))
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            for (int i = 0; i < section0Block5Entries.Count; i++)
            {
                /*UnityEngine.Debug.Log("================================");
                UnityEngine.Debug.Log("Entry No: " + i);*/
                if (i != 0)
                    writer.Write("\n");

                writer.WriteLine("================================");
                writer.WriteLine("Entry No: " + i);

                for (int j = 0; j < section0Block5Entries[i].entries.Count; j++)
                {
                    //UnityEngine.Debug.Log(Hashing.TryGetStringName(section0Block16Entries[section0Block0Entries[section0Block5Entries[i].entries[j]].stringId]));
                    if(stringsIndex != -1)
                        writer.Write($"\"{strings[section0Block0Entries[section0Block5Entries[i].entries[j]].stringId]}\", ");
                    else
                        writer.Write($"\"{Hashing.TryGetStringName(section0Block16Entries[section0Block0Entries[section0Block5Entries[i].entries[j]].stringId])}\", ");
                } //for
            } //for

            stream.Close();
        } //using
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
            UnityEngine.Debug.Log("Texture Type/Material Parameter: " + strings[section0Block7Entries[i].stringId]);
            if(section0Block7Entries[i].referenceId < section0Block6Entries.Count)
                UnityEngine.Debug.Log("Reference: " + strings[section0Block6Entries[section0Block7Entries[i].referenceId].stringId]);
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