using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using Xml2CSharp;

namespace Assets.Fmdl_Studio.Scripts.Experimental
{
    public static class COLLADAConverter
    {
        public static void ConvertToCOLLADA(GameObject gameObject, string filepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(COLLADA));
            //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            //namespaces.Add("xmlns", "http://www.collada.org/2008/03/COLLADASchema");

            List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
            GetMeshes(gameObject.transform, meshes);

            COLLADA collada = new COLLADA();
            collada.Version = "1.5.0";

            //Asset
            collada.Asset = new Asset();
            collada.Asset.Contributor = new Contributor();
            collada.Asset.Contributor.Authoring_tool = "Fmdl Studio v2";
            collada.Asset.Created = System.DateTime.Now.ToString();
            collada.Asset.Modified = System.DateTime.Now.ToString();
            collada.Asset.Unit = new Unit();
            collada.Asset.Unit.Meter = "1";
            collada.Asset.Unit.Name = "meter";
            collada.Asset.Up_axis = "Y_UP";

            //Library Geometries
            collada.Library_geometries = new Library_geometries();
            collada.Library_geometries.Geometry = new List<Geometry>(0);

            foreach (SkinnedMeshRenderer m in meshes)
            {
                Geometry geometry = new Geometry();
                geometry.Id = m.name;
                geometry.Name = m.name;
                geometry.Mesh = new Xml2CSharp.Mesh();
                geometry.Mesh.Source = new List<Source>(0);

                //Positions
                Source positions = new Source();
                positions.Id = $"{m.name}_Positions";
                positions.Float_array = new Float_array();
                positions.Float_array.Id = $"{m.name}_PosArr";
                positions.Float_array.Count = (m.sharedMesh.vertexCount * 3).ToString();

                StringBuilder posArr = new StringBuilder();

                foreach(Vector3 v in m.sharedMesh.vertices)
                {
                    posArr.Append($"{v.x} {v.y} {v.z} ");
                } //foreach

                posArr.Length--;

                positions.Float_array.Text = posArr.ToString();
                positions.Technique_common = new Technique_common();
                positions.Technique_common.Accessor = new Accessor();
                positions.Technique_common.Accessor.Source = $"#{positions.Float_array.Id}";
                positions.Technique_common.Accessor.Count = m.sharedMesh.vertexCount.ToString();
                positions.Technique_common.Accessor.Stride = "3";

                Param x = new Param();
                Param y = new Param();
                Param z = new Param();
                x.Name = "X";
                y.Name = "Y";
                z.Name = "Z";
                x.Type = "float";
                y.Type = "float";
                z.Type = "float";
                positions.Technique_common.Accessor.Param = new List<Param>(0);
                positions.Technique_common.Accessor.Param.Add(x);
                positions.Technique_common.Accessor.Param.Add(y);
                positions.Technique_common.Accessor.Param.Add(z);

                geometry.Mesh.Source.Add(positions);

                //Normals
                Source normals = new Source();
                normals.Id = $"{m.name}_Normals";
                normals.Float_array = new Float_array();
                normals.Float_array.Id = $"{m.name}_NormArr";
                normals.Float_array.Count = (m.sharedMesh.normals.Length * 3).ToString();

                StringBuilder normArr = new StringBuilder();

                foreach (Vector3 v in m.sharedMesh.normals)
                {
                    normArr.Append($"{v.x} {v.y} {v.z} ");
                } //foreach

                normArr.Length--;

                normals.Float_array.Text = posArr.ToString();
                normals.Technique_common = new Technique_common();
                normals.Technique_common.Accessor = new Accessor();
                normals.Technique_common.Accessor.Source = $"#{normals.Float_array.Id}";
                normals.Technique_common.Accessor.Count = m.sharedMesh.normals.Length.ToString();
                normals.Technique_common.Accessor.Stride = "3";

                normals.Technique_common.Accessor.Param = new List<Param>(0);
                normals.Technique_common.Accessor.Param.Add(x);
                normals.Technique_common.Accessor.Param.Add(y);
                normals.Technique_common.Accessor.Param.Add(z);

                geometry.Mesh.Source.Add(normals);

                //UVs0
                Source uvs0 = new Source();
                uvs0.Id = $"{m.name}_UVs0";
                uvs0.Float_array = new Float_array();
                uvs0.Float_array.Id = $"{m.name}_UVArr0";
                uvs0.Float_array.Count = (m.sharedMesh.uv.Length * 2).ToString();

                StringBuilder uvArr0 = new StringBuilder();

                foreach (Vector2 v in m.sharedMesh.uv)
                {
                    uvArr0.Append($"{v.x} {v.y} ");
                } //foreach

                uvArr0.Length--;

                uvs0.Float_array.Text = uvArr0.ToString();
                uvs0.Technique_common = new Technique_common();
                uvs0.Technique_common.Accessor = new Accessor();
                uvs0.Technique_common.Accessor.Source = $"#{uvs0.Float_array.Id}";
                uvs0.Technique_common.Accessor.Count = m.sharedMesh.uv.Length.ToString();
                uvs0.Technique_common.Accessor.Stride = "2";

                Param s = new Param();
                Param t = new Param();
                s.Name = "S";
                t.Name = "T";
                s.Type = "float";
                t.Type = "float";
                uvs0.Technique_common.Accessor.Param = new List<Param>(0);
                uvs0.Technique_common.Accessor.Param.Add(s);
                uvs0.Technique_common.Accessor.Param.Add(t);

                geometry.Mesh.Source.Add(uvs0);

                //Vertices
                geometry.Mesh.Vertices = new Vertices();
                geometry.Mesh.Vertices.Id = $"{m.name}_Vertices";
                geometry.Mesh.Vertices.Input = new Xml2CSharp.Input();
                geometry.Mesh.Vertices.Input.Semantic = "POSITION";
                geometry.Mesh.Vertices.Input.Source = $"#{positions.Id}";

                //Triangles
                geometry.Mesh.Triangles = new Triangles();
                geometry.Mesh.Triangles.Material = ""; //FIX LATER
                geometry.Mesh.Triangles.Count = m.sharedMesh.triangles.Length.ToString();

                geometry.Mesh.Triangles.Input = new List<Xml2CSharp.Input>(0);

                Xml2CSharp.Input vertex = new Xml2CSharp.Input();
                vertex.Semantic = "VERTEX";
                vertex.Source = $"#{geometry.Mesh.Vertices.Id}";
                vertex.Offset = "0";
                geometry.Mesh.Triangles.Input.Add(vertex);

                /*Xml2CSharp.Input normal = new Xml2CSharp.Input();
                normal.Semantic = "NORMAL";
                normal.Source = $"#{normals.Id}";
                normal.Offset = "1";
                geometry.Mesh.Triangles.Input.Add(normal);

                Xml2CSharp.Input texcoord = new Xml2CSharp.Input();
                texcoord.Semantic = "TEXCOORD";
                texcoord.Source = $"#{uvs0.Id}";
                texcoord.Set = "0";
                texcoord.Offset = "2";
                geometry.Mesh.Triangles.Input.Add(texcoord);*/

                StringBuilder trianglesArr = new StringBuilder();

                foreach(int i in m.sharedMesh.triangles)
                {
                    trianglesArr.Append($"{i} ");
                } //foreach

                trianglesArr.Length--;

                geometry.Mesh.Triangles.P = trianglesArr.ToString();

                collada.Library_geometries.Geometry.Add(geometry);
            } //foreach

