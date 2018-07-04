using System;
using System.IO;
using UnityEngine;

public class ExpFmdl
{
    private enum Section0BlockType
    {
        Bones = 0,
        MeshGroups = 1,
        MeshGroupEntries = 2,
        MeshInfo = 3,
        MaterialInstances = 4,
        BoneGroups = 5,
        Textures = 6,
        MaterialParameters = 7,
        Materials = 8,
        MeshFormatInfo = 9,
        MeshFormats = 10,
        VertexFormats = 11,
        StringInfo = 12,
        BoundingBoxes = 13,
        BufferOffsets = 14,
        LodInfo = 16,
        FaceInfo = 17,
        Type12 = 18,
        Type14 = 20,
        PathCode64s = 21,
        StrCode64s = 22
    }; //Section0BlockType

    private enum Section1BlockType
    {
        MaterialParameterFloats = 0,
        Buffer = 2,
        Strings = 3
    }; //Section1BlockType

    public struct Section0Info
    {
        public ushort type;
        public ushort entryCount;
        public uint offset;
    } //struct

    public struct Section1Info
    {
        public uint type;
        public uint offset;
        public uint length;
    } //struct

    //0x0
    public struct FmdlBone
    {
        public ushort nameIndex;
        public short parentIndex;
        public ushort boundingBoxIndex;
        public ushort unknown0; //always 0x1?
        public Vector4 localPosition;
        public Vector4 worldPosition;
    } //struct

    //0x1
    public struct FmdlMeshGroup
    {
        public ushort nameIndex;
        public ushort invisibilityFlag;
        public short parentIndex;
        public short unknown0; //always 0xFF
    } //struct

    //0x2
    public struct FmdlMeshGroupEntry
    {
        public ushort meshGroupIndex;
        public ushort meshCount;
        public ushort firstMeshIndex;
        public ushort index;
        public ushort firstFaceInfoIndex;
    } //struct

    //0x3
    public struct FmdlMeshInfo
    {
        public byte alphaEnum;
        public byte shadowEnum;
        public ushort materialInstanceIndex;
        public ushort boneGroupIndex;
        public ushort index;
        public ushort vertexCount;
        public uint firstFaceVertexIndex;
        public uint faceVertexCount;
        public ulong firstFaceInfoIndex;
    } //struct

    //0x4
    public struct FmdlMaterialInstance
    {
        public ushort nameIndex;
        public ushort materialIndex;
        public byte textureCount;
        public byte parameterCount;
        public ushort firstTextureIndex;
        public ushort firstParameterIndex;
    } //struct

    //0x5
    public struct FmdlBoneGroup
    {
        public ushort unknown0;
        public ushort boneIndexCount;
        public ushort[] boneIndices;
    } //struct

    //0x6
    public struct FmdlTexture
    {
        public ushort nameIndex;
        public ushort pathIndex;
    } //struct

    //0x7
    public struct FmdlMaterialParameter
    {
        public ushort nameIndex;
        public ushort referenceIndex;
    } //struct

    //0x8
    public struct FmdlMaterial
    {
        public ushort nameIndex;
        public ushort typeIndex;
    } //struct

    //0x9
    public struct FmdlMeshFormatInfo
    {
        public byte meshFormatCount;
        public byte vertexFormatCount;
        public ushort unknown0;
        public ushort firstMeshFormatIndex;
        public ushort firstVertexFormatIndex;
    } //Section0Block9Entry

    //0xA
    public struct FmdlMeshFormat
    {
        public byte bufferOffsetIndex;
        public byte vertexFormatCount;
        public byte length;
        public byte type;
        public uint offset;
    } //struct

    //0xB
    public struct FmdlVertexFormat
    {
        public byte type;
        public byte dataType;
        public ushort offset;
    } //struct

    //0xC
    public struct FmdlStringInfo
    {
        public ushort section1BlockIndex;
        public ushort length;
        public uint offset;
    } //struct

    //0xD
    public struct FmdlBoundingBox
    {
        public Vector4 max;
        public Vector4 min;
    } //struct

    //0xE
    public class FmdlBufferOffset
    {
        public uint unknown0; //Flag of some sort?
        public uint length;
        public uint offset;
    } //struct

