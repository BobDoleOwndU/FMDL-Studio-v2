using FmdlStudio.Scripts.Static;
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
            List<UnityEngine.Material> materials = new List<UnityEngine.Material>(0);
            List<UnityEngine.Texture> textures = new List<UnityEngine.Texture>(0);

            GetMeshes(gameObject.transform, meshes, materials, textures);

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

            //Library Images
            collada.Library_images = new Library_images();
            collada.Library_images.Image = new List<Image>(0);

            foreach (UnityEngine.Texture t in textures)
            {
                string textureName = Path.GetFileNameWithoutExtension(t.name);

                Image image = new Image();
                image.Id = $"{textureName}-image";
                image.Name = textureName;
                image.Init_from = new Init_from();
                image.Init_from.Ref = $"/{Globals.texturePath}/{t.name}";

                collada.Library_images.Image.Add(image);
            } //foreach

            //Library Materials
            collada.Library_materials = new Library_materials();
            collada.Library_materials.Material = new List<Xml2CSharp.Material>(0);

            foreach (UnityEngine.Material m in materials)
            {
                Xml2CSharp.Material material = new Xml2CSharp.Material();
                material.Id = m.name;
                material.Instance_effect = new Instance_effect();
                material.Instance_effect.Url = $"#{m.name}-fx";

                collada.Library_materials.Material.Add(material);
            } //foreach

            //Library Effects
            collada.Library_effects = new Library_effects();
            collada.Library_effects.Effect = new List<Effect>(0);

            foreach (UnityEngine.Material m in materials)
            {
                Effect effect = new Effect();
                effect.Id = $"{m.name}-fx";
                effect.Name = m.name;
                effect.Profile_COMMON = new Profile_COMMON();
                effect.Profile_COMMON.Newparam = new List<Newparam>(0);

                if (m.HasProperty("_Base_Tex_SRGB"))
                {
                    UnityEngine.Texture texture = m.GetTexture("_Base_Tex_SRGB");
                    string textureName = Path.GetFileNameWithoutExtension(texture.name);

                    Newparam newparam0 = new Newparam();
                    newparam0.Sid = $"{textureName}-surface";
                    newparam0.Surface = new Surface();
                    newparam0.Surface.Type = "2D";
                    newparam0.Surface.Init_from = new Init_from();
                    newparam0.Surface.Init_from.Ref = $"{textureName}-image";

                    effect.Profile_COMMON.Newparam.Add(newparam0);

                    Newparam newparam1 = new Newparam();
                    newparam1.Sid = $"{textureName}-sampler";
                    newparam1.Sampler2D = new Sampler2D();
                    newparam1.Sampler2D.Source = $"{textureName}-surface";
                    newparam1.Sampler2D.Instance_image = new Instance_image();
                    newparam1.Sampler2D.Instance_image.Url = $"#{textureName}-image";
                    newparam1.Sampler2D.Wrap_s = "CLAMP";
                    newparam1.Sampler2D.Wrap_t = "CLAMP";
                    newparam1.Sampler2D.Minfilter = "NEAREST";
                    newparam1.Sampler2D.Mipfilter = "NEAREST";
                    newparam1.Sampler2D.Magfilter = "LINEAR";

                    effect.Profile_COMMON.Newparam.Add(newparam1);
                } //if

                effect.Profile_COMMON.Technique = new Technique();
                effect.Profile_COMMON.Technique.Sid = "COMMON";
                effect.Profile_COMMON.Technique.Phong = new Phong();

                if (m.HasProperty("_Base_Tex_SRGB"))
                {
                    UnityEngine.Texture texture = m.GetTexture("_Base_Tex_SRGB");
                    string textureName = Path.GetFileNameWithoutExtension(texture.name);

                    effect.Profile_COMMON.Technique.Phong.Diffuse = new Diffuse();
                    effect.Profile_COMMON.Technique.Phong.Diffuse.Texture = new Xml2CSharp.Texture();
                    effect.Profile_COMMON.Technique.Phong.Diffuse.Texture._texture = $"{textureName}-sampler";
                    effect.Profile_COMMON.Technique.Phong.Diffuse.Texture.Texcoord = "TEXCOORD0";
                } //if

                collada.Library_effects.Effect.Add(effect);
            } //foreach

            //Library Geometries
            collada.Library_geometries = new Library_geometries();
            collada.Library_geometries.Geometry = new List<Geometry>(0);

            foreach (SkinnedMeshRenderer m in meshes)
            {
                Geometry geometry = new Geometry();
                geometry.Id = m.name.Replace(' ', '_');
                geometry.Name = m.name.Replace(' ', '_');
                geometry.Mesh = new Xml2CSharp.Mesh();
                geometry.Mesh.Source = new List<Source>(0);

                //Positions
                Source positions = new Source();
                positions.Id = $"{m.name.Replace(' ', '_')}_Positions";
                positions.Float_array = new Float_array();
                positions.Float_array.Id = $"{m.name.Replace(' ', '_')}_PosArr";
                positions.Float_array.Count = (m.sharedMesh.vertexCount * 3).ToString();

                StringBuilder posArr = new StringBuilder();

                foreach (Vector3 v in m.sharedMesh.vertices)
                {
                    posArr.Append($"{-v.x} {v.y} {v.z} ");
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
                normals.Id = $"{m.name.Replace(' ', '_')}_Normals";
                normals.Float_array = new Float_array();
                normals.Float_array.Id = $"{m.name.Replace(' ', '_')}_NormArr";
                normals.Float_array.Count = (m.sharedMesh.normals.Length * 3).ToString();

                StringBuilder normArr = new StringBuilder();

                foreach (Vector3 v in m.sharedMesh.normals)
                {
                    normArr.Append($"{-v.x} {v.y} {v.z} ");
                } //foreach

                normArr.Length--;

                normals.Float_array.Text = normArr.ToString();
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
                if (m.sharedMesh.uv.Length > 0)
                {
                    Source uvs0 = new Source();
                    uvs0.Id = $"{m.name.Replace(' ', '_')}_UVs0";
                    uvs0.Float_array = new Float_array();
                    uvs0.Float_array.Id = $"{m.name.Replace(' ', '_')}_UVArr0";
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
                } //if

                //UVs1
                if (m.sharedMesh.uv2.Length > 0)
                {
                    Source uvs1 = new Source();
                    uvs1.Id = $"{m.name.Replace(' ', '_')}_UVs1";
                    uvs1.Float_array = new Float_array();
                    uvs1.Float_array.Id = $"{m.name.Replace(' ', '_')}_UVArr1";
                    uvs1.Float_array.Count = (m.sharedMesh.uv.Length * 2).ToString();

                    StringBuilder uvArr1 = new StringBuilder();

                    foreach (Vector2 v in m.sharedMesh.uv2)
                    {
                        uvArr1.Append($"{v.x} {v.y} ");
                    } //foreach

                    uvArr1.Length--;

                    uvs1.Float_array.Text = uvArr1.ToString();
                    uvs1.Technique_common = new Technique_common();
                    uvs1.Technique_common.Accessor = new Accessor();
                    uvs1.Technique_common.Accessor.Source = $"#{uvs1.Float_array.Id}";
                    uvs1.Technique_common.Accessor.Count = m.sharedMesh.uv2.Length.ToString();
                    uvs1.Technique_common.Accessor.Stride = "2";

                    Param s = new Param();
                    Param t = new Param();
                    s.Name = "S";
                    t.Name = "T";
                    s.Type = "float";
                    t.Type = "float";
                    uvs1.Technique_common.Accessor.Param = new List<Param>(0);
                    uvs1.Technique_common.Accessor.Param.Add(s);
                    uvs1.Technique_common.Accessor.Param.Add(t);

                    geometry.Mesh.Source.Add(uvs1);
                } //if

                //UVs2
                if (m.sharedMesh.uv3.Length > 0)
                {
                    Source uvs2 = new Source();
                    uvs2.Id = $"{m.name.Replace(' ', '_')}_UVs2";
                    uvs2.Float_array = new Float_array();
                    uvs2.Float_array.Id = $"{m.name.Replace(' ', '_')}_UVArr2";
                    uvs2.Float_array.Count = (m.sharedMesh.uv3.Length * 2).ToString();

                    StringBuilder uvArr2 = new StringBuilder();

                    foreach (Vector2 v in m.sharedMesh.uv3)
                    {
                        uvArr2.Append($"{v.x} {v.y} ");
                    } //foreach

                    uvArr2.Length--;

                    uvs2.Float_array.Text = uvArr2.ToString();
                    uvs2.Technique_common = new Technique_common();
                    uvs2.Technique_common.Accessor = new Accessor();
                    uvs2.Technique_common.Accessor.Source = $"#{uvs2.Float_array.Id}";
                    uvs2.Technique_common.Accessor.Count = m.sharedMesh.uv3.Length.ToString();
                    uvs2.Technique_common.Accessor.Stride = "2";

                    Param s = new Param();
                    Param t = new Param();
                    s.Name = "S";
                    t.Name = "T";
                    s.Type = "float";
                    t.Type = "float";
                    uvs2.Technique_common.Accessor.Param = new List<Param>(0);
                    uvs2.Technique_common.Accessor.Param.Add(s);
                    uvs2.Technique_common.Accessor.Param.Add(t);

                    geometry.Mesh.Source.Add(uvs2);
                } //if

                //UVs3
                if (m.sharedMesh.uv4.Length > 0)
                {
                    Source uvs3 = new Source();
                    uvs3.Id = $"{m.name.Replace(' ', '_')}_UVs3";
                    uvs3.Float_array = new Float_array();
                    uvs3.Float_array.Id = $"{m.name.Replace(' ', '_')}_UVArr3";
                    uvs3.Float_array.Count = (m.sharedMesh.uv4.Length * 2).ToString();

                    StringBuilder uvArr3 = new StringBuilder();

                    foreach (Vector2 v in m.sharedMesh.uv4)
                    {
                        uvArr3.Append($"{v.x} {v.y} ");
                    } //foreach

                    uvArr3.Length--;

                    uvs3.Float_array.Text = uvArr3.ToString();
                    uvs3.Technique_common = new Technique_common();
                    uvs3.Technique_common.Accessor = new Accessor();
                    uvs3.Technique_common.Accessor.Source = $"#{uvs3.Float_array.Id}";
                    uvs3.Technique_common.Accessor.Count = m.sharedMesh.uv4.Length.ToString();
                    uvs3.Technique_common.Accessor.Stride = "2";

                    Param s = new Param();
                    Param t = new Param();
                    s.Name = "S";
                    t.Name = "T";
                    s.Type = "float";
                    t.Type = "float";
                    uvs3.Technique_common.Accessor.Param = new List<Param>(0);
                    uvs3.Technique_common.Accessor.Param.Add(s);
                    uvs3.Technique_common.Accessor.Param.Add(t);

                    geometry.Mesh.Source.Add(uvs3);
                } //if

                //Vertices
                geometry.Mesh.Vertices = new Vertices();
                geometry.Mesh.Vertices.Id = $"{m.name.Replace(' ', '_')}_Vertices";
                geometry.Mesh.Vertices.Input = new Xml2CSharp.Input();
                geometry.Mesh.Vertices.Input.Semantic = "POSITION";
                geometry.Mesh.Vertices.Input.Source = $"#{positions.Id}";

                //Triangles
                geometry.Mesh.Triangles = new Triangles();
                geometry.Mesh.Triangles.Material = m.sharedMaterial.name;
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
                normal.Offset = "0";
                geometry.Mesh.Triangles.Input.Add(normal);

                if (m.sharedMesh.uv.Length > 0)
                {
                    Xml2CSharp.Input texcoord = new Xml2CSharp.Input();
                    texcoord.Semantic = "TEXCOORD";
                    texcoord.Source = $"#{m.name.Replace(' ', '_')}_UVs0";
                    texcoord.Set = "0";
                    texcoord.Offset = "0";
                    geometry.Mesh.Triangles.Input.Add(texcoord);
                } //if

                if (m.sharedMesh.uv2.Length > 0)
                {
                    Xml2CSharp.Input texcoord1 = new Xml2CSharp.Input();
                    texcoord1.Semantic = "TEXCOORD";
                    texcoord1.Source = $"#{m.name.Replace(' ', '_')}_UVs1";
                    texcoord1.Set = "1";
                    texcoord1.Offset = "0";
                    geometry.Mesh.Triangles.Input.Add(texcoord1);
                } //if

                if (m.sharedMesh.uv3.Length > 0)
                {
                    Xml2CSharp.Input texcoord2 = new Xml2CSharp.Input();
                    texcoord2.Semantic = "TEXCOORD";
                    texcoord2.Source = $"#{m.name.Replace(' ', '_')}_UVs2";
                    texcoord2.Set = "2";
                    texcoord2.Offset = "0";
                    geometry.Mesh.Triangles.Input.Add(texcoord2);
                } //if

                if (m.sharedMesh.uv4.Length > 0)
                {
                    Xml2CSharp.Input texcoord3 = new Xml2CSharp.Input();
                    texcoord3.Semantic = "TEXCOORD";
                    texcoord3.Source = $"#{m.name.Replace(' ', '_')}_UVs3";
                    texcoord3.Set = "3";
                    texcoord3.Offset = "0";
                    geometry.Mesh.Triangles.Input.Add(texcoord3);
                } //if

                StringBuilder trianglesArr = new StringBuilder();

                //flip triangles
                int triangleCount = m.sharedMesh.triangles.Length;

                for (int i = 0; i < triangleCount; i += 3)
                {
                    trianglesArr.Append($"{m.sharedMesh.triangles[i + 1]} {m.sharedMesh.triangles[i]} {m.sharedMesh.triangles[i + 2]} ");
                } //for

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
                controller.Id = $"{m.name.Replace(' ', '_')}_Controller";
                controller.Skin = new Skin();
                controller.Skin._Source = $"#{m.name.Replace(' ', '_')}";
                controller.Skin.Bind_shape_matrix = $"{m.localToWorldMatrix.m00} {m.localToWorldMatrix.m01} {m.localToWorldMatrix.m02} {-m.localToWorldMatrix.m03} {m.localToWorldMatrix.m10} {m.localToWorldMatrix.m11} {m.localToWorldMatrix.m12} {m.localToWorldMatrix.m13} {m.localToWorldMatrix.m20} {m.localToWorldMatrix.m21} {m.localToWorldMatrix.m22} {m.localToWorldMatrix.m23} {m.localToWorldMatrix.m30} {m.localToWorldMatrix.m31} {m.localToWorldMatrix.m32} {m.localToWorldMatrix.m33}";
                controller.Skin.Source = new List<Source>(0);

                //Joints
                Source joints = new Source();
                joints.Id = $"{m.name.Replace(' ', '_')}_Joints";
                joints.Name_array = new Name_array();
                joints.Name_array.Id = $"{m.name.Replace(' ', '_')}_JointArr";
                joints.Name_array.Count = m.bones.Length.ToString();

                StringBuilder jointArr = new StringBuilder();

                foreach (Transform t in m.bones)
                {
                    jointArr.Append($"{t.name.Replace(' ', '_')} ");
                } //foreach

                jointArr.Length--;

                joints.Name_array.Text = jointArr.ToString();

                joints.Technique_common = new Technique_common();
                joints.Technique_common.Accessor = new Accessor();
                joints.Technique_common.Accessor.Source = $"#{m.name.Replace(' ', '_')}_JointArr";
                joints.Technique_common.Accessor.Count = m.bones.Length.ToString();
                joints.Technique_common.Accessor.Param = new List<Param>(0);

                Param joint = new Param();
                joint.Name = "JOINT";
                joint.Type = "Name";

                joints.Technique_common.Accessor.Param.Add(joint);

                controller.Skin.Source.Add(joints);

                //Matrices
                Source matrices = new Source();
                matrices.Id = $"{m.name.Replace(' ', '_')}_Matrices";
                matrices.Float_array = new Float_array();
                matrices.Float_array.Id = $"{m.name.Replace(' ', '_')}_MatArr";
                matrices.Float_array.Count = (m.bones.Length * 16).ToString();

                StringBuilder matArr = new StringBuilder();

                foreach (Matrix4x4 mtx in m.sharedMesh.bindposes)
                {
                    matArr.Append($"{mtx.m00} {mtx.m01} {mtx.m02} {-mtx.m03} {mtx.m10} {mtx.m11} {mtx.m12} {mtx.m13} {mtx.m20} {mtx.m21} {mtx.m22} {mtx.m23} {mtx.m30} {mtx.m31} {mtx.m32} {mtx.m33} ");
                } //foreach

                matArr.Length--;

                matrices.Float_array.Text = matArr.ToString();

                matrices.Technique_common = new Technique_common();
                matrices.Technique_common.Accessor = new Accessor();
                matrices.Technique_common.Accessor.Source = $"#{m.name.Replace(' ', '_')}_MatArr";
                matrices.Technique_common.Accessor.Count = m.bones.Length.ToString();
                matrices.Technique_common.Accessor.Stride = "16";
                matrices.Technique_common.Accessor.Param = new List<Param>(0);

                Param matrix = new Param();
                matrix.Type = "float4x4";

                matrices.Technique_common.Accessor.Param.Add(matrix);

                controller.Skin.Source.Add(matrices);

                //Weights
                Source weights = new Source();
                weights.Id = $"{m.name.Replace(' ', '_')}_Weights";
                weights.Float_array = new Float_array();
                weights.Float_array.Id = $"{m.name.Replace(' ', '_')}_WeightArr";

                List<int> usedWeights = new List<int>(0);
                List<float> uniqueWeights = new List<float>(0);

                int vertexCount = m.sharedMesh.vertexCount;

                for (int i = 0; i < vertexCount; i++)
                {
                    BoneWeight b = m.sharedMesh.boneWeights[i];
                    int weightCount = 0;

                    if (b.weight0 != 0)
                    {
                        weightCount++;

                        if (!uniqueWeights.Contains(b.weight0))
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

                foreach (float f in uniqueWeights)
                {
                    floats.Append($"{f} ");
                } //foreach

                floats.Length--;

                weights.Float_array.Text = floats.ToString();
                weights.Technique_common = new Technique_common();
                weights.Technique_common.Accessor = new Accessor();
                weights.Technique_common.Accessor.Source = $"#{m.name.Replace(' ', '_')}_WeightArr";
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
                jointInput.Source = $"#{m.name.Replace(' ', '_')}_Joints";
                controller.Skin.Joints.Input.Add(jointInput);

                Xml2CSharp.Input matrixInput = new Xml2CSharp.Input();
                matrixInput.Semantic = "INV_BIND_MATRIX";
                matrixInput.Source = $"#{m.name.Replace(' ', '_')}_Matrices";
                controller.Skin.Joints.Input.Add(matrixInput);

                //Vertex Weights
                controller.Skin.Vertex_weights = new Vertex_weights();
                controller.Skin.Vertex_weights.Count = $"{vertexCount}";
                controller.Skin.Vertex_weights.Input = new List<Xml2CSharp.Input>(0);

                Xml2CSharp.Input jointInput1 = new Xml2CSharp.Input();
                jointInput1.Semantic = "JOINT";
                jointInput1.Offset = "0";
                jointInput1.Source = $"#{m.name.Replace(' ', '_')}_Joints";
                controller.Skin.Vertex_weights.Input.Add(jointInput1);

                Xml2CSharp.Input weightInput = new Xml2CSharp.Input();
                weightInput.Semantic = "WEIGHT";
                weightInput.Offset = "1";
                weightInput.Source = $"#{m.name.Replace(' ', '_')}_Weights";
                controller.Skin.Vertex_weights.Input.Add(weightInput);

                StringBuilder vCount = new StringBuilder();

                foreach (int i in usedWeights)
                {
                    vCount.Append($"{i} ");
                } //foreach

                vCount.Length--;

                controller.Skin.Vertex_weights.Vcount = vCount.ToString();

                StringBuilder v = new StringBuilder();

                for (int i = 0; i < vertexCount; i++)
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
                node.Id = m.name.Replace(' ', '_');
                node.Name = m.name.Replace(' ', '_');
                node.Type = "NODE";
                node.Instance_controller = new Instance_controller();
                node.Instance_controller.Url = $"#{m.name.Replace(' ', '_')}_Controller";
                node.Instance_controller.Skeleton = "#[Root]";
                node.Instance_controller.Bind_material = new Bind_material();
                node.Instance_controller.Bind_material.Technique_common = new Technique_common();
                node.Instance_controller.Bind_material.Technique_common.Instance_material = new Instance_material();
                node.Instance_controller.Bind_material.Technique_common.Instance_material.Symbol = m.sharedMaterial.name;
                node.Instance_controller.Bind_material.Technique_common.Instance_material.Target = $"#{m.sharedMaterial.name}";
                node.Instance_controller.Bind_material.Technique_common.Instance_material.Bind_vertex_input = new List<Bind_vertex_input>(0);

                Bind_vertex_input bvi = new Bind_vertex_input();
                bvi.Semantic = "TEXCOORD0";
                bvi.Input_semantic = "TEXCOORD";
                bvi.Input_set = "0";

                node.Instance_controller.Bind_material.Technique_common.Instance_material.Bind_vertex_input.Add(bvi);

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
            node.Id = transform.name.Replace(' ', '_');
            node.Name = transform.name.Replace(' ', '_');
            node.Sid = transform.name.Replace(' ', '_');
            node.Type = "JOINT";

            if (transform.localPosition != new Vector3(0, 0, 0))
            {
                node.Translate = $"{-transform.localPosition.x} {transform.localPosition.y} {transform.localPosition.z}";
            } //if

            node.SubNode = new List<Node>(0);

            foreach (Transform t in transform)
            {
                GetBones(t, node.SubNode);
            } //foreach

            nodes.Add(node);
        } //GetBones

        private static void GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes, List<UnityEngine.Material> materials, List<UnityEngine.Texture> textures)
        {
            foreach (Transform t in transform)
            {
                if (t.GetComponent<SkinnedMeshRenderer>())
                {
                    SkinnedMeshRenderer mesh = t.gameObject.GetComponent<SkinnedMeshRenderer>();

                    meshes.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>());

                    UnityEngine.Material material = mesh.sharedMaterial;

                    if (!materials.Contains(material))
                    {
                        materials.Add(material);

                        if (material.HasProperty("_Base_Tex_SRGB"))
                        {
                            if (!textures.Contains(material.GetTexture("_Base_Tex_SRGB")))
                            {
                                textures.Add(material.GetTexture("_Base_Tex_SRGB"));
                            } //if
                        } //if

                        if (material.HasProperty("_Base_Tex_SRGB"))
                        {
                            if (!textures.Contains(material.GetTexture("_Base_Tex_SRGB")))
                            {
                                textures.Add(material.GetTexture("_Base_Tex_SRGB"));
                            } //if
                        } //if
                    } //if
                } //if
            } //foreach
        } //GetMeshes
    } //COLLADAConverter
} //namespace