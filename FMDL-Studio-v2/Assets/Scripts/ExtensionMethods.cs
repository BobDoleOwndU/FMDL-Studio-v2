using UnityEngine;

public static class ExtensionMethods
{
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
