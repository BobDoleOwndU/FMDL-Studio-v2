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

                Xml2CSharp.Input normal = new Xml2CSharp.Input();
                normal.Semantic = "NORMAL";
                normal.Source = $"#{normals.Id}";
                normal.Offset = "1";
                geometry.Mesh.Triangles.Input.Add(normal);

                Xml2CSharp.Input texcoord = new Xml2CSharp.Input();
                texcoord.Semantic = "TEXCOORD";
                texcoord.Source = $"#{uvs0.Id}";
                texcoord.Set = "0";
                texcoord.Offset = "1";
                geometry.Mesh.Triangles.Input.Add(texcoord);

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
                //controller.Skin.Bind_shape_matrix = m.localToWorldMatrix.ToString();
                //controller.Skin.Source = new List<Source>(0);

                //Joints
                //Source joints = new Source();
                //joints.Id = $"{m.name}_Joints";

                collada.Library_controllers.Controller.Add(controller);
            } //foreach

            //Library Visual Scenes
            collada.Library_visual_scenes = new Library_visual_scenes();
            collada.Library_visual_scenes.Visual_scene = new Visual_scene();
            collada.Library_visual_scenes.Visual_scene.Id = gameObject.name;
            collada.Library_visual_scenes.Visual_scene.Name = gameObject.name;

            collada.Library_visual_scenes.Visual_scene.Node = new List<Node>(0);

            foreach (SkinnedMeshRenderer m in meshes)
            {
                Node node = new Node();
                node.Id = m.name;
                node.Name = m.name;
                node.Type = "NODE";
                node.Instance_controller = new Instance_controller();
                node.Instance_controller.Url = $"{m.name}_Controller";

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
                    Debug.Log("Export successful! Please note that this does not currently output valid COLLADA files. It is just for debugging purposes.");
                } //try
                finally
                {
                    stream.Close();
                } //finally
            } //using
        } //ConvertToCOLLADA

        private static List<Transform> GetBones(Transform transform)
        {
            List<Transform> bones = new List<Transform>(0);

            foreach(Transform t in transform)
            {
                bones.Add(t);

                GetBones(t);
            } //foreach

            return bones;
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