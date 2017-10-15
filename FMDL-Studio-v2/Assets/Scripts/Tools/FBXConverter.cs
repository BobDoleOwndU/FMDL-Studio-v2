using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FBXConverter
{
    public static void ConvertToFBX(GameObject gameObject)
    {
        int numModelObjects = 1;
        StringBuilder fbx = new StringBuilder();
        List<Mesh> meshes = new List<Mesh>(0);
        List<Material> materials = new List<Material>(0);

        GetNumObjects(gameObject.transform, ref numModelObjects);
        GetMeshesAndMaterials(gameObject.transform, meshes, materials);

        //Header
        fbx.Append("; FBX 7.4.0 project file");
        fbx.Append("\n; Copyright (C) 1997-2015 Autodesk Inc. and/or its licensors.");
        fbx.Append("\n; All rights reserved.");
        fbx.Append("\n; ----------------------------------------------------");
        fbx.Append("\n\nFBXHeaderExtension:  {");
        fbx.Append("\n\tFBXHeaderVersion: 1003");
        fbx.Append("\n\tFBXVersion: 7400");
        fbx.Append("\n\tCreationTimeStamp:  {");
        fbx.Append("\n\t\tVersion: 1000");
        fbx.Append("\n\t\tYear: " + DateTime.Now.Year);
        fbx.Append("\n\t\tMonth: " + DateTime.Now.Month);
        fbx.Append("\n\t\tDay: " + DateTime.Now.Day);
        fbx.Append("\n\t\tHour: " + DateTime.Now.Hour);
        fbx.Append("\n\t\tMinute: " + DateTime.Now.Minute);
        fbx.Append("\n\t\tSecond: " + DateTime.Now.Second);
        fbx.Append("\n\t\tMillisecond: " + DateTime.Now.Millisecond);
        fbx.Append("\n\t}");
        fbx.Append("\n\tCreator: \"Fmdl Studio v2\"");
        fbx.Append("\n\tSceneInfo: \"SceneInfo::GlobalInfo\", \"UserData\" {");
        fbx.Append("\n\t\tType: \"UserData\"");
        fbx.Append("\n\t\tVersion: 100");
        fbx.Append("\n\t\tMetaData:  {");
        fbx.Append("\n\t\t\tVersion: 100");
        fbx.Append("\n\t\t\tTitle: \"Unity to FBX converter\"");
        fbx.Append("\n\t\t\tSubject: \"\"");
        fbx.Append("\n\t\t\tAuthor: \"BobDoleOwndU and Joey35233\"");
        fbx.Append("\n\t\t\tKeywords: \"export fmdl fbx unity\"");
        fbx.Append("\n\t\t\tRevision: \"0.1\"");
        fbx.Append("\n\t\t\tComment: \"Development version. Do not expect working results.\"");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t\tProperties70:  {");
        fbx.Append("\n\t\t\tP: \"DocumentUrl\", \"KString\", \"Url\", \"\", \"<AppLocation>\"");
        fbx.Append("\n\t\t\tP: \"SrcDocumentUrl\", \"KString\", \"Url\", \"\", \"<AppLocation>\"");
        fbx.Append("\n\t\t\tP: \"Original\", \"Compound\", \"\", \"\"");
        fbx.Append("\n\t\t\tP: \"Original|ApplicationVendor\", \"KString\", \"\", \"\", \"BobDoleOwndU and Joey35233\"");
        fbx.Append("\n\t\t\tP: \"Original|ApplicationName\", \"KString\", \"\", \"\", \"Fmdl Studio v2\"");
        fbx.Append("\n\t\t\tP: \"Original|ApplicationVersion\", \"KString\", \"\", \"\", \"0.1\"");
        fbx.Append("\n\t\t\tP: \"Original|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
        fbx.Append("\n\t\t\tP: \"Original|FileName\", \"KString\", \"\", \"\", \"\"");
        fbx.Append("\n\t\t\tP: \"LastSaved\", \"Compound\", \"\", \"\"");
        fbx.Append("\n\t\t\tP: \"LastSaved|ApplicationVendor\", \"KString\", \"\", \"\", \"BobDoleOwndU and Joey35233\"");
        fbx.Append("\n\t\t\tP: \"LastSaved|ApplicationName\", \"KString\", \"\", \"\", \"Fmdl Studio v2\"");
        fbx.Append("\n\t\t\tP: \"LastSaved|ApplicationVersion\", \"KString\", \"\", \"\", \"0.1\"");
        fbx.Append("\n\t\t\tP: \"LastSaved|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t}");
        fbx.Append("\n}");

        //Global Settings
        fbx.Append("\nGlobalSettings:  {");
        fbx.Append("\n\tVersion: 1000");
        fbx.Append("\n\tProperties70:  {");
        fbx.Append("\n\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",1");
        fbx.Append("\n\t\tP: \"UpAxisSign\", \"int\", \"Integer\", \"\",1");
        fbx.Append("\n\t\tP: \"FrontAxis\", \"int\", \"Integer\", \"\",2");
        fbx.Append("\n\t\tP: \"FrontAxisSign\", \"int\", \"Integer\", \"\",1");
        fbx.Append("\n\t\tP: \"CoordAxis\", \"int\", \"Integer\", \"\",0");
        fbx.Append("\n\t\tP: \"CoordAxisSign\", \"int\", \"Integer\", \"\",1");
        fbx.Append("\n\t\tP: \"OriginalUpAxis\", \"int\", \"Integer\", \"\",-1");
        fbx.Append("\n\t\tP: \"OriginalUpAxisSign\", \"int\", \"Integer\", \"\",1");
        fbx.Append("\n\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",1");
        fbx.Append("\n\t\tP: \"OriginalUnitScaleFactor\", \"double\", \"Number\", \"\",1");
        fbx.Append("\n\t\tP: \"AmbientColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        fbx.Append("\n\t\tP: \"DefaultCamera\", \"KString\", \"\", \"\", \"Producer Perspective\"");
        fbx.Append("\n\t\tP: \"TimeMode\", \"enum\", \"\", \"\",0");
        fbx.Append("\n\t\tP: \"TimeProtocol\", \"enum\", \"\", \"\",2");
        fbx.Append("\n\t\tP: \"SnapOnFrameMode\", \"enum\", \"\", \"\",0");
        fbx.Append("\n\t\tP: \"TimeSpanStart\", \"KTime\", \"Time\", \"\",0");
        fbx.Append("\n\t\tP: \"TimeSpanStop\", \"KTime\", \"Time\", \"\",46186158000");
        fbx.Append("\n\t\tP: \"CustomFrameRate\", \"double\", \"Number\", \"\",-1");
        fbx.Append("\n\t\tP: \"TimeMarker\", \"Compound\", \"\", \"\"");
        fbx.Append("\n\t\tP: \"CurrentTimeMarker\", \"int\", \"Integer\", \"\",-1");
        fbx.Append("\n\t}");
        fbx.Append("\n}");

        //Documents Description
        fbx.Append("\n\n; Documents Description");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nDocuments:  {");
        fbx.Append("\n\tCount: 1");
        fbx.Append("\n\tDocument: 9999999999, \"Scene\", \"Scene\" {");
        fbx.Append("\n\t\tProperties70:  {");
        fbx.Append("\n\t\t\tP: \"SourceObject\", \"object\", \"\", \"\"");
        fbx.Append("\n\t\t\tP: \"ActiveAnimStackName\", \"KString\", \"\", \"\", \"\"");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t\tRootNode: 0");
        fbx.Append("\n\t}");
        fbx.Append("\n}");

        //Document References
        fbx.Append("\n\n; Document References");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nReferences:  {");
        fbx.Append("\n}");

        //Object Definitions
        fbx.Append("\n\n; Object definitions");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nDefinitions:  {");
        fbx.Append("\n\tVersion: 100");
        fbx.Append("\n\tCount: " + (numModelObjects + meshes.Count + materials.Count + 1).ToString());
        fbx.Append("\n\tObjectType: \"GlobalSettings\" {");
        fbx.Append("\n\t\tCount: 1");
        fbx.Append("\n\t}");
        fbx.Append("\n\tObjectType: \"Model\" {");
        fbx.Append("\n\t\tCount: " + numModelObjects);
        fbx.Append("\n\t\tPropertyTemplate: \"FbxNode\" {");
        fbx.Append("\n\t\t\tProperties70:  {");
        fbx.Append("\n\t\t\t\tP: \"QuaternionInterpolate\", \"enum\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"RotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"ScalingOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"ScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"TranslationActive\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMinX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMinY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMinZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMaxX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMaxY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"TranslationMaxZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationSpaceForLimitOnly\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationStiffnessX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationStiffnessY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationStiffnessZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"AxisLen\", \"double\", \"Number\", \"\",10");
        fbx.Append("\n\t\t\t\tP: \"PreRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"PostRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"RotationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"RotationMinX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMinY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMinZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMaxX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMaxY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"RotationMaxZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingActive\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",1,1,1");
        fbx.Append("\n\t\t\t\tP: \"ScalingMinX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMinY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMinZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMaxX\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMaxY\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"ScalingMaxZ\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"GeometricTranslation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"GeometricRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"GeometricScaling\", \"Vector3D\", \"Vector\", \"\",1,1,1");
        fbx.Append("\n\t\t\t\tP: \"MinDampRangeX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MinDampRangeY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MinDampRangeZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampRangeX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampRangeY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampRangeZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MinDampStrengthX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MinDampStrengthY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MinDampStrengthZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampStrengthX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampStrengthY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"MaxDampStrengthZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"PreferedAngleX\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"PreferedAngleY\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"LookAtProperty\", \"object\", \"\", \"\"");
        fbx.Append("\n\t\t\t\tP: \"UpVectorProperty\", \"object\", \"\", \"\"");
        fbx.Append("\n\t\t\t\tP: \"Show\", \"bool\", \"\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"NegativePercentShapeSupport\", \"bool\", \"\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",-1");
        fbx.Append("\n\t\t\t\tP: \"Freeze\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"LODBox\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",1,1,1");
        fbx.Append("\n\t\t\t\tP: \"Visibility\", \"Visibility\", \"\", \"A\",1");
        fbx.Append("\n\t\t\t\tP: \"Visibility Inheritance\", \"Visibility Inheritance\", \"\", \"\",1");
        fbx.Append("\n\t\t\t}");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t}");
        fbx.Append("\n\tObjectType: \"Geometry\" {");
        fbx.Append("\n\t\tCount: " + meshes.Count);
        fbx.Append("\n\t\tPropertyTemplate: \"FbxMesh\" {");
        fbx.Append("\n\t\t\tProperties70:  {");
        fbx.Append("\n\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
        fbx.Append("\n\t\t\t\tP: \"BBoxMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"BBoxMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"Primary Visibility\", \"bool\", \"\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"Casts Shadows\", \"bool\", \"\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"Receive Shadows\", \"bool\", \"\", \"\",1");
        fbx.Append("\n\t\t\t}");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t}");
        fbx.Append("\n\tObjectType: \"Material\" {");
        fbx.Append("\n\t\tCount: " + materials.Count);
        fbx.Append("\n\t\tPropertyTemplate: \"FbxSurfaceLambert\" {");
        fbx.Append("\n\t\t\tProperties70:  {");
        fbx.Append("\n\t\t\t\tP: \"ShadingModel\", \"KString\", \"\", \"\", \"Lambert\"");
        fbx.Append("\n\t\t\t\tP: \"MultiLayer\", \"bool\", \"\", \"\",0");
        fbx.Append("\n\t\t\t\tP: \"EmissiveColor\", \"Color\", \"\", \"A\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",1");
        fbx.Append("\n\t\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
        fbx.Append("\n\t\t\t\tP: \"AmbientFactor\", \"Number\", \"\", \"A\",1");
        fbx.Append("\n\t\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",0.8,0.8,0.8");
        fbx.Append("\n\t\t\t\tP: \"DiffuseFactor\", \"Number\", \"\", \"A\",1");
        fbx.Append("\n\t\t\t\tP: \"Bump\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"NormalMap\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"TransparencyFactor\", \"Number\", \"\", \"A\",0");
        fbx.Append("\n\t\t\t\tP: \"DisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"DisplacementFactor\", \"double\", \"Number\", \"\",1");
        fbx.Append("\n\t\t\t\tP: \"VectorDisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        fbx.Append("\n\t\t\t\tP: \"VectorDisplacementFactor\", \"double\", \"Number\", \"\",1");
        fbx.Append("\n\t\t\t}");
        fbx.Append("\n\t\t}");
        fbx.Append("\n\t}");
        fbx.Append("\n}");


        using (FileStream stream = new FileStream("debug.fbx", FileMode.Create))
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            writer.Write(fbx.ToString());
        } //using

        Debug.Log("Num Meshes: " + meshes.Count);
        Debug.Log("Num Materials: " + materials.Count);
    } //ConvertToFbx

    private static void GetNumObjects(Transform transform, ref int count)
    {
        foreach (Transform t in transform)
        {
            count++;
            GetNumObjects(t, ref count);
        } //foreach ends
    } //GetNumObjects

    private static void GetMeshesAndMaterials(Transform transform, List<Mesh> meshes, List<Material> materials)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh);

                bool addMaterial = true;

                for (int i = 0; i < materials.Count; i++)
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().material.name == materials[i].name)
                        addMaterial = false;

                if(addMaterial)
                    materials.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().material);
            } //if
        } //foreach ends
    } //GetMeshesAndMaterials
} //class
