/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Xml2CSharp
{
    [XmlRoot(ElementName = "contributor", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Contributor
    {
        [XmlElement(ElementName = "authoring_tool", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Authoring_tool { get; set; }
    }

    [XmlRoot(ElementName = "unit", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Unit
    {
        [XmlAttribute(AttributeName = "meter")]
        public string Meter { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "asset", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Asset
    {
        [XmlElement(ElementName = "contributor", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Contributor Contributor { get; set; }
        [XmlElement(ElementName = "created", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Created { get; set; }
        [XmlElement(ElementName = "modified", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Modified { get; set; }
        [XmlElement(ElementName = "unit", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Unit Unit { get; set; }
        [XmlElement(ElementName = "up_axis", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Up_axis { get; set; }
    }

    [XmlRoot(ElementName = "init_from", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Init_from
    {
        [XmlElement(ElementName = "ref", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Ref { get; set; }
    }

    [XmlRoot(ElementName = "image", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Image
    {
        [XmlElement(ElementName = "init_from", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Init_from Init_from { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "library_images", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_images
    {
        [XmlElement(ElementName = "image", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Image> Image { get; set; }
    }

    [XmlRoot(ElementName = "instance_effect", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Instance_effect
    {
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Material
    {
        [XmlElement(ElementName = "instance_effect", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Instance_effect Instance_effect { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "library_materials", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_materials
    {
        [XmlElement(ElementName = "material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Material> Material { get; set; }
    }

    [XmlRoot(ElementName = "surface", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Surface
    {
        [XmlElement(ElementName = "init_from", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Init_from Init_from { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "newparam", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Newparam
    {
        [XmlElement(ElementName = "surface", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Surface Surface { get; set; }
        [XmlAttribute(AttributeName = "sid")]
        public string Sid { get; set; }
        [XmlElement(ElementName = "sampler2D", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Sampler2D Sampler2D { get; set; }
    }

    [XmlRoot(ElementName = "instance_image", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Instance_image
    {
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "sampler2D", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Sampler2D
    {
        [XmlElement(ElementName = "source", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Source { get; set; }
        [XmlElement(ElementName = "instance_image", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Instance_image Instance_image { get; set; }
        [XmlElement(ElementName = "wrap_s", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Wrap_s { get; set; }
        [XmlElement(ElementName = "wrap_t", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Wrap_t { get; set; }
        [XmlElement(ElementName = "minfilter", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Minfilter { get; set; }
        [XmlElement(ElementName = "mipfilter", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Mipfilter { get; set; }
        [XmlElement(ElementName = "magfilter", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Magfilter { get; set; }
    }

    [XmlRoot(ElementName = "texture", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Texture
    {
        [XmlAttribute(AttributeName = "texture")]
        public string _texture { get; set; }
        [XmlAttribute(AttributeName = "texcoord")]
        public string Texcoord { get; set; }
    }

    [XmlRoot(ElementName = "diffuse", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Diffuse
    {
        [XmlElement(ElementName = "texture", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Texture Texture { get; set; }
    }

    [XmlRoot(ElementName = "index_of_refraction", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Index_of_refraction
    {
        [XmlElement(ElementName = "float", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Float { get; set; }
    }

    [XmlRoot(ElementName = "phong", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Phong
    {
        [XmlElement(ElementName = "diffuse", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Diffuse Diffuse { get; set; }
        [XmlElement(ElementName = "index_of_refraction", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Index_of_refraction Index_of_refraction { get; set; }
    }

    [XmlRoot(ElementName = "technique", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Technique
    {
        [XmlElement(ElementName = "phong", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Phong Phong { get; set; }
        [XmlAttribute(AttributeName = "sid")]
        public string Sid { get; set; }
    }

    [XmlRoot(ElementName = "profile_COMMON", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Profile_COMMON
    {
        [XmlElement(ElementName = "newparam", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Newparam> Newparam { get; set; }
        [XmlElement(ElementName = "technique", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Technique Technique { get; set; }
    }

    [XmlRoot(ElementName = "effect", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Effect
    {
        [XmlElement(ElementName = "profile_COMMON", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Profile_COMMON Profile_COMMON { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "library_effects", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_effects
    {
        [XmlElement(ElementName = "effect", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Effect> Effect { get; set; }
    }

    [XmlRoot(ElementName = "float_array", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Float_array
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "param", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Param
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "accessor", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Accessor
    {
        [XmlElement(ElementName = "param", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Param> Param { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlAttribute(AttributeName = "stride")]
        public string Stride { get; set; }
    }

    [XmlRoot(ElementName = "technique_common", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Technique_common
    {
        [XmlElement(ElementName = "accessor", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Accessor Accessor { get; set; }
        [XmlElement(ElementName = "instance_material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Instance_material Instance_material { get; set; }
    }

    [XmlRoot(ElementName = "source", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Source
    {
        [XmlElement(ElementName = "float_array", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Float_array Float_array { get; set; }
        [XmlElement(ElementName = "technique_common", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Technique_common Technique_common { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Name_array", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Name_array Name_array { get; set; }
    }

    [XmlRoot(ElementName = "input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Input
    {
        [XmlAttribute(AttributeName = "semantic")]
        public string Semantic { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "offset")]
        public string Offset { get; set; }
        [XmlAttribute(AttributeName = "set")]
        public string Set { get; set; }
    }

    [XmlRoot(ElementName = "vertices", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Vertices
    {
        [XmlElement(ElementName = "input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Input Input { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "triangles", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Triangles
    {
        [XmlElement(ElementName = "input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Input> Input { get; set; }
        [XmlElement(ElementName = "p", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string P { get; set; }
        [XmlAttribute(AttributeName = "material")]
        public string Material { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "mesh", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Mesh
    {
        [XmlElement(ElementName = "source", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Source> Source { get; set; }
        [XmlElement(ElementName = "vertices", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Vertices Vertices { get; set; }
        [XmlElement(ElementName = "triangles", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Triangles Triangles { get; set; }
    }

    [XmlRoot(ElementName = "geometry", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Geometry
    {
        [XmlElement(ElementName = "mesh", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Mesh Mesh { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "library_geometries", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_geometries
    {
        [XmlElement(ElementName = "geometry", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Geometry> Geometry { get; set; }
    }

    [XmlRoot(ElementName = "Name_array", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Name_array
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "joints", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Joints
    {
        [XmlElement(ElementName = "input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Input> Input { get; set; }
    }

    [XmlRoot(ElementName = "vertex_weights", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Vertex_weights
    {
        [XmlElement(ElementName = "input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Input> Input { get; set; }
        [XmlElement(ElementName = "vcount", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Vcount { get; set; }
        [XmlElement(ElementName = "v", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string V { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "skin", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Skin
    {
        [XmlElement(ElementName = "bind_shape_matrix", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Bind_shape_matrix { get; set; }
        [XmlElement(ElementName = "source", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Source> Source { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string _Source { get; set; }
        [XmlElement(ElementName = "joints", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Joints Joints { get; set; }
        [XmlElement(ElementName = "vertex_weights", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Vertex_weights Vertex_weights { get; set; }
    }

    [XmlRoot(ElementName = "controller", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Controller
    {
        [XmlElement(ElementName = "skin", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Skin Skin { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "library_controllers", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_controllers
    {
        [XmlElement(ElementName = "controller", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Controller> Controller { get; set; }
    }

    [XmlRoot(ElementName = "node", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Node
    {
        [XmlElement(ElementName = "translate", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Translate { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "sid")]
        public string Sid { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "node", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Node> SubNode { get; set; }
        [XmlElement(ElementName = "rotate", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Rotate { get; set; }
        [XmlElement(ElementName = "instance_controller", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Instance_controller Instance_controller { get; set; }
    }

    [XmlRoot(ElementName = "bind_vertex_input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Bind_vertex_input
    {
        [XmlAttribute(AttributeName = "semantic")]
        public string Semantic { get; set; }
        [XmlAttribute(AttributeName = "input_semantic")]
        public string Input_semantic { get; set; }
        [XmlAttribute(AttributeName = "input_set")]
        public string Input_set { get; set; }
    }

    [XmlRoot(ElementName = "instance_material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Instance_material
    {
        [XmlElement(ElementName = "bind_vertex_input", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Bind_vertex_input> Bind_vertex_input { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }
    }

    [XmlRoot(ElementName = "bind_material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Bind_material
    {
        [XmlElement(ElementName = "technique_common", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Technique_common Technique_common { get; set; }
    }

    [XmlRoot(ElementName = "instance_controller", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Instance_controller
    {
        [XmlElement(ElementName = "skeleton", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public string Skeleton { get; set; }
        [XmlElement(ElementName = "bind_material", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Bind_material Bind_material { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "visual_scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Visual_scene
    {
        [XmlElement(ElementName = "node", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public List<Node> Node { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "library_visual_scenes", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Library_visual_scenes
    {
        [XmlElement(ElementName = "visual_scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Visual_scene Visual_scene { get; set; }
    }

    [XmlRoot(ElementName = "instance_visual_scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Instance_visual_scene
    {
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class Scene
    {
        [XmlElement(ElementName = "instance_visual_scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Instance_visual_scene Instance_visual_scene { get; set; }
    }

    [XmlRoot(ElementName = "COLLADA", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
    public class COLLADA
    {
        [XmlElement(ElementName = "asset", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Asset Asset { get; set; }
        [XmlElement(ElementName = "library_images", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_images Library_images { get; set; }
        [XmlElement(ElementName = "library_materials", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_materials Library_materials { get; set; }
        [XmlElement(ElementName = "library_effects", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_effects Library_effects { get; set; }
        [XmlElement(ElementName = "library_geometries", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_geometries Library_geometries { get; set; }
        [XmlElement(ElementName = "library_controllers", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_controllers Library_controllers { get; set; }
        [XmlElement(ElementName = "library_visual_scenes", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Library_visual_scenes Library_visual_scenes { get; set; }
        [XmlElement(ElementName = "scene", Namespace = "http://www.collada.org/2008/03/COLLADASchema")]
        public Scene Scene { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
