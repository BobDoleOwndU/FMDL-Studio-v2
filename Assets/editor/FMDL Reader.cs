using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Half;
using UnityEngine;

namespace FmdlTool
{
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
            public float x0;
            public float y0;
            public float z0;
            public float w0;
            public float x1;
            public float y1;
            public float z1;
            public float w1;
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
            public uint faceOffset;
            public uint numFaceVertices;
            public ulong unknown3; //probably related to section 0xA or 0x11
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
            //TBD....
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

        private struct Object
        {
            public Vertex[] vertices;
            public Face[] faces;
        } //struct

        private struct VBuffer //Not actually the vbuffer, the entire section containing the vertex positions, add. vertex data and faces all make up the VBuffer.
        {
            public Half nX;
            public Half nY;
            public Half nZ;
            public Half nW;

            public Half unknownFloat0; //
            public Half unknownFloat1; // I think these are the bone weight floats but I can't remember.
            public Half unknownFloat2; // Due to the fact I can't remember these are just going to be unknowns right now.
            public Half unknownFloat3; //

            public uint floatDivisor; //I am pretty sure this is what this does. Working without documentation is difficult.
            public uint unknown5;

            public Half uvX;
            public Half uvY;
        } //struct

        private struct Face
        {
            public ushort v1;
            public ushort v2;
            public ushort v3;
        } //struct

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
        private VBuffer[] vbuffer;
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
                section0Block0Entries[i].x0 = reader.ReadSingle();
                section0Block0Entries[i].y0 = reader.ReadSingle();
                section0Block0Entries[i].z0 = reader.ReadSingle();
                section0Block0Entries[i].w0 = reader.ReadSingle();
                section0Block0Entries[i].x1 = reader.ReadSingle();
                section0Block0Entries[i].y1 = reader.ReadSingle();
                section0Block0Entries[i].z1 = reader.ReadSingle();
                section0Block0Entries[i].w1 = reader.ReadSingle();
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
            reader.BaseStream.Position = section0Info[3].offset + section0Offset;

            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                section0Block3Entries[i].unknown0 = reader.ReadUInt32();
                section0Block3Entries[i].unknown1 = reader.ReadUInt16();
                section0Block3Entries[i].boneGroupId = reader.ReadUInt16();
                section0Block3Entries[i].id = reader.ReadUInt16();
                section0Block3Entries[i].numVertices = reader.ReadUInt16();
                reader.BaseStream.Position += 0x4;
                section0Block3Entries[i].faceOffset = reader.ReadUInt32();
                section0Block3Entries[i].numFaceVertices = reader.ReadUInt32();
                section0Block3Entries[1].unknown3 = reader.ReadUInt64();
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
                //...TBD
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
            //go to and get the section 0xD entry info.
            reader.BaseStream.Position = section0Info[13].offset + section0Offset;

            /*for (int i = 0; i < section0BlockDEntries.Length; i++)
            {
                section0BlockEEntries[i].unknown0 = reader.ReadUInt32();
                section0BlockEEntries[i].length = reader.ReadUInt32();
                section0BlockEEntries[i].offset = reader.ReadUInt32();
                reader.BaseStream.Position += 0x4;
            } //for */

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
             * VERTEX DATA
             *
             ****************************************************************/
            reader.BaseStream.Position = section1Info[1].offset + section1Offset;

            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                objects[i].vertices = new Vertex[section0Block3Entries[i].numVertices];

                for (int j = 0; j < section0Block3Entries[i].numVertices; j++)
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
             * FACES DATA
             *
             ****************************************************************/
            //Need offset code before the face loop can read its data.
            /// <example>
            /// reader.BaseStream.Position = section1Info[1].offset + section1Offset;
            /// </example>

            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                objects[i].faces = new Face[section0Block3Entries[i].numFaceVertices / 3];

                for(int j = 0; j < section0Block3Entries[i].numFaceVertices / 3; j++)
                {
                    objects[i].faces[j].v1 = reader.ReadUInt16();
                    objects[i].faces[j].v2 = reader.ReadUInt16();
                    objects[i].faces[j].v3 = reader.ReadUInt16();
                } //forSection0Block3Entry
            } //for
                

            /****************************************************************
             *
             * VERTEX BUFFER, KINDA
             *
             ****************************************************************/
            /*reader.BaseStream.Position = section0BlockEEntries[1].offset + section1Offset + section1Info[1].offset;

            for (int i = 0; i < section0BlockEEntries[1].length; i++) //This .length thing won't actually work but I am going to leave it for now because we don't actually know how much padding and how it is formatted right now.
            {
                vbuffer[i].nX = reader.ReadHalf();
                vbuffer[i].nY = reader.ReadHalf();
                vbuffer[i].nZ = reader.ReadHalf();
                vbuffer[i].nW = reader.ReadHalf();

                vbuffer[i].unknownFloat0 = reader.ReadHalf();
                vbuffer[i].unknownFloat1 = reader.ReadHalf();
                vbuffer[i].unknownFloat2 = reader.ReadHalf();
                vbuffer[i].unknownFloat3 = reader.ReadHalf();

                vbuffer[i].floatDivisor = reader.ReadUInt32();
                vbuffer[i].unknown5 = reader.ReadUInt32();

                vbuffer[i].uvX = reader.ReadHalf();
                vbuffer[i].uvY = reader.ReadHalf();

                vbuffer[i].unknownFloat0 = vbuffer[i].unknownFloat0 / vbuffer[i].floatDivisor;
                vbuffer[i].unknownFloat1 = vbuffer[i].unknownFloat1 / vbuffer[i].floatDivisor;
                vbuffer[i].unknownFloat2 = vbuffer[i].unknownFloat2 / vbuffer[i].floatDivisor;
                vbuffer[i].unknownFloat3 = vbuffer[i].unknownFloat3 / vbuffer[i].floatDivisor;
            } //for */
        } //Read

        public void MeshReader()
        {
            Vector3[] unityVertices;
            int[] unityFaces;
            HalfVector4[] unityNormals;
            HalfVector2[] unityUVs;
            HalfVector4[] unityBoneWeights;

            MeshFilter meshFilter = new MeshFilter();
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            //Vertices
            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                unityVertices = new Vector3[section0Block3Entries[i].numVertices];

                for (int j = 0; j < section0Block3Entries[i].numVertices; j++)
                {
                    unityVertices[i] = new Vector3(objects[i].vertices[j].x, objects[i].vertices[j].y, objects[i].vertices[j].z);
                } //for
            } //for

            //Faces
            for (int i = 0; i < section0Block3Entries.Length; i++)
            {
                unityFaces = new int[section0Block3Entries[i].numFaceVertices];

                for (int j = 0; j < section0Block3Entries[i].numFaceVertices; j++)
                {
                    unityFaces[i] = /*objects[i].faces[j].v1 and then objects[i].faces[j].v2 and then objects[i].faces[j].v3*/;
                } //for
            } //for

            //Normals

            //UVs

            //BoneWeights

            //Bones

            /// <summary>
            /// This is what takes the arrays and assigns them to the mesh class for use in Unity.
            /// </summary>
            mesh.vertices = unityVertices;
            mesh.triangles = unityFaces;
            mesh.normals = unityNormals;
            mesh.uv = unityUVs;
            mesh.boneWeights = unityBoneWeights; //I think this one can probably be done easier. Also, when bone weights are implemented, "MeshRenderer" in the GameObjOpener class will need to be changed to "SkinnedMeshRenderer"
        } //MeshReader
    } //class
} //namespace