    //0x10
    public struct FmdlLodInfo
    {
        public uint lodCount;
        public float unknown0;
        public float unknown1;
        public float unknown2;
    } //struct

    //0x11
    public struct FmdlFaceInfo
    {
        public uint firstFaceVertexIndex;
        public uint faceVertexCount;
    } //struct

    //0x12
    public struct FmdlType12
    {
        public ulong unknown0; //Always 0.
    } //struct

    //0x14
    public struct FmdlType14
    {
        public float unknown0; //Nulling triggers lowest LOD faces.
        public float unknown1;
        public float unknown2;
        public float unknown3; //Always a whole number?
        public uint unknown4;
        public uint unknown5;
    } //struct

    public struct FmdlMesh
    {
        public Vector3[] vertices;
        public Vector4Half[] normals;
        public Vector4Half[] tangents;
        public Vector4[] colors;
        public Vector4[] boneWeights;
        public Vector4[] boneIndices;
        public Vector2Half[] uv;
        public Vector2Half[] uv2;
        public Vector2Half[] uv3;
        public Vector2Half[] uv4;
        public Vector4[] unknownWeights;
        public Vector4[] unknownIndices;
        public ushort[] triangles;
    } //struct

    //Class vars
    public string name;

    private int bonesIndex = -1;
    private int meshGroupsIndex = -1;
    private int meshGroupEntriesIndex = -1;
    private int meshInfoIndex = -1;
    private int materialInstancesIndex = -1;
    private int boneGroupsIndex = -1;
    private int texturesIndex = -1;
    private int materialParametersIndex = -1;
    private int materialsIndex = -1;
    private int meshFormatInfoIndex = -1;
    private int meshFormatsIndex = -1;
    private int vertexFormatsIndex = -1;
    private int stringInfoIndex = -1;
    private int boundingBoxesIndex = -1;
    private int bufferOffsetsIndex = -1;
    private int lodInfoIndex = -1;
    private int faceInfoIndex = -1;
    private int type12Index = -1;
    private int type14Index = -1;
    private int pathCode64sIndex = -1;
    private int strCode64sIndex = -1;

    private int materialParameterFloatsIndex = -1;
    private int bufferIndex = -1;
    private int stringsIndex = -1;

    //Fmdl vars
    private uint signature;
    public float version { get; private set; }
    private ulong sectionInfoOffset;
    private ulong section0BlockFlags;
    private ulong section1BlockFlags;
    private uint section0BlockCount;
    private uint section1BlockCount;
    private uint section0Offset;
    private uint section0Length;
    private uint section1Offset;
    private uint section1Length;
    public Section0Info[] section0Infos { get; private set; }
    public Section1Info[] section1Infos { get; private set; }
    public FmdlBone[] fmdlBones { get; private set; }
    public FmdlMeshGroup[] fmdlMeshGroups { get; private set; }
    public FmdlMeshGroupEntry[] fmdlMeshGroupEntries { get; private set; }
    public FmdlMeshInfo[] fmdlMeshInfos { get; private set; }
    public FmdlMaterialInstance[] fmdlMaterialInstances { get; private set; }
    public FmdlBoneGroup[] fmdlBoneGroups { get; private set; }
    public FmdlTexture[] fmdlTextures { get; private set; }
    public FmdlMaterialParameter[] fmdlMaterialParameters { get; private set; }
    public FmdlMaterial[] fmdlMaterials { get; private set; }
    public FmdlMeshFormatInfo[] fmdlMeshFormatInfos { get; private set; }
    public FmdlMeshFormat[] fmdlMeshFormats { get; private set; }
    public FmdlVertexFormat[] fmdlVertexFormats { get; private set; }
    public FmdlStringInfo[] fmdlStringInfos { get; private set; }
    public FmdlBoundingBox[] fmdlBoundingBoxes { get; private set; }
    public FmdlBufferOffset[] fmdlBufferOffsets { get; private set; }
    public FmdlLodInfo[] fmdlLodInfos { get; private set; }
    public FmdlFaceInfo[] fmdlFaceInfos { get; private set; }
    public FmdlType12[] fmdlType12s { get; private set; }
    public FmdlType14[] fmdlType14s { get; private set; }
    public ulong[] fmdlPathCode64s { get; private set; }
    public ulong[] fmdlStrCode64s { get; private set; }