            //Library Controllers
            collada.Library_controllers = new Library_controllers();
            collada.Library_controllers.Controller = new List<Controller>(0);

            foreach (SkinnedMeshRenderer m in meshes)
            {
                Controller controller = new Controller();
                controller.Id = $"{m.name}_Controller";
                controller.Skin = new Skin();
                controller.Skin._Source = $"#{m.name}";
                controller.Skin.Bind_shape_matrix = $"{m.localToWorldMatrix.m00} {m.localToWorldMatrix.m01} {m.localToWorldMatrix.m02} {m.localToWorldMatrix.m03} {m.localToWorldMatrix.m10} {m.localToWorldMatrix.m11} {m.localToWorldMatrix.m12} {m.localToWorldMatrix.m13} {m.localToWorldMatrix.m20} {m.localToWorldMatrix.m21} {m.localToWorldMatrix.m22} {m.localToWorldMatrix.m23} {m.localToWorldMatrix.m30} {m.localToWorldMatrix.m31} {m.localToWorldMatrix.m32} {m.localToWorldMatrix.m33}";
                controller.Skin.Source = new List<Source>(0);

                //Joints
                Source joints = new Source();
                joints.Id = $"{m.name}_Joints";
                joints.Name_array = new Name_array();
                joints.Name_array.Id = $"{m.name}_JointArr";
                joints.Name_array.Count = m.bones.Length.ToString();

                StringBuilder jointArr = new StringBuilder();

                foreach(Transform t in m.bones)
                {
                    jointArr.Append($"{t.name} ");
                } //foreach

                jointArr.Length--;

                joints.Name_array.Text = jointArr.ToString();

                joints.Technique_common = new Technique_common();
                joints.Technique_common.Accessor = new Accessor();
                joints.Technique_common.Accessor.Source = $"#{m.name}_JointArr";
                joints.Technique_common.Accessor.Count = m.bones.Length.ToString();
                joints.Technique_common.Accessor.Param = new List<Param>(0);

                Param joint = new Param();
                joint.Name = "JOINT";
                joint.Type = "Name";

                joints.Technique_common.Accessor.Param.Add(joint);

                controller.Skin.Source.Add(joints);

                //Matrices
                Source matrices = new Source();
                matrices.Id = $"{m.name}_Matrices";
                matrices.Float_array = new Float_array();
                matrices.Float_array.Id = $"{m.name}_MatArr";
                matrices.Float_array.Count = (m.bones.Length * 16).ToString();

                StringBuilder matArr = new StringBuilder();

                foreach (Matrix4x4 mtx in m.sharedMesh.bindposes)
                {
                    matArr.Append($"{mtx.m00} {mtx.m01} {mtx.m02} {mtx.m03} {mtx.m10} {mtx.m11} {mtx.m12} {mtx.m13} {mtx.m20} {mtx.m21} {mtx.m22} {mtx.m23} {mtx.m30} {mtx.m31} {mtx.m32} {mtx.m33} ");
                } //foreach

                matArr.Length--;

                matrices.Float_array.Text = matArr.ToString();

                matrices.Technique_common = new Technique_common();
                matrices.Technique_common.Accessor = new Accessor();
                matrices.Technique_common.Accessor.Source = $"#{m.name}_MatArr";
                matrices.Technique_common.Accessor.Count = m.bones.Length.ToString();
                matrices.Technique_common.Accessor.Stride = "16";
                matrices.Technique_common.Accessor.Param = new List<Param>(0);

                Param matrix = new Param();
                matrix.Type = "float4x4";

                matrices.Technique_common.Accessor.Param.Add(matrix);

                controller.Skin.Source.Add(matrices);

                //Weights
                Source weights = new Source();
                weights.Id = $"{m.name}_Weights";
                weights.Float_array = new Float_array();
                weights.Float_array.Id = $"{m.name}_WeightArr";

                List<int> usedWeights = new List<int>(0);
                List<float> uniqueWeights = new List<float>(0);

                int vertexCount = m.sharedMesh.vertexCount;

                for (int i = 0; i < vertexCount; i++)
                {
                    BoneWeight b = m.sharedMesh.boneWeights[i];
                    int weightCount = 0;

                    if(b.weight0 != 0)
                    {
                        weightCount++;

                        if(!uniqueWeights.Contains(b.weight0))
                        {
                            uniqueWeights.Add(b.weight0);
                        } //if
                    } //if

                    if (b.weight1 != 0)
                    {
                        weightCount++;

                        if (!uniqueWeights.Contains(b.weight1))
                        {
                            uniqueWeights.Add(b.weight1);
                        } //if
                    } //if

                    if (b.weight2 != 0)
                    {
                        weightCount++;

                        if (!uniqueWeights.Contains(b.weight2))
                        {
                            uniqueWeights.Add(b.weight2);
                        } //if
                    } //if

                    if (b.weight3 != 0)
                    {
                        weightCount++;

                        if (!uniqueWeights.Contains(b.weight3))
                        {
                            uniqueWeights.Add(b.weight3);
                        } //if
                    } //if

                    usedWeights.Add(weightCount);
                } //for

                weights.Float_array.Count = uniqueWeights.Count.ToString();

                StringBuilder floats = new StringBuilder();
                
                foreach(float f in uniqueWeights)
                {
                    floats.Append($"{f} ");
                } //foreach

                floats.Length--;

                weights.Float_array.Text = floats.ToString();
                weights.Technique_common = new Technique_common();
                weights.Technique_common.Accessor = new Accessor();
                weights.Technique_common.Accessor.Source = $"#{m.name}_WeightArr";
                weights.Technique_common.Accessor.Count = uniqueWeights.Count.ToString();
                weights.Technique_common.Accessor.Param = new List<Param>(0);

                Param flt = new Param();
                flt.Type = "float";

                controller.Skin.Source.Add(weights);

                //Skin Joints
                controller.Skin.Joints = new Joints();
                controller.Skin.Joints.Input = new List<Xml2CSharp.Input>(0);

                Xml2CSharp.Input jointInput = new Xml2CSharp.Input();
                jointInput.Semantic = "JOINT";
                jointInput.Source = $"#{m.name}_Joints";
                controller.Skin.Joints.Input.Add(jointInput);

                Xml2CSharp.Input matrixInput = new Xml2CSharp.Input();
                matrixInput.Semantic = "INV_BIND_MATRIX";
                matrixInput.Source = $"#{m.name}_Matrices";
                controller.Skin.Joints.Input.Add(matrixInput);

                //Vertex Weights
                controller.Skin.Vertex_weights = new Vertex_weights();
                controller.Skin.Vertex_weights.Count = $"{vertexCount}";
                controller.Skin.Vertex_weights.Input = new List<Xml2CSharp.Input>(0);

                Xml2CSharp.Input jointInput1 = new Xml2CSharp.Input();
                jointInput1.Semantic = "JOINT";
                jointInput1.Offset = "0";
                jointInput1.Source = $"#{m.name}_Joints";
                controller.Skin.Vertex_weights.Input.Add(jointInput1);

                Xml2CSharp.Input weightInput = new Xml2CSharp.Input();
                weightInput.Semantic = "WEIGHT";
                weightInput.Offset = "1";
                weightInput.Source = $"#{m.name}_Weights";
                controller.Skin.Vertex_weights.Input.Add(weightInput);

                StringBuilder vCount = new StringBuilder();

                foreach(int i in usedWeights)
                {
                    vCount.Append($"{i} ");
                } //foreach

                vCount.Length--;

                controller.Skin.Vertex_weights.Vcount = vCount.ToString();

                StringBuilder v = new StringBuilder();

                for(int i = 0; i < vertexCount; i++)
                {
                    BoneWeight b = m.sharedMesh.boneWeights[i];

                    switch (usedWeights[i])
                    {
                        case 1:
                            v.Append($"{b.boneIndex0} {uniqueWeights.IndexOf(b.weight0)} ");
                            break;
                        case 2:
                            v.Append($"{b.boneIndex0} {uniqueWeights.IndexOf(b.weight0)} ");
                            v.Append($"{b.boneIndex1} {uniqueWeights.IndexOf(b.weight1)} ");
                            break;
                        case 3:
                            v.Append($"{b.boneIndex0} {uniqueWeights.IndexOf(b.weight0)} ");
                            v.Append($"{b.boneIndex1} {uniqueWeights.IndexOf(b.weight1)} ");
                            v.Append($"{b.boneIndex2} {uniqueWeights.IndexOf(b.weight2)} ");
                            break;
                        case 4:
                            v.Append($"{b.boneIndex0} {uniqueWeights.IndexOf(b.weight0)} ");
                            v.Append($"{b.boneIndex1} {uniqueWeights.IndexOf(b.weight1)} ");
                            v.Append($"{b.boneIndex2} {uniqueWeights.IndexOf(b.weight2)} ");
                            v.Append($"{b.boneIndex3} {uniqueWeights.IndexOf(b.weight3)} ");
                            break;
                    } //switch
                } //for

                v.Length--;

                controller.Skin.Vertex_weights.V = v.ToString();

                collada.Library_controllers.Controller.Add(controller);
            } //foreach

