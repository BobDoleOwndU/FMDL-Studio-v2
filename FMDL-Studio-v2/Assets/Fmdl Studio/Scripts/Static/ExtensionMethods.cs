using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
    public static class ExtensionMethods
    {
        //BinaryWriter
        public static void WriteZeroes(this BinaryWriter writer, int numZeroes)
        {
            byte[] bytes = new byte[numZeroes];
            writer.Write(bytes);
        } //WriteZeroes

        //List<Vector4>
        public static bool ContainsEqualValue(this List<Vector4> vector4s, Vector4 vector4)
        {
            foreach (Vector4 v in vector4s)
            {
                if (v.x == vector4.x && v.y == vector4.y && v.z == vector4.z && v.w == vector4.w)
                    return true;
            } //foreach

            return false;
        } //ContainsEqualValue

        public static int IndexOfEqualValue(this List<Vector4> vector4s, Vector4 vector4)
        {
            int vector4Count = vector4s.Count;

            for (int i = 0; i < vector4Count; i++)
            {
                if (vector4s[i].x == vector4.x && vector4s[i].y == vector4.y && vector4s[i].z == vector4.z && vector4s[i].w == vector4.w)
                    return i;
            } //foreach

            return -1;
        } //IndexOfEqualValue

        //Vector2[]
        public static bool IsNullOrZeroes(this Vector2[] array)
        {
            if (array == null)
                return true;

            for (int i = 0; i < array.Length; i++)
                if (array[i].x != 0 || array[i].y != 0)
                    return false;

            return true;
        } //IsNullOrZeroes

        //Vector3[]
        public static Vector3 Max(this Vector3[] array)
        {
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (Vector3 v in array)
            {
                if (v.x > max.x)
                    max.x = v.x;
                if (v.y > max.y)
                    max.y = v.y;
                if (v.z > max.z)
                    max.z = v.z;
            } //foreach

            return max;
        } //Max

        public static Vector3 Min(this Vector3[] array)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (Vector3 v in array)
            {
                if (v.x < min.x)
                    min.x = v.x;
                if (v.y < min.y)
                    min.y = v.y;
                if (v.z < min.z)
                    min.z = v.z;
            } //foreach

            return min;
        } //Max
    } //class
} //namespace