    public Vector4[] fmdlMaterialParameterVectors { get; private set; }
    public FmdlMesh[] fmdlMeshes { get; private set; }
    public string[] fmdlStrings { get; private set; }

    public ExpFmdl(string name)
    {
        this.name = name;
    } //constructor

    public void Read(FileStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        if (File.Exists("Assets/fmdl_dictionary.txt"))
            Hashing.ReadStringDictionary("Assets/fmdl_dictionary.txt");

        if (File.Exists("Assets/qar_dictionary.txt"))
            Hashing.ReadPathDictionary("Assets/qar_dictionary.txt");

        ReadHeader(reader);
        ReadSectionInfo(reader);

        if (bonesIndex != -1)
            ReadBones(reader);
        if (meshGroupsIndex != -1)
            ReadMeshGroups(reader);
        if (meshGroupEntriesIndex != -1)
            ReadMeshGroupEntries(reader);
        if (meshInfoIndex != -1)
            ReadMeshInfo(reader);
        if (materialInstancesIndex != -1)
            ReadMaterialInstances(reader);
        if (boneGroupsIndex != -1)
            ReadBoneGroups(reader);
        if (texturesIndex != -1)
            ReadTextures(reader);
        if (materialParametersIndex != -1)
            ReadMaterialParameters(reader);
        if (materialsIndex != -1)
            ReadMaterials(reader);
        if (meshFormatInfoIndex != -1)
            ReadMeshFormatInfo(reader);
        if (meshFormatsIndex != -1)
            ReadMeshFormats(reader);
        if (vertexFormatsIndex != -1)
            ReadVertexFormats(reader);
        if (stringInfoIndex != -1)
            ReadStringInfo(reader);
        if (boundingBoxesIndex != -1)
            ReadBoundingBoxes(reader);
        if (bufferOffsetsIndex != -1)
            ReadBufferOffsets(reader);
        if (lodInfoIndex != -1)
            ReadLodInfo(reader);
        if (faceInfoIndex != -1)
            ReadFaceInfo(reader);
        if (type12Index != -1)
            ReadType12(reader);
        if (type14Index != -1)
            ReadType14(reader);
        if (pathCode64sIndex != -1)
            ReadPathCode64s(reader);
        if (strCode64sIndex != -1)
            ReadStrCode64s(reader);
        if (materialParameterFloatsIndex != -1)
            ReadMaterialParameterFloats(reader);
        if (bufferIndex != -1)
            ReadBuffer(reader);
        if (stringsIndex != -1)
            ReadStrings(reader);
    } //Read

    private void ReadHeader(BinaryReader reader)
    {
        signature = reader.ReadUInt32();
        version = reader.ReadSingle();
        sectionInfoOffset = reader.ReadUInt64();
        section0BlockFlags = reader.ReadUInt64();
        section1BlockFlags = reader.ReadUInt64();
        section0BlockCount = reader.ReadUInt32();
        section1BlockCount = reader.ReadUInt32();
        section0Offset = reader.ReadUInt32();
        section0Length = reader.ReadUInt32();
        section1Offset = reader.ReadUInt32();
        section1Length = reader.ReadUInt32();
    } //ReadHeader

    private void ReadSectionInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = (long)sectionInfoOffset;

        section0Infos = new Section0Info[section0BlockCount];
        section1Infos = new Section1Info[section1BlockCount];

