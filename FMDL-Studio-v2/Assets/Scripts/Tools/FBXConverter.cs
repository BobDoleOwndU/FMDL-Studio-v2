using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FBXConverter
{
    static int materialId = 800000000;
    static int geometryId = 900000000;
    static int modelId = 1000000001;
    static List<Tuple<int, SkinnedMeshRenderer>> meshes = new List<Tuple<int, SkinnedMeshRenderer>>(0);
    static List<Tuple<int, GameObject>> objects = new List<Tuple<int, GameObject>>(0);
    static List<Tuple<int, int>> geometryIds = new List<Tuple<int, int>>(0);
    static List<Tuple<int, string>> materials = new List<Tuple<int, string>>(0);

    public static void ConvertToFBX(GameObject gameObject)
    {
        int numModelObjects = 1;
        StringBuilder fbx = new StringBuilder();

        GetNumObjects(gameObject.transform, ref numModelObjects);

        objects.Add(new Tuple<int, GameObject>(1000000000, gameObject));

        GetObjects(gameObject.transform);

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
        fbx.AppendFormat("\n\t\tYear: {0}", DateTime.Now.Year);
        fbx.AppendFormat("\n\t\tMonth: {0}", DateTime.Now.Month);
        fbx.AppendFormat("\n\t\tDay: {0}", DateTime.Now.Day);
        fbx.AppendFormat("\n\t\tHour: {0}", DateTime.Now.Hour);
        fbx.AppendFormat("\n\t\tMinute: {0}", DateTime.Now.Minute);
        fbx.AppendFormat("\n\t\tSecond: {0}", DateTime.Now.Second);
        fbx.AppendFormat("\n\t\tMillisecond: {0}", DateTime.Now.Millisecond);
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
        fbx.AppendFormat("\n\tCount: {0}", numModelObjects + meshes.Count + materials.Count + 1);
        fbx.Append("\n\tObjectType: \"GlobalSettings\" {");
        fbx.Append("\n\t\tCount: 1");
        fbx.Append("\n\t}");
        fbx.Append("\n\tObjectType: \"Model\" {");
        fbx.AppendFormat("\n\t\tCount: {0}", numModelObjects);
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
        fbx.AppendFormat("\n\t\tCount: {0}", meshes.Count);
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
        fbx.AppendFormat("\n\t\tCount: {0}", materials.Count);
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

        //Object Properties
        fbx.Append("\n; Object properties");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nObjects:  {");
        for(int i = 0; i < meshes.Count; i++)
        {
            fbx.AppendFormat("\n\tGeometry: {0}, \"Geometry::Scene\", \"Mesh\" {{", geometryIds[i].Item1);
            fbx.AppendFormat("\n\t\tVertices: *{0} {{", meshes[i].Item2.sharedMesh.vertices.Length * 3);
            fbx.Append("\n\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.vertices.Length; j++)
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.vertices[j].x * 100, (double)meshes[i].Item2.sharedMesh.vertices[j].y * 100, (double)meshes[i].Item2.sharedMesh.vertices[j].z * 100);
            fbx.Length--;
            fbx.Append("\n\t\t}");
            fbx.AppendFormat("\n\t\tPolygonVertexIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], (-meshes[i].Item2.sharedMesh.triangles[j * 3 + 1] - 1));
            fbx.Length--;
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tGeometryVersion: 124");
            fbx.Append("\n\t\tLayerElementNormal: 0 {");
            fbx.Append("\n\t\t\tVersion: 102");
            fbx.Append("\n\t\t\tName: \"Normals\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
            fbx.AppendFormat("\n\t\t\tNormals: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length * 3);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
            {
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].x, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].y, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].z);
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].x, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].y, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].z);
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].x, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].y, (double)meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].z);
            } //for
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tNormalsW: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length; j++)
                fbx.Append("1,");
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            //Binormals are supposed to go here. Don't think they're needed though.
            fbx.Append("\n\t\tLayerElementTangent: 0 {");
            fbx.Append("\n\t\t\tVersion: 102");
            fbx.Append("\n\t\t\tName: \"Tangents\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
            fbx.AppendFormat("\n\t\t\tTangents: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length * 3);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
            {
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].x, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].y, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].z);
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].x, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].y, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].z);
                fbx.AppendFormat("{0},{1},{2},", (double)-meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].x, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].y, (double)meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].z);
            } //for
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tTangentsW: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].w, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].w, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].w);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tLayerElementColor: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"VertexColors\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.AppendFormat("\n\t\t\tColorIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tLayerElementUV: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"UVSet0\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.AppendFormat("\n\t\t\tUV: *{0} {{", meshes[i].Item2.sharedMesh.uv.Length * 2);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.uv.Length; j++)
                fbx.AppendFormat("{0},{1},", (double)meshes[i].Item2.sharedMesh.uv[j].x, (double)meshes[i].Item2.sharedMesh.uv[j].y);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tLayerElementMaterial: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"Material\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"AllSame\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.Append("\n\t\t\tMaterials: *1 {");
            fbx.Append("\n\t\t\t\ta: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tLayer: 0 {");
            fbx.Append("\n\t\t\tVersion: 100");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementNormal\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            //Binormals are supposed to go here. Don't think they're necessary though.
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementTangent\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementMaterial\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementColor\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t}");
        } //for
        for (int i = 0; i < objects.Count; i++)
        {
            fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Null\" {{", objects[i].Item1, objects[i].Item2.name);
            fbx.Append("\n\t\tVersion: 232");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            if(objects[i].Item2.transform.localPosition.x != 0 || objects[i].Item2.transform.localPosition.y != 0 || objects[i].Item2.transform.localPosition.z != 0)
                fbx.AppendFormat("\n\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",{0},{1},{2}", (double)-objects[i].Item2.transform.localPosition.x * 100, (double)objects[i].Item2.transform.localPosition.y * 100, (double)objects[i].Item2.transform.localPosition.z * 100);
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tShading: Y");
            fbx.Append("\n\t\tCulling: \"CullingOff\"");
            fbx.Append("\n\t}");
        } //for
        for (int i = 0; i < meshes.Count; i++)
        {
            fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Mesh\" {{", meshes[i].Item1, meshes[i].Item2.name);
            fbx.Append("\n\t\tVersion: 232");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tShading: W");
            fbx.Append("\n\t\tCulling: \"CullingOff\"");
            fbx.Append("\n\t}");
        } //for
        for(int i = 0; i < materials.Count; i++)
        {
            fbx.AppendFormat("\n\tMaterial: {0}, \"Material::{1}\", \"\" {{", materials[i].Item1, materials[i].Item2);
            fbx.Append("\n\t\tVersion: 102");
            fbx.Append("\n\t\tShadingModel: \"lambert\"");
            fbx.Append("\n\t\tMultiLayer: 0");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0,0,0");
            fbx.Append("\n\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",1,1,1");
            fbx.Append("\n\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",0");
            fbx.Append("\n\t\t\tP: \"Emissive\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"Ambient\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"Diffuse\", \"Vector3D\", \"Vector\", \"\",1,1,1");
            fbx.Append("\n\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",1");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t}");
        } //for
        fbx.Append("\n}");

        //Object Connections
        fbx.Append("\n\n; Object connections");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nConnections:  {");

        fbx.AppendFormat("\n\n\t;Model::{0}, Model::RootNode", objects[0].Item2.name);
        fbx.AppendFormat("\n\tC: \"OO\",{0},0", objects[0].Item1);

        for (int i = 1; i < objects.Count; i++)
        {
            int parentId = 0;

            for(int j = 0; j < objects.Count; j++)
            {
                if (objects[i].Item2.transform.parent.name == objects[j].Item2.name)
                    parentId = objects[j].Item1;
            } //for

            fbx.AppendFormat("\n\n\t;Model::{0}, Model::{1}", objects[i].Item2.name, objects[i].Item2.transform.parent.name);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", objects[i].Item1, parentId);
        } //for
        for (int i = 0; i < meshes.Count; i++)
        {
            int parentId = 0;

            for (int j = 0; j < objects.Count; j++)
            {
                if (meshes[i].Item2.transform.parent.name == objects[j].Item2.name)
                    parentId = objects[j].Item1;
            } //for

            fbx.AppendFormat("\n\n\t;Model::{0}, Model::{1}", meshes[i].Item2.name, meshes[i].Item2.transform.parent.name);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", meshes[i].Item1, parentId);
        } //for
        for (int i = 0; i < meshes.Count; i++)
        {
            int materialId = 0;

            for (int j = 0; j < materials.Count; j++)
            {
                if (meshes[i].Item2.material.name == materials[j].Item2)
                    materialId = materials[j].Item1;
            } //for

            fbx.AppendFormat("\n\n\t;Material::{0}, Model::{1}", meshes[i].Item2.material.name, meshes[i].Item2.name);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", materialId, meshes[i].Item1);
        } //for
        for (int i = 0; i < geometryIds.Count; i++)
        {
            string meshName = "";

            for (int j = 0; j < meshes.Count; j++)
            {
                if (geometryIds[i].Item2 == meshes[j].Item1)
                    meshName = meshes[j].Item2.name;
            } //for

            fbx.AppendFormat("\n\n\t;Geometry::Scene, Model::{0}", meshName);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", geometryIds[i].Item1, geometryIds[i].Item2);
        } //for

        //Takes
        fbx.Append("\n}");
        fbx.Append("\n;Takes section");
        fbx.Append("\n;----------------------------------------------------");
        fbx.Append("\n\nTakes:  {");
        fbx.Append("\n\tCurrent: \"\"");
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
        } //foreach
    } //GetNumObjects

    private static void GetObjects(Transform transform)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(new Tuple<int, SkinnedMeshRenderer>(modelId, t.gameObject.GetComponent<SkinnedMeshRenderer>()));
                geometryIds.Add(new Tuple<int, int>(geometryId, modelId));
                geometryId++;

                bool addMaterial = true;

                for (int i = 0; i < materials.Count; i++)
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().material.name == materials[i].Item2)
                        addMaterial = false;

                if (addMaterial)
                {
                    materials.Add(new Tuple<int, string>(materialId++, t.gameObject.GetComponent<SkinnedMeshRenderer>().material.name));
                    materialId++;
                } //if
            } //if
            else
            {
                objects.Add(new Tuple<int, GameObject>(modelId, t.gameObject));
            } //else

            modelId++;

            GetObjects(t);
        } //foreach
    } //GetMeshesAndMaterials
} //class
