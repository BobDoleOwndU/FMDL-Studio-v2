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
    } //class
} //namespace