        for (int i = 0; i < section0BlockCount; i++)
        {
            Section0Info section0Info = new Section0Info();

            section0Info.type = reader.ReadUInt16();
            section0Info.entryCount = reader.ReadUInt16();
            section0Info.offset = reader.ReadUInt32();

            switch (section0Info.type)
            {
                case (ushort)Section0BlockType.Bones:
                    bonesIndex = i;
                    fmdlBones = new FmdlBone[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MeshGroups:
                    meshGroupsIndex = i;
                    fmdlMeshGroups = new FmdlMeshGroup[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MeshGroupEntries:
                    meshGroupEntriesIndex = i;
                    fmdlMeshGroupEntries = new FmdlMeshGroupEntry[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MeshInfo:
                    meshInfoIndex = i;
                    fmdlMeshInfos = new FmdlMeshInfo[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MaterialInstances:
                    materialInstancesIndex = i;
                    fmdlMaterialInstances = new FmdlMaterialInstance[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.BoneGroups:
                    boneGroupsIndex = i;
                    fmdlBoneGroups = new FmdlBoneGroup[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.Textures:
                    texturesIndex = i;
                    fmdlTextures = new FmdlTexture[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MaterialParameters:
                    materialParametersIndex = i;
                    fmdlMaterialParameters = new FmdlMaterialParameter[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.Materials:
                    materialsIndex = i;
                    fmdlMaterials = new FmdlMaterial[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MeshFormatInfo:
                    meshFormatInfoIndex = i;
                    fmdlMeshFormatInfos = new FmdlMeshFormatInfo[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.MeshFormats:
                    meshFormatsIndex = i;
                    fmdlMeshFormats = new FmdlMeshFormat[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.VertexFormats:
                    vertexFormatsIndex = i;
                    fmdlVertexFormats = new FmdlVertexFormat[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.StringInfo:
                    stringInfoIndex = i;
                    fmdlStringInfos = new FmdlStringInfo[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.BoundingBoxes:
                    boundingBoxesIndex = i;
                    fmdlBoundingBoxes = new FmdlBoundingBox[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.BufferOffsets:
                    bufferOffsetsIndex = i;
                    fmdlBufferOffsets = new FmdlBufferOffset[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.LodInfo:
                    lodInfoIndex = i;
                    fmdlLodInfos = new FmdlLodInfo[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.FaceInfo:
                    faceInfoIndex = i;
                    fmdlFaceInfos = new FmdlFaceInfo[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.Type12:
                    type12Index = i;
                    fmdlType12s = new FmdlType12[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.Type14:
                    type14Index = i;
                    fmdlType14s = new FmdlType14[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.PathCode64s:
                    pathCode64sIndex = i;
                    fmdlPathCode64s = new ulong[section0Info.entryCount];
                    break;
                case (ushort)Section0BlockType.StrCode64s:
                    strCode64sIndex = i;
                    fmdlStrCode64s = new ulong[section0Info.entryCount];
                    break;
            } //switch

            section0Infos[i] = section0Info;
        } //for

        for (int i = 0; i < section1BlockCount; i++)
        {
            Section1Info section1Info = new Section1Info();

            section1Info.type = reader.ReadUInt32();
            section1Info.offset = reader.ReadUInt32();
            section1Info.length = reader.ReadUInt32();

            switch (section1Info.type)
            {
                case (uint)Section1BlockType.MaterialParameterFloats:
                    materialParameterFloatsIndex = i;
                    fmdlMaterialParameterVectors = new Vector4[section1Info.length / 10];
                    break;
                case (uint)Section1BlockType.Buffer:
                    bufferIndex = i;
                    fmdlMeshes = new FmdlMesh[section0Infos[meshInfoIndex].entryCount];
                    break;
                case (uint)Section1BlockType.Strings:
                    stringsIndex = i;
                    fmdlStrings = new string[section0Infos[stringInfoIndex].entryCount];
                    break;
            } //switch

            section1Infos[i] = section1Info;
        } //for
    } //ReadSectionInfo

    private void ReadBones(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[bonesIndex].offset;

        for (int i = 0; i < section0Infos[bonesIndex].entryCount; i++)
        {
            FmdlBone fmdlBone = new FmdlBone();

            fmdlBone.nameIndex = reader.ReadUInt16();
            fmdlBone.parentIndex = reader.ReadInt16();
            fmdlBone.boundingBoxIndex = reader.ReadUInt16();
            fmdlBone.unknown0 = reader.ReadUInt16();
            reader.BaseStream.Position += 8;
            fmdlBone.localPosition = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            fmdlBone.worldPosition = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            fmdlBones[i] = fmdlBone;
        } //for
    } //ReadBones

    private void ReadMeshGroups(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[meshGroupsIndex].offset;

        for (int i = 0; i < section0Infos[meshGroupsIndex].entryCount; i++)
        {
            FmdlMeshGroup fmdlMeshGroup = new FmdlMeshGroup();

            fmdlMeshGroup.nameIndex = reader.ReadUInt16();
            fmdlMeshGroup.invisibilityFlag = reader.ReadUInt16();
            fmdlMeshGroup.parentIndex = reader.ReadInt16();
            fmdlMeshGroup.unknown0 = reader.ReadInt16();

            fmdlMeshGroups[i] = fmdlMeshGroup;
        } //for
    } //ReadMeshGroups

    private void ReadMeshGroupEntries(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[meshGroupEntriesIndex].offset;

        for (int i = 0; i < section0Infos[meshGroupEntriesIndex].entryCount; i++)
        {
            FmdlMeshGroupEntry fmdlMeshGroupEntry = new FmdlMeshGroupEntry();

            reader.BaseStream.Position += 4;
            fmdlMeshGroupEntry.meshGroupIndex = reader.ReadUInt16();
            fmdlMeshGroupEntry.meshCount = reader.ReadUInt16();
            fmdlMeshGroupEntry.firstMeshIndex = reader.ReadUInt16();
            fmdlMeshGroupEntry.index = reader.ReadUInt16();
            reader.BaseStream.Position += 4;
            fmdlMeshGroupEntry.firstFaceInfoIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 0xE;

            fmdlMeshGroupEntries[i] = fmdlMeshGroupEntry;
        } //for
    } //ReadMeshGroupEntries

    private void ReadMeshInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[meshInfoIndex].offset;

        for (int i = 0; i < section0Infos[meshInfoIndex].entryCount; i++)
        {
            FmdlMeshInfo fmdlMeshInfo = new FmdlMeshInfo();

            fmdlMeshInfo.alphaEnum = reader.ReadByte();
            fmdlMeshInfo.shadowEnum = reader.ReadByte();
            reader.BaseStream.Position += 2;
            fmdlMeshInfo.materialInstanceIndex = reader.ReadUInt16();
            fmdlMeshInfo.boneGroupIndex = reader.ReadUInt16();
            fmdlMeshInfo.index = reader.ReadUInt16();
            fmdlMeshInfo.vertexCount = reader.ReadUInt16();
            reader.BaseStream.Position += 4;
            fmdlMeshInfo.firstFaceVertexIndex = reader.ReadUInt32();
            fmdlMeshInfo.faceVertexCount = reader.ReadUInt32();
            fmdlMeshInfo.firstFaceInfoIndex = reader.ReadUInt64();
            reader.BaseStream.Position += 0x10;

            fmdlMeshInfos[i] = fmdlMeshInfo;
        } //for
    } //ReadMeshInfo

    private void ReadMaterialInstances(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[materialInstancesIndex].offset;

        for (int i = 0; i < section0Infos[materialInstancesIndex].entryCount; i++)
        {
            FmdlMaterialInstance fmdlMaterialInstance = new FmdlMaterialInstance();

            fmdlMaterialInstance.nameIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2;
            fmdlMaterialInstance.materialIndex = reader.ReadUInt16();
            fmdlMaterialInstance.textureCount = reader.ReadByte();
            fmdlMaterialInstance.parameterCount = reader.ReadByte();
            fmdlMaterialInstance.firstTextureIndex = reader.ReadUInt16();
            fmdlMaterialInstance.firstParameterIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 4;

            fmdlMaterialInstances[i] = fmdlMaterialInstance;
        } //for
    } //ReadMaterialInstances

    private void ReadBoneGroups(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[boneGroupsIndex].offset;

        for (int i = 0; i < section0Infos[boneGroupsIndex].entryCount; i++)
        {
            FmdlBoneGroup fmdlBoneGroup = new FmdlBoneGroup();

            fmdlBoneGroup.unknown0 = reader.ReadUInt16();
            fmdlBoneGroup.boneIndexCount = reader.ReadUInt16();
            fmdlBoneGroup.boneIndices = new ushort[fmdlBoneGroup.boneIndexCount];

            for (int j = 0; j < fmdlBoneGroup.boneIndexCount; j++)
                fmdlBoneGroup.boneIndices[j] = reader.ReadUInt16();

            reader.BaseStream.Position += 0x40 - fmdlBoneGroup.boneIndexCount * 2;

            fmdlBoneGroups[i] = fmdlBoneGroup;
        } //for
    } //ReadBoneGroups

    private void ReadTextures(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[texturesIndex].offset;

        for (int i = 0; i < section0Infos[texturesIndex].entryCount; i++)
        {
            FmdlTexture fmdlTexture = new FmdlTexture();

            fmdlTexture.nameIndex = reader.ReadUInt16();
            fmdlTexture.pathIndex = reader.ReadUInt16();

            fmdlTextures[i] = fmdlTexture;
        } //for
    } //ReadTextures

    private void ReadMaterialParameters(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[materialParametersIndex].offset;

        for (int i = 0; i < section0Infos[materialParametersIndex].entryCount; i++)
        {
            FmdlMaterialParameter fmdlMaterialParameter = new FmdlMaterialParameter();

            fmdlMaterialParameter.nameIndex = reader.ReadUInt16();
            fmdlMaterialParameter.referenceIndex = reader.ReadUInt16();

            fmdlMaterialParameters[i] = fmdlMaterialParameter;
        } //for
    } //ReadMaterialParameters

    private void ReadMaterials(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[materialsIndex].offset;

        for (int i = 0; i < section0Infos[materialsIndex].entryCount; i++)
        {
            FmdlMaterial fmdlMaterial = new FmdlMaterial();

            fmdlMaterial.nameIndex = reader.ReadUInt16();
            fmdlMaterial.typeIndex = reader.ReadUInt16();

            fmdlMaterials[i] = fmdlMaterial;
        } //for
    } //ReadMaterials

    private void ReadMeshFormatInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[meshFormatInfoIndex].offset;

        for (int i = 0; i < section0Infos[meshFormatInfoIndex].entryCount; i++)
        {
            FmdlMeshFormatInfo fmdlMeshFormatInfo = new FmdlMeshFormatInfo();

            fmdlMeshFormatInfo.meshFormatCount = reader.ReadByte();
            fmdlMeshFormatInfo.vertexFormatCount = reader.ReadByte();
            fmdlMeshFormatInfo.unknown0 = reader.ReadUInt16();
            fmdlMeshFormatInfo.firstMeshFormatIndex = reader.ReadUInt16();
            fmdlMeshFormatInfo.firstVertexFormatIndex = reader.ReadUInt16();

            fmdlMeshFormatInfos[i] = fmdlMeshFormatInfo;
        } //for
    } //ReadMeshFormatInfo

    private void ReadMeshFormats(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[meshFormatsIndex].offset;

        for (int i = 0; i < section0Infos[meshFormatsIndex].entryCount; i++)
        {
            FmdlMeshFormat fmdlMeshFormat = new FmdlMeshFormat();

            fmdlMeshFormat.bufferOffsetIndex = reader.ReadByte();
            fmdlMeshFormat.vertexFormatCount = reader.ReadByte();
            fmdlMeshFormat.length = reader.ReadByte();
            fmdlMeshFormat.type = reader.ReadByte();
            fmdlMeshFormat.offset = reader.ReadUInt32();

            fmdlMeshFormats[i] = fmdlMeshFormat;
        } //for
    } //ReadMeshFormats

    private void ReadVertexFormats(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[vertexFormatsIndex].offset;

        for (int i = 0; i < section0Infos[vertexFormatsIndex].entryCount; i++)
        {
            FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();

            fmdlVertexFormat.type = reader.ReadByte();
            fmdlVertexFormat.dataType = reader.ReadByte();
            fmdlVertexFormat.offset = reader.ReadUInt16();

            fmdlVertexFormats[i] = fmdlVertexFormat;
        } //for
    } //ReadVertexFormats

    private void ReadStringInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[stringInfoIndex].offset;

        for (int i = 0; i < section0Infos[stringInfoIndex].entryCount; i++)
        {
            FmdlStringInfo fmdlStringInfo = new FmdlStringInfo();

            fmdlStringInfo.section1BlockIndex = reader.ReadUInt16();
            fmdlStringInfo.length = reader.ReadUInt16();
            fmdlStringInfo.offset = reader.ReadUInt32();

            fmdlStringInfos[i] = fmdlStringInfo;
        } //for
    } //ReadStringInfo

    private void ReadBoundingBoxes(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[boundingBoxesIndex].offset;

        for (int i = 0; i < section0Infos[boundingBoxesIndex].entryCount; i++)
        {
            FmdlBoundingBox fmdlBoundingBox = new FmdlBoundingBox();

            fmdlBoundingBox.max = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            fmdlBoundingBox.min = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            fmdlBoundingBoxes[i] = fmdlBoundingBox;
        } //for
    } //ReadBoundingBoxes

    private void ReadBufferOffsets(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[bufferOffsetsIndex].offset;

        for (int i = 0; i < section0Infos[bufferOffsetsIndex].entryCount; i++)
        {
            FmdlBufferOffset fmdlBufferOffset = new FmdlBufferOffset();

            fmdlBufferOffset.unknown0 = reader.ReadUInt32();
            fmdlBufferOffset.length = reader.ReadUInt32();
            fmdlBufferOffset.offset = reader.ReadUInt32();
            reader.BaseStream.Position += 4;

            fmdlBufferOffsets[i] = fmdlBufferOffset;
        } //for
    } //ReadBufferOffsets

    private void ReadLodInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[lodInfoIndex].offset;

        for (int i = 0; i < section0Infos[lodInfoIndex].entryCount; i++)
        {
            FmdlLodInfo fmdlLodInfo = new FmdlLodInfo();

            fmdlLodInfo.lodCount = reader.ReadUInt32();
            fmdlLodInfo.unknown0 = reader.ReadSingle();
            fmdlLodInfo.unknown1 = reader.ReadSingle();
            fmdlLodInfo.unknown2 = reader.ReadSingle();

            fmdlLodInfos[i] = fmdlLodInfo;
        } //for
    } //ReadLodInfo

    private void ReadFaceInfo(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[faceInfoIndex].offset;

        for (int i = 0; i < section0Infos[faceInfoIndex].entryCount; i++)
        {
            FmdlFaceInfo fmdlFaceInfo = new FmdlFaceInfo();

            fmdlFaceInfo.firstFaceVertexIndex = reader.ReadUInt32();
            fmdlFaceInfo.faceVertexCount = reader.ReadUInt32();

            fmdlFaceInfos[i] = fmdlFaceInfo;
        } //for
    } //ReadFaceInfo

    private void ReadType12(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[type12Index].offset;

        for (int i = 0; i < section0Infos[type12Index].entryCount; i++)
        {
            FmdlType12 fmdlType12 = new FmdlType12();

            fmdlType12.unknown0 = reader.ReadUInt64();

            fmdlType12s[i] = fmdlType12;
        } //for
    } //ReadType12s

    private void ReadType14(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[type12Index].offset;

        for (int i = 0; i < section0Infos[type14Index].entryCount; i++)
        {
            FmdlType14 fmdlType14 = new FmdlType14();

            reader.BaseStream.Position += 4;
            fmdlType14.unknown0 = reader.ReadSingle();
            fmdlType14.unknown1 = reader.ReadSingle();
            fmdlType14.unknown2 = reader.ReadSingle();
            fmdlType14.unknown3 = reader.ReadSingle();
            reader.BaseStream.Position += 8;
            fmdlType14.unknown4 = reader.ReadUInt32();
            fmdlType14.unknown5 = reader.ReadUInt32();
            reader.BaseStream.Position += 0x5C;

            fmdlType14s[i] = fmdlType14;
        } //for
    } //ReadType14

    private void ReadPathCode64s(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[pathCode64sIndex].offset;

        for (int i = 0; i < section0Infos[pathCode64sIndex].entryCount; i++)
            fmdlPathCode64s[i] = reader.ReadUInt64();
    } //ReadPathCode32s

    private void ReadStrCode64s(BinaryReader reader)
    {
        reader.BaseStream.Position = section0Offset + section0Infos[strCode64sIndex].offset;

        for (int i = 0; i < section0Infos[strCode64sIndex].entryCount; i++)
            fmdlStrCode64s[i] = reader.ReadUInt64();
    } //ReadPathCode32s

    private void ReadMaterialParameterFloats(BinaryReader reader)
    {
        reader.BaseStream.Position = section1Offset + section1Infos[materialParameterFloatsIndex].offset;

        for (int i = 0; i < fmdlMaterialParameterVectors.Length; i++)
        {
            Vector4 materialParameterVector = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            fmdlMaterialParameterVectors[i] = materialParameterVector;
        } //for
    } //ReadMaterialParameterFloats

    private void ReadBuffer(BinaryReader reader)
    {
        fmdlMeshes = new FmdlMesh[section0Infos[meshInfoIndex].entryCount];

        for (int i = 0; i < section0Infos[meshInfoIndex].entryCount; i++)
        {
            FmdlMesh fmdlMesh = new FmdlMesh();

            fmdlMesh.vertices = new Vector3[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.boneWeights = new Vector4[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.normals = new Vector4Half[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.colors = new Vector4[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.boneIndices = new Vector4[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.uv = new Vector2Half[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.uv2 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.uv3 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.uv4 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.unknownWeights = new Vector4[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.unknownIndices = new Vector4[fmdlMeshInfos[i].vertexCount];
            fmdlMesh.tangents = new Vector4Half[fmdlMeshInfos[i].vertexCount];

            //Go to the position of the first vertex.
            reader.BaseStream.Position = section1Offset + section1Infos[bufferIndex].offset + fmdlBufferOffsets[0].offset + fmdlMeshFormats[fmdlMeshFormatInfos[i].firstMeshFormatIndex].offset;

            for (int j = 0; j < fmdlMeshInfos[i].vertexCount; j++)
                fmdlMesh.vertices[j] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            reader.BaseStream.Position = section1Offset + section1Infos[bufferIndex].offset + fmdlBufferOffsets[1].offset + fmdlMeshFormats[fmdlMeshFormatInfos[i].firstMeshFormatIndex + 1].offset;

            for (int j = 0; j < fmdlMeshInfos[i].vertexCount; j++)
            {
                long position = reader.BaseStream.Position;

                for (int h = fmdlMeshFormatInfos[i].firstVertexFormatIndex; h < fmdlMeshFormatInfos[i].firstVertexFormatIndex + fmdlMeshFormatInfos[i].vertexFormatCount; h++)
                {
                    reader.BaseStream.Position = position + fmdlVertexFormats[h].offset;

                    switch (fmdlVertexFormats[h].type)
                    {
                        case 0:
                            break;
                        case 1:
                            fmdlMesh.boneWeights[j] = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            break;
                        case 2:
                            for (int k = 0; k < 4; k++)
                                fmdlMesh.normals[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                        case 3:
                            for (int k = 0; k < 4; k++)
                                fmdlMesh.colors[j][k] = reader.ReadByte();
                            break;
                        case 7:
                            fmdlMesh.boneIndices[j] = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            break;
                        case 8:
                            for (int k = 0; k < 2; k++)
                                fmdlMesh.uv[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                        case 9:
                            for (int k = 0; k < 2; k++)
                                fmdlMesh.uv2[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                        case 0xA:
                            for (int k = 0; k < 2; k++)
                                fmdlMesh.uv3[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                        case 0xB:
                            for (int k = 0; k < 2; k++)
                                fmdlMesh.uv4[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                        case 0xC:
                            fmdlMesh.unknownWeights[j] = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            break;
                        case 0xD:
                            fmdlMesh.unknownIndices[j] = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            break;
                        case 0xE:
                            for (int k = 0; k < 4; k++)
                                fmdlMesh.tangents[j][k] = Half.ToHalf(reader.ReadUInt16());
                            break;
                    } //switch
                } //for
            } //for

            reader.BaseStream.Position = section1Offset + section1Infos[bufferIndex].offset + fmdlBufferOffsets[2].offset + fmdlMeshInfos[i].firstFaceVertexIndex * 2;

            fmdlMesh.triangles = new ushort[fmdlMeshInfos[i].faceVertexCount];

            for (int j = 0; j < fmdlMeshInfos[i].faceVertexCount; j++)
            {
                fmdlMesh.triangles[j] = reader.ReadUInt16();
            } //for

            fmdlMeshes[i] = fmdlMesh;
        } //for
    } //ReadBuffer

    private void ReadStrings(BinaryReader reader)
    {
        fmdlStrings = new string[section0Infos[stringInfoIndex].entryCount];

        for (int i = 0; i < section0Infos[stringInfoIndex].entryCount; i++)
        {
            string s = "";

            for (int j = 0; j < fmdlStringInfos[i].length; j++)
                s += reader.ReadChar();

            fmdlStrings[i] = s;
        } //for
    } //ReadStrings
} //ExpFmdl