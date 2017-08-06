using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class Hashing
{
    static List<string> stringDictionary = new List<string>(0);
    static List<ulong> hashDictionary = new List<ulong>(0);

    /*
     * HashFileNameLegacy
     * Hashes a given string and outputs the ulong result.
     */
    public static ulong HashFileNameLegacy(string text, bool removeExtension = true)
    {
        if (removeExtension)
        {
            int index = text.IndexOf('.');
            text = index == -1 ? text : text.Substring(0, index);
        }

        const ulong seed0 = 0x9ae16a3b2f90404f;
        ulong seed1 = text.Length > 0 ? (uint)((text[0]) << 16) + (uint)text.Length : 0;
        return CityHash.CityHash.CityHash64WithSeeds(text + "\0", seed0, seed1) & 0xFFFFFFFFFFFF;
    } //HashFileNameLegacy

    public static void ReadDictionary(string path)
    {
        foreach (string line in File.ReadAllLines(path))
        {
            stringDictionary.Add(line);
            hashDictionary.Add(HashFileNameLegacy(line));
        } //foreach
    } //ReadDictionary

    public static string TryGetName(ulong hash)
    {
        for (int i = 0; i < hashDictionary.Count; i++)
            if (hash == hashDictionary[i])
                return stringDictionary[i];

        return hash.ToString("x");
    } //TryGetName
} //class
