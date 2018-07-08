using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

            for (int j = fmdlMeshFormatInfos[i].firstVertexFormatIndex; j < fmdlMeshFormatInfos[i].firstVertexFormatIndex + fmdlMeshFormatInfos[i].vertexFormatCount; j++)
            {
                switch (fmdlVertexFormats[j].type)
                {
                    case 0:
                        fmdlMesh.vertices = new Vector3[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 1:
                        fmdlMesh.boneWeights = new Vector4[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 2:
                        fmdlMesh.normals = new Vector4Half[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 3:
                        fmdlMesh.colors = new Vector4[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 7:
                        fmdlMesh.boneIndices = new Vector4[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 8:
                        fmdlMesh.uv = new Vector2Half[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 9:
                        fmdlMesh.uv2 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 0xA:
                        fmdlMesh.uv3 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 0xB:
                        fmdlMesh.uv4 = new Vector2Half[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 0xC:
                        fmdlMesh.unknownWeights = new Vector4[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 0xD:
                        fmdlMesh.unknownIndices = new Vector4[fmdlMeshInfos[i].vertexCount];
                        break;
                    case 0xE:
                        fmdlMesh.tangents = new Vector4Half[fmdlMeshInfos[i].vertexCount];
                        break;
                } //switch
            } //for

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

            reader.BaseStream.Position = section1Offset + section1Infos[stringsIndex].offset + fmdlStringInfos[i].offset;

            for (int j = 0; j < fmdlStringInfos[i].length; j++)
                s += reader.ReadChar();

            fmdlStrings[i] = s;
        } //for
    } //ReadStrings

    public void Write(GameObject gameObject)
    {
        GetFmdlData(gameObject);
    } //Write

    private void GetFmdlData(GameObject gameObject)
    {
        FoxModel foxModel = gameObject.GetComponent<FoxModel>();
        Transform rootBone = gameObject.transform;
        List<Transform> bones = new List<Transform>(0);
        List<BoxCollider> boundingBoxes = new List<BoxCollider>(0);
        List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
        List<Material> materials = new List<Material>(0);
        List<string> materialTypeNames = new List<string>(0);
        List<Texture> textures = new List<Texture>(0);
        List<Vector4> materialParameterVectors = new List<Vector4>(0);
        List<string> strings = new List<string>(0);

        foreach (Transform t in gameObject.transform)
            if (t.gameObject.name == "[Root]")
            {
                rootBone = t;
                boundingBoxes.Add(t.gameObject.GetComponent<BoxCollider>());
                break;
            } //if

        if (rootBone == gameObject.transform)
            throw new Exception("[Root] not found!");

        GetBonesAndBoundingBoxes(rootBone, bones, boundingBoxes);
        GetMeshesMaterialsTexturesAndVectors(gameObject, meshes, materials, textures, materialParameterVectors);

        strings.Add("");

        //Bones
        int boneCount = bones.Count;

        fmdlBones = new FmdlBone[boneCount];

        for (int i = 0; i < boneCount; i++)
        {
            FmdlBone fmdlBone = new FmdlBone();
            Transform bone = bones[i];

            fmdlBone.nameIndex = (ushort)strings.Count;
            strings.Add(bones[i].gameObject.name);

            if (bone.parent == rootBone)
                fmdlBone.parentIndex = -1;
            else
                fmdlBone.parentIndex = (short)bones.IndexOf(bone.parent);

            fmdlBone.boundingBoxIndex = (ushort)boundingBoxes.IndexOf(bones[i].gameObject.GetComponent<BoxCollider>());
            fmdlBone.unknown0 = 1;
            fmdlBone.localPosition = bones[i].localPosition;
            fmdlBone.worldPosition = bones[i].position;

            fmdlBones[i] = fmdlBone;
        } //for

        //Mesh Groups
        int meshGroupCount = foxModel.meshGroups.Length;

        fmdlMeshGroups = new FmdlMeshGroup[meshGroupCount];

        for (int i = 0; i < meshGroupCount; i++)
        {
            FmdlMeshGroup fmdlMeshGroup = new FmdlMeshGroup();
            FoxMeshGroup foxMeshGroup = foxModel.meshGroups[i];

            fmdlMeshGroup.nameIndex = (ushort)strings.Count;
            fmdlMeshGroup.invisibilityFlag = foxMeshGroup.visible ? (ushort)0 : (ushort)1;
            fmdlMeshGroup.parentIndex = foxMeshGroup.parent;
            fmdlMeshGroup.unknown0 = -1;

            fmdlMeshGroups[i] = fmdlMeshGroup;
        } //for

        //Mesh Group Entries
        int meshCount = meshes.Count;
        List<FmdlMeshGroupEntry> meshGroupEntries = new List<FmdlMeshGroupEntry>(0);
        FmdlMeshGroupEntry fmdlMeshGroupEntry = new FmdlMeshGroupEntry();

        for (int i = 0; i < meshCount; i++)
        {
            if (i == 0)
            {
                fmdlMeshGroupEntry.meshGroupIndex = (ushort)foxModel.meshDefinitions[i].meshGroup;
                fmdlMeshGroupEntry.meshCount = 1;
                fmdlMeshGroupEntry.firstMeshIndex = 0;
                fmdlMeshGroupEntry.index = 0;
                fmdlMeshGroupEntry.firstFaceInfoIndex = 0;
            } //if
            else
            {
                if (foxModel.meshDefinitions[i].meshGroup == foxModel.meshDefinitions[i - 1].meshGroup)
                    fmdlMeshGroupEntry.meshCount += 1;
                else
                {
                    meshGroupEntries.Add(fmdlMeshGroupEntry);

                    fmdlMeshGroupEntry = new FmdlMeshGroupEntry();

                    fmdlMeshGroupEntry.meshGroupIndex = (ushort)foxModel.meshDefinitions[i].meshGroup;
                    fmdlMeshGroupEntry.meshCount = 1;
                    fmdlMeshGroupEntry.firstMeshIndex = (ushort)i;
                    fmdlMeshGroupEntry.index = (ushort)meshGroupEntries.Count;
                    fmdlMeshGroupEntry.firstFaceInfoIndex = (ushort)i;
                } //else
            } //else
        } //for

        fmdlMeshGroupEntries = meshGroupEntries.ToArray();

        //Mesh Info
        fmdlMeshInfos = new FmdlMeshInfo[meshCount];

        for (int i = 0; i < meshCount; i++)
        {
            FmdlMeshInfo fmdlMeshInfo = new FmdlMeshInfo();
            SkinnedMeshRenderer mesh = meshes[i];

            fmdlMeshInfo.alphaEnum = (byte)foxModel.meshDefinitions[i].alpha;
            fmdlMeshInfo.shadowEnum = (byte)foxModel.meshDefinitions[i].shadow;
            fmdlMeshInfo.materialInstanceIndex = (ushort)materials.IndexOf(mesh.sharedMaterial);
            fmdlMeshInfo.boneGroupIndex = (ushort)i;
            fmdlMeshInfo.index = (ushort)i;
            fmdlMeshInfo.vertexCount = (ushort)mesh.sharedMesh.vertices.Length;
            if (i == 0)
                fmdlMeshInfo.firstFaceVertexIndex = 0;
            else
                fmdlMeshInfo.firstFaceVertexIndex = fmdlMeshInfos[i - 1].firstFaceVertexIndex + fmdlMeshInfos[i - 1].faceVertexCount;
            fmdlMeshInfo.faceVertexCount = (uint)mesh.sharedMesh.triangles.Length;
            fmdlMeshInfo.firstFaceInfoIndex = (ulong)i;

            fmdlMeshInfos[i] = fmdlMeshInfo;
        } //for

        //Materials
        int materialInstanceCount = materials.Count;

        for (int i = 0; i < materialInstanceCount; i++)
        {
            string name = materials[i].shader.name;
            name = name.Substring(name.IndexOf('/') + 1);

            if (!materialTypeNames.Contains(name))
                materialTypeNames.Add(name);
        } //for

        int materialTypeNamesCount = materialTypeNames.Count;

        fmdlMaterials = new FmdlMaterial[materialTypeNamesCount];

        for (int i = 0; i < materialTypeNamesCount; i++)
        {
            FmdlMaterial fmdlMaterial = new FmdlMaterial();

            int currentStringCount = strings.Count;

            fmdlMaterial.nameIndex = (ushort)currentStringCount;
            fmdlMaterial.typeIndex = (ushort)currentStringCount;

            strings.Add(materialTypeNames[i]);

            fmdlMaterials[i] = fmdlMaterial;
        } //for

        //MaterialInstances
        fmdlMaterialInstances = new FmdlMaterialInstance[materialInstanceCount];

        for (int i = 0; i < materialInstanceCount; i++)
        {
            FmdlMaterialInstance fmdlMaterialInstance = new FmdlMaterialInstance();
            Material material = materials[i];
            Shader shader = material.shader;
            int materialTextureCount = 0;
            int materialParameterCount = 0;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);

            fmdlMaterialInstance.nameIndex = (ushort)strings.Count;
            strings.Add(materials[i].name);
            fmdlMaterialInstance.materialIndex = (ushort)materialTypeNames.IndexOf(shader.name.Substring(shader.name.IndexOf('/') + 1));

            for (int j = 0; j < propertyCount; j++)
            {
                if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
                    materialTextureCount++;
                else
                    materialParameterCount++;
            } //for

            fmdlMaterialInstance.textureCount = (byte)materialTextureCount;
            fmdlMaterialInstance.parameterCount = (byte)materialParameterCount;

            if (i == 0)
            {
                fmdlMaterialInstance.firstTextureIndex = 0;
                fmdlMaterialInstance.firstParameterIndex = fmdlMaterialInstance.textureCount;
            } //if
            else
            {
                if (fmdlMaterialInstances[i - 1].firstParameterIndex >= fmdlMaterialInstances[i - 1].firstTextureIndex && fmdlMaterialInstances[i - 1].parameterCount > 0)
                    fmdlMaterialInstance.firstTextureIndex = (ushort)(fmdlMaterialInstances[i - 1].firstParameterIndex + fmdlMaterialInstances[i - 1].parameterCount);
                else
                    fmdlMaterialInstance.firstTextureIndex = (ushort)(fmdlMaterialInstances[i - 1].firstTextureIndex + fmdlMaterialInstances[i - 1].textureCount);

                if (fmdlMaterialInstances[i - 1].firstParameterIndex + fmdlMaterialInstances[i - 1].parameterCount >= fmdlMaterialInstance.firstTextureIndex + fmdlMaterialInstance.textureCount)
                    fmdlMaterialInstance.firstParameterIndex = (ushort)(fmdlMaterialInstances[i - 1].firstParameterIndex + fmdlMaterialInstances[i - 1].parameterCount);
                else
                    fmdlMaterialInstance.firstParameterIndex = (ushort)(fmdlMaterialInstance.firstTextureIndex + fmdlMaterialInstance.textureCount);
            } //else

            fmdlMaterialInstances[i] = fmdlMaterialInstance;
        } //for

        //Bone Groups
        fmdlBoneGroups = new FmdlBoneGroup[meshCount];

        for(int i = 0; i < meshCount; i++)
        {
            FmdlBoneGroup fmdlBoneGroup = new FmdlBoneGroup();
            int meshBoneCount = meshes[i].bones.Length;

            fmdlBoneGroup.unknown0 = 4;
            fmdlBoneGroup.boneIndexCount = (ushort)meshBoneCount;
            fmdlBoneGroup.boneIndices = new ushort[meshBoneCount];

            for(int j = 0; j < meshBoneCount; j++)
                fmdlBoneGroup.boneIndices[j] = (ushort)bones.IndexOf(meshes[i].bones[j]);

            fmdlBoneGroups[i] = fmdlBoneGroup;
        } //for

        //Textures
        int textureCount = textures.Count;

        fmdlTextures = new FmdlTexture[textureCount];

        for(int i = 0; i < textureCount; i++)
        {
            FmdlTexture fmdlTexture = new FmdlTexture();
            Texture texture = textures[i];
            string assetPath = AssetDatabase.GetAssetPath(texture);

            if (assetPath == "" || assetPath.Contains(".fmdl"))
                assetPath = texture.name.Substring(1);

            string textureName = Path.GetFileNameWithoutExtension(assetPath) + ".tga";
            string texturePath = $"/{Path.GetDirectoryName(assetPath)}/";

            fmdlTexture.nameIndex = (ushort)strings.Count;
            strings.Add(textureName);

            if (!strings.Contains(texturePath))
            {
                fmdlTexture.pathIndex = (ushort)strings.Count;
                strings.Add(texturePath);
            } //if
            else
                fmdlTexture.pathIndex = (ushort)strings.IndexOf(texturePath);

            fmdlTextures[i] = fmdlTexture;
        } //for

        //Material Parameters
        List<FmdlMaterialParameter> materialParameters = new List<FmdlMaterialParameter>(0);

        for(int i = 0; i < materialInstanceCount; i++)
        {
            Material material = materials[i];
            Shader shader = material.shader;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);

            for(int j = 0; j < propertyCount; j++)
            {
                FmdlMaterialParameter fmdlMaterialParameter = new FmdlMaterialParameter();

                if(ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, j);

                    if (!strings.Contains(propertyName))
                    {
                        fmdlMaterialParameter.nameIndex = (ushort)strings.Count;
                        strings.Add(propertyName);
                    } //if
                    else
                        fmdlMaterialParameter.nameIndex = (ushort)strings.IndexOf(propertyName);

                    fmdlMaterialParameter.referenceIndex = (ushort)textures.IndexOf(material.GetTexture(propertyName));
                } //if

                materialParameters.Add(fmdlMaterialParameter);
            } //for

            for (int j = 0; j < propertyCount; j++)
            {
                FmdlMaterialParameter fmdlMaterialParameter = new FmdlMaterialParameter();

                if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.Vector)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, j);
                    Vector4 vector = material.GetVector(propertyName);

                    if (!strings.Contains(propertyName))
                    {
                        fmdlMaterialParameter.nameIndex = (ushort)strings.Count;
                        strings.Add(propertyName);
                    } //if
                    else
                        fmdlMaterialParameter.nameIndex = (ushort)strings.IndexOf(propertyName);

                    fmdlMaterialParameter.referenceIndex = (ushort)materialParameterVectors.IndexOfEqualValue(material.GetVector(propertyName));
                } //if

                materialParameters.Add(fmdlMaterialParameter);
            } //for
        } //for

        fmdlMaterialParameters = materialParameters.ToArray();

        //Mesh Format Info
        List<FmdlVertexFormat> vertexFormats = new List<FmdlVertexFormat>(0);
        List<FmdlMeshFormat> meshFormats = new List<FmdlMeshFormat>(0);
        uint positionOffset = 0;
        uint meshOffset = 0;

        fmdlMeshFormatInfos = new FmdlMeshFormatInfo[meshCount];

        for (int i = 0; i < meshCount; i++)
        {
            FmdlMeshFormatInfo fmdlMeshFormatInfo = new FmdlMeshFormatInfo();
            fmdlMeshFormatInfo.meshFormatCount = 0;
            fmdlMeshFormatInfo.vertexFormatCount = 0;
            fmdlMeshFormatInfo.unknown0 = 0x100;
            if(i == 0)
            {
                fmdlMeshFormatInfo.firstMeshFormatIndex = 0;
                fmdlMeshFormatInfo.firstVertexFormatIndex = 0;
            } //if
            else
            {
                fmdlMeshFormatInfo.firstMeshFormatIndex = (ushort)(fmdlMeshFormatInfos[i - 1].firstMeshFormatIndex + fmdlMeshFormatInfos[i - 1].meshFormatCount);
                fmdlMeshFormatInfo.firstVertexFormatIndex = (ushort)(fmdlMeshFormatInfos[i - 1].firstVertexFormatIndex + fmdlMeshFormatInfos[i - 1].vertexFormatCount);
            } //else

            Mesh mesh = meshes[i].sharedMesh;
            FmdlMeshFormat fmdlMeshFormat2 = new FmdlMeshFormat();
            fmdlMeshFormat2.bufferOffsetIndex = 1;
            fmdlMeshFormat2.vertexFormatCount = 0;
            fmdlMeshFormat2.length = 0;
            fmdlMeshFormat2.type = 1;
            fmdlMeshFormat2.offset = meshOffset;
            FmdlMeshFormat fmdlMeshFormat3 = new FmdlMeshFormat();
            fmdlMeshFormat3.bufferOffsetIndex = 1;
            fmdlMeshFormat3.vertexFormatCount = 0;
            fmdlMeshFormat3.length = 0;
            fmdlMeshFormat3.type = 2;
            fmdlMeshFormat3.offset = meshOffset;
            FmdlMeshFormat fmdlMeshFormat4 = new FmdlMeshFormat();
            fmdlMeshFormat4.bufferOffsetIndex = 1;
            fmdlMeshFormat4.vertexFormatCount = 0;
            fmdlMeshFormat4.length = 0;
            fmdlMeshFormat4.type = 2;
            fmdlMeshFormat4.offset = meshOffset;
            ushort vertexOffset = 0;

            if(mesh.vertices.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0;
                fmdlVertexFormat.dataType = 1;
                fmdlVertexFormat.offset = 0;

                vertexFormats.Add(fmdlVertexFormat);

                FmdlMeshFormat fmdlMeshFormat = new FmdlMeshFormat();
                fmdlMeshFormat.bufferOffsetIndex = 0;
                fmdlMeshFormat.vertexFormatCount = 1;
                fmdlMeshFormat.length = 0xC;
                fmdlMeshFormat.type = 0;
                fmdlMeshFormat.offset = positionOffset;

                meshFormats.Add(fmdlMeshFormat);

                fmdlMeshFormatInfo.meshFormatCount++;
                fmdlMeshFormatInfo.vertexFormatCount++;

                positionOffset += (uint)(0xC * mesh.vertices.Length);
            } //if

            if (mesh.normals.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 2;
                fmdlVertexFormat.dataType = 6;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 8;

                fmdlMeshFormat2.vertexFormatCount++;
                fmdlMeshFormat2.length += 8;
            } //if

            if (mesh.tangents.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0xE;
                fmdlVertexFormat.dataType = 6;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 8;

                fmdlMeshFormat2.vertexFormatCount++;
                fmdlMeshFormat2.length += 8;
            } //if

            if (mesh.colors.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 3;
                fmdlVertexFormat.dataType = 8;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlMeshFormat3.vertexFormatCount++;
                fmdlMeshFormat3.length += 4;
            } //if

            if (mesh.boneWeights.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 1;
                fmdlVertexFormat.dataType = 8;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 7;
                fmdlVertexFormat.dataType = 9;
                fmdlVertexFormat.offset = vertexOffset;
                vertexFormats.Add(fmdlVertexFormat);
                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount += 2;
                fmdlMeshFormat4.length += 8;
            } //if

            if (mesh.uv.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 8;
                fmdlVertexFormat.dataType = 7;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount++;
                fmdlMeshFormat4.length += 4;
            } //if

            if (mesh.uv2.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 9;
                fmdlVertexFormat.dataType = 7;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount++;
                fmdlMeshFormat4.length += 4;
            } //if

            if (mesh.uv3.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0xA;
                fmdlVertexFormat.dataType = 7;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount++;
                fmdlMeshFormat4.length += 4;
            } //if

            if (mesh.uv4.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0xB;
                fmdlVertexFormat.dataType = 7;
                fmdlVertexFormat.offset = vertexOffset;

                vertexFormats.Add(fmdlVertexFormat);

                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount++;
                fmdlMeshFormat4.length += 4;
            } //if

            //Can't implement this. Unity doesn't support it.
            /*if (mesh.boneWeights2.Length > 0)
            {
                FmdlVertexFormat fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0xC;
                fmdlVertexFormat.dataType = 8;
                fmdlVertexFormat.offset = vertexOffset;
                vertexFormats.Add(fmdlVertexFormat);
                vertexOffset += 4;

                fmdlVertexFormat = new FmdlVertexFormat();
                fmdlVertexFormat.type = 0xD;
                fmdlVertexFormat.dataType = 4;
                fmdlVertexFormat.offset = vertexOffset;
                vertexFormats.Add(fmdlVertexFormat);
                vertexOffset += 4;

                fmdlMeshFormat4.vertexFormatCount += 2;
                fmdlMeshFormat4.length = 8;
            } //if
            */

            fmdlMeshFormatInfo.vertexFormatCount += (byte)(fmdlMeshFormat2.vertexFormatCount + fmdlMeshFormat3.vertexFormatCount + fmdlMeshFormat4.vertexFormatCount);

            meshOffset += (uint)(mesh.vertices.Length * fmdlMeshFormatInfo.vertexFormatCount);

            fmdlMeshFormat2.offset = meshOffset;
            fmdlMeshFormat3.offset = meshOffset;
            fmdlMeshFormat4.offset = meshOffset;

            meshFormats.Add(fmdlMeshFormat2);
            if (fmdlMeshFormat3.length > 0)
            {
                meshFormats.Add(fmdlMeshFormat3);
                fmdlMeshFormatInfo.meshFormatCount++;
            } //if
            meshFormats.Add(fmdlMeshFormat4);

            fmdlMeshFormatInfo.meshFormatCount += 2;

            fmdlMeshFormatInfos[i] = fmdlMeshFormatInfo;
        } //for

        fmdlMeshFormats = meshFormats.ToArray();
        fmdlVertexFormats = vertexFormats.ToArray();

        //String Info
        int stringCount = strings.Count;

        fmdlStringInfos = new FmdlStringInfo[stringCount];

        for(int i = 0; i < stringCount; i++)
        {
            FmdlStringInfo fmdlStringInfo = new FmdlStringInfo();
            string fmdlString = strings[i];

            fmdlStringInfo.section1BlockIndex = 3;
            fmdlStringInfo.length = (ushort)fmdlString.Length;
            if (i == 0)
                fmdlStringInfo.offset = 0;
            else
                fmdlStringInfo.offset = fmdlStringInfos[i - 1].offset + fmdlStringInfos[i - 1].length + 1;

            fmdlStringInfos[i] = fmdlStringInfo;
        } //for

        //Bounding Boxes
        int boundingBoxCount = boundingBoxes.Count;

        fmdlBoundingBoxes = new FmdlBoundingBox[boundingBoxCount];

        for (int i = 0; i < boundingBoxCount; i++)
        {
            FmdlBoundingBox fmdlBoundingBox = new FmdlBoundingBox();
            BoxCollider boundingBox = boundingBoxes[i];

            fmdlBoundingBox.max = new Vector4(-boundingBox.bounds.max.x, boundingBox.bounds.max.y, boundingBox.bounds.max.z, 1f);
            fmdlBoundingBox.min = new Vector4(-boundingBox.bounds.min.x, boundingBox.bounds.min.y, boundingBox.bounds.min.z, 1f);

            fmdlBoundingBoxes[i] = fmdlBoundingBox;
        } //for

        //Lod Info
        fmdlLodInfos = new FmdlLodInfo[1];

        FmdlLodInfo fmdlLodInfo = new FmdlLodInfo();

        fmdlLodInfo.lodCount = 1;
        fmdlLodInfo.unknown0 = 1f;
        fmdlLodInfo.unknown1 = 1f;
        fmdlLodInfo.unknown2 = 1f;

        fmdlLodInfos[0] = fmdlLodInfo;

        //Face Info
        fmdlFaceInfos = new FmdlFaceInfo[meshCount];

        for(int i = 0; i < meshCount; i++)
        {
            FmdlFaceInfo fmdlFaceInfo = new FmdlFaceInfo();

            if (i == 0)
                fmdlFaceInfo.firstFaceVertexIndex = 0;
            else
                fmdlFaceInfo.firstFaceVertexIndex = fmdlFaceInfos[i - 1].firstFaceVertexIndex + fmdlFaceInfos[i - 1].faceVertexCount;

            fmdlFaceInfo.faceVertexCount = fmdlMeshInfos[i].faceVertexCount;
        } //for

        //Type 12
        fmdlType12s = new FmdlType12[1];

        FmdlType12 fmdlType12 = new FmdlType12();

        fmdlType12.unknown0 = 0;

        fmdlType12s[0] = fmdlType12;

        //Type 14
        fmdlType14s = new FmdlType14[1];

        FmdlType14 fmdlType14 = new FmdlType14();

        fmdlType14.unknown0 = 3.33850384f;
        fmdlType14.unknown1 = 0.8753322f;
        fmdlType14.unknown2 = 0.200000048f;
        fmdlType14.unknown3 = 5f;
        fmdlType14.unknown4 = 5;
        fmdlType14.unknown5 = 1;

        fmdlType14s[0] = fmdlType14;

        //Material Parameter Vectors
        fmdlMaterialParameterVectors = materialParameterVectors.ToArray();

        //Meshes
        fmdlMeshes = new FmdlMesh[meshCount];

        for(int i = 0; i < meshCount; i++)
        {
            FmdlMesh fmdlMesh = new FmdlMesh();
            Mesh mesh = meshes[i].sharedMesh;
            int vertexCount = mesh.vertices.Length;
            int triangleCount = mesh.triangles.Length;

            fmdlMesh.vertices = new Vector3[vertexCount];
            fmdlMesh.normals = new Vector4Half[vertexCount];
            fmdlMesh.tangents = new Vector4Half[vertexCount];
            fmdlMesh.colors = mesh.colors.Length > 0 ? new Vector4[vertexCount] : new Vector4[0];
            fmdlMesh.boneWeights = mesh.boneWeights.Length > 0 ? new Vector4[vertexCount] : new Vector4[0];
            fmdlMesh.boneIndices = mesh.boneWeights.Length > 0 ? new Vector4[vertexCount] : new Vector4[0];
            fmdlMesh.uv = new Vector2Half[vertexCount];
            fmdlMesh.uv2 = mesh.uv2.Length > 0 ? new Vector2Half[vertexCount] : new Vector2Half[0];
            fmdlMesh.uv3 = mesh.uv4.Length > 0 ? new Vector2Half[vertexCount] : new Vector2Half[0];
            fmdlMesh.uv4 = mesh.uv4.Length > 0 ? new Vector2Half[vertexCount] : new Vector2Half[0];
            fmdlMesh.triangles = new ushort[triangleCount];

            for (int j = 0; j < vertexCount; j++)
            {
                fmdlMesh.vertices[j] = new Vector3(-mesh.vertices[j].x, mesh.vertices[j].y, mesh.vertices[j].z);
                fmdlMesh.normals[j] = new Vector4Half(new Half(-mesh.normals[j].x), new Half(mesh.normals[j].y), new Half(mesh.normals[j].z), new Half(1.0f));
                fmdlMesh.tangents[j] = new Vector4Half(new Half(-mesh.tangents[j].x), new Half(mesh.tangents[j].y), new Half(mesh.tangents[j].z), new Half(mesh.tangents[j].w));
                if (mesh.colors.Length > 0)
                    fmdlMesh.colors[j] = new Vector4(mesh.colors[j].r, mesh.colors[j].g, mesh.colors[j].b, mesh.colors[j].a);
                if(mesh.boneWeights.Length > 0)
                {
                    fmdlMesh.boneWeights[j] = new Vector4(mesh.boneWeights[j].weight0, mesh.boneWeights[j].weight1, mesh.boneWeights[j].weight2, mesh.boneWeights[j].weight3);
                    fmdlMesh.boneIndices[j] = new Vector4(mesh.boneWeights[j].boneIndex0, mesh.boneWeights[j].boneIndex1, mesh.boneWeights[j].boneIndex2, mesh.boneWeights[j].boneIndex3);
                } //if
                fmdlMesh.uv[j] = new Vector2Half(new Half(mesh.uv[j].x), new Half(-mesh.uv[j].y));
                if (mesh.uv2.Length > 0)
                {
                    fmdlMesh.uv2[j] = new Vector2Half(new Half(mesh.uv2[j].x), new Half(-mesh.uv2[j].y));

                    if (mesh.uv3.Length > 0)
                    {
                        fmdlMesh.uv3[j] = new Vector2Half(new Half(mesh.uv3[j].x), new Half(-mesh.uv3[j].y));

                        if (mesh.uv4.Length > 0)
                        {
                            fmdlMesh.uv4[j] = new Vector2Half(new Half(mesh.uv4[j].x), new Half(-mesh.uv4[j].y));
                        } //if
                    } //if
                } //if
            } //for

            for(int j = 0; j < triangleCount; j++)
                fmdlMesh.triangles[j] = (ushort)mesh.triangles[j];
        } //for

        //Strings
        fmdlStrings = strings.ToArray();
    } //GetFmdlData

    private void GetBonesAndBoundingBoxes(Transform transform, List<Transform> bones, List<BoxCollider> boundingBoxes)
    {
        foreach (Transform t in transform)
        {
            bones.Add(t);
            boundingBoxes.Add(t.gameObject.GetComponent<BoxCollider>());
            GetBonesAndBoundingBoxes(t, bones, boundingBoxes);
        } //foreach
    } //GetBones

    private void GetMeshesMaterialsTexturesAndVectors(GameObject gameObject, List<SkinnedMeshRenderer> meshes, List<Material> materials, List<Texture> textures, List<Vector4> vectors)
    {
        foreach (Transform t in gameObject.transform)
        {
            if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer skinnedMeshRenderer = t.gameObject.GetComponent<SkinnedMeshRenderer>();
                Material material = skinnedMeshRenderer.sharedMaterial;

                meshes.Add(skinnedMeshRenderer);

                if (!materials.Contains(material))
                {
                    materials.Add(material);
                    Shader shader = material.shader;
                    int propertyCount = ShaderUtil.GetPropertyCount(shader);

                    for (int i = 0; i < propertyCount; i++)
                        if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            Texture texture = material.GetTexture(ShaderUtil.GetPropertyName(shader, i));

                            if (!textures.Contains(texture))
                                textures.Add(texture);
                        } //if
                        else if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Vector)
                        {
                            Vector4 vector = material.GetVector(ShaderUtil.GetPropertyName(shader, i));

                            if (!vectors.ContainsEqualValue(vector))
                                vectors.Add(vector);
                        } //else
                } //if
            } //if
        } //foreach
    } //GetMeshesMaterialsAndTextures
} //ExpFmdl