            //Library Visual Scenes
            collada.Library_visual_scenes = new Library_visual_scenes();
            collada.Library_visual_scenes.Visual_scene = new Visual_scene();
            collada.Library_visual_scenes.Visual_scene.Id = gameObject.name;
            collada.Library_visual_scenes.Visual_scene.Name = gameObject.name;

            collada.Library_visual_scenes.Visual_scene.Node = new List<Node>(0);

            //Bone Zone
            foreach (Transform t in gameObject.transform)
            {
                if (t.name == "[Root]")
                {
                    GetBones(t, collada.Library_visual_scenes.Visual_scene.Node);
                    break;
                } //if
            } //foreach

            foreach (SkinnedMeshRenderer m in meshes)
            {
                Node node = new Node();
                node.Id = m.name;
                node.Name = m.name;
                node.Type = "NODE";
                node.Instance_controller = new Instance_controller();
                node.Instance_controller.Url = $"#{m.name}_Controller";
                node.Instance_controller.Skeleton = "#[Root]";

                collada.Library_visual_scenes.Visual_scene.Node.Add(node);
            } //foreach

            //Scene
            collada.Scene = new Scene();
            collada.Scene.Instance_visual_scene = new Instance_visual_scene();
            collada.Scene.Instance_visual_scene.Url = $"#{gameObject.name}";

            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                try
                {
                    xmlSerializer.Serialize(stream, collada);
                    Debug.Log("Export successful! Please note that this exporter is still incomplete and is intended for debugging purposes.");
                } //try
                finally
                {
                    stream.Close();
                } //finally
            } //using
        } //ConvertToCOLLADA

        private static void GetBones(Transform transform, List<Node> nodes)
        {
            Node node = new Node();
            node.Id = transform.name;
            node.Name = transform.name;
            node.Sid = transform.name;
            node.Type = "JOINT";

            if(transform.localPosition != new Vector3(0, 0, 0))
            {
                node.Translate = $"{transform.localPosition.x} {transform.localPosition.y} {transform.localPosition.z}";
            } //if

            node.SubNode = new List<Node>(0);

            foreach(Transform t in transform)
            {
                GetBones(t, node.SubNode);
            } //foreach

            nodes.Add(node);
        } //GetBones

        private static List<SkinnedMeshRenderer> GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes)
        {
            foreach(Transform t in transform)
            {
                if(t.GetComponent<SkinnedMeshRenderer>())
                {
                    meshes.Add(t.GetComponent<SkinnedMeshRenderer>());
                } //if
            } //foreach

            return meshes;
        } //GetMeshes
    } //COLLADAConverter
} //namespace