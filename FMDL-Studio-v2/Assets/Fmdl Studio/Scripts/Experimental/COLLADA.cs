using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Fmdl_Studio.Scripts.Experimental
{
    [XmlType("COLLADA")]
    public class COLLADA
    {
        [XmlType("asset")]
        public class Asset
        {
            [XmlType("contributor")]
            public class Contributor
            {
                [XmlElement]
                public string authoring_tool = "Fmdl Studio v2";
            } //Contributor

            [XmlType("unit")]
            public class Unit
            {
                [XmlAttribute]
                public float meter = 1.0f;

                [XmlAttribute]
                public string name = "meter";
            } //Unit

            [XmlElement]
            public Contributor contributor = new Contributor();

            [XmlElement]
            public DateTime created = DateTime.Now;

            [XmlElement]
            public DateTime modified = DateTime.Now;

            [XmlElement]
            public Unit unit = new Unit();

            [XmlAttribute]
            public string up_axis = "Y_UP";
        } //Asset

        [XmlType("init_from")]
        public class InitFrom
        {
            [XmlElement("ref")]
            public string reference;
        } //InitFrom

        [XmlType("image")]
        public class Image
        {
            [XmlAttribute]
            public string id;

            [XmlAttribute]
            public string name;

            [XmlElement]
            public InitFrom init_from = new InitFrom();
        } //Image

        [XmlType("material")]
        public class Material
        {
            [XmlType("instance_effect")]
            public class InstanceEffect
            {
                [XmlAttribute]
                public string url;
            } //InstanceEffect

            [XmlAttribute]
            public string id;

            [XmlElement]
            public InstanceEffect instance_effect = new InstanceEffect();
        } //Material

        [XmlType("effect")]
        public class Effect
        {
            [XmlType("profile_COMMON")]
            public class ProfileCommon
            {
                [XmlType("newparam")]
                public class Newparam
                {
                    [XmlType("surface")]
                    public class Surface
                    {
                        [XmlAttribute]
                        public string type;

                        [XmlElement]
                        public InitFrom init_from = new InitFrom();
                    } //Surface

                    [XmlType("sampler2D")]
                    public class Sampler2D
                    {
                        [XmlType("instance_image")]
                        public class InstanceImage
                        {
                            [XmlAttribute]
                            public string url;
                        } //InstanceImage

                        [XmlElement]
                        public string source;

                        [XmlElement]
                        public InstanceImage instance_image = new InstanceImage();

                        [XmlElement]
                        public string wrap_s = "CLAMP";

                        [XmlElement]
                        public string wrap_t = "CLAMP";

                        [XmlElement]
                        public string minfilter = "NEAREST_MIPMAP_NEAREST";

                        [XmlElement]
                        public string magfilter = "LINEAR";
                    } //Sampler2D

                    [XmlAttribute]
                    public string sid;

                    [XmlElement]
                    public Surface surface = new Surface();

                    [XmlElement]
                    public Sampler2D sampler2D = new Sampler2D();
                } //Newparam

                [XmlType("technique")]
                public class Technique
                {
                    [XmlType("phong")]
                    public class Phong
                    {
                        [XmlType("diffuse")]
                        public class Diffuse
                        {
                            [XmlType("texture")]
                            public class Texture
                            {
                                [XmlAttribute]
                                public string texture;

                                [XmlAttribute]
                                public string texcoord;
                            } //Texture

                            [XmlElement]
                            public Texture texture = new Texture();
                        } //Diffuse

                        [XmlElement]
                        public Diffuse diffuse = new Diffuse();
                    } //Phong

                    [XmlAttribute]
                    public string sid = "COMMON";

                    [XmlElement]
                    public Phong phong = new Phong();
                } //Technique

                //THIS SECTION NEEDS FIXED!
                [XmlElement("newparam")]
                public Newparam newparam0 = new Newparam();

                [XmlElement("newparam0")]
                public Newparam newparam1 = new Newparam();

                [XmlElement("technique")]
                public Technique technique = new Technique();
            } //ProfileCommon

            [XmlAttribute]
            public string id;

            [XmlAttribute]
            public string name;

            [XmlElement]
            public ProfileCommon profile_COMMON = new ProfileCommon();
        } //Effect
        
        /****************************************************************
         * 
         * VARIABLES
         * 
         ****************************************************************/
        [XmlAttribute]
        public string version = "1.5.0";

        [XmlElement]
        public Asset asset = new Asset();

        [XmlArray]
        public List<Image> library_images = new List<Image>(0);

        [XmlArray]
        public List<Material> library_materials = new List<Material>(0);

        [XmlArray]
        public List<Effect> library_effects = new List<Effect>(0);


        /****************************************************************
         * 
         * CONSTRUCTOR
         * Currently just contains code for testing purposes.
         * 
         ****************************************************************/
        public COLLADA()
        {
            //Library Images
            Image image = new Image();
            image.id = "id-test";
            image.name = "name-test";
            image.init_from.reference = "ref-test.png";

            library_images.Add(image);

            //Library Materials
            Material material = new Material();
            material.id = "id-test";
            material.instance_effect.url = "#url-test";

            library_materials.Add(material);

            //Library Effects
            Effect effect = new Effect();
            effect.id = "id-test";
            effect.name = "name-test";
            effect.profile_COMMON.newparam0.sid = "sid-test";
            effect.profile_COMMON.newparam0.surface = new Effect.ProfileCommon.Newparam.Surface();
            effect.profile_COMMON.newparam0.surface.type = "2D";
            effect.profile_COMMON.newparam0.surface.init_from.reference = "ref-test";
            effect.profile_COMMON.newparam1.sid = "sid-test";
            effect.profile_COMMON.newparam1.sampler2D.source = "source-test";
            effect.profile_COMMON.newparam1.sampler2D.instance_image.url = "#url-test";
            effect.profile_COMMON.technique.sid = "sid-test";
            effect.profile_COMMON.technique.phong.diffuse.texture.texture = "texture-test";
            effect.profile_COMMON.technique.phong.diffuse.texture.texcoord = "TEXCOORD0";

            library_effects.Add(effect);
        }
    } //COLLADA
} //namespace
