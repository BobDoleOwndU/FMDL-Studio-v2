using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class Hashing
{
    static List<string> stringDictionary = new List<string>(0);
    static List<ulong> stringHashDictionary = new List<ulong>(0);

    static List<string> pathDictionary = new List<string>(0);
    static List<ulong> pathHashDictionary = new List<ulong>(0);

    public const ulong MetaFlag = 0x4000000000000;

    private static readonly List<string> FileExtensions = new List<string>
    {
        "1.ftexs",
        "1.nav2",
        "2.ftexs",
        "3.ftexs",
        "4.ftexs",
        "5.ftexs",
        "6.ftexs",
        "ag.evf",
        "aia",
        "aib",
        "aibc",
        "aig",
        "aigc",
        "aim",
        "aip",
        "ait",
        "atsh",
        "bnd",
        "bnk",
        "cc.evf",
        "clo",
        "csnav",
        "dat",
        "des",
        "dnav",
        "dnav2",
        "eng.lng",
        "ese",
        "evb",
        "evf",
        "fag",
        "fage",
        "fago",
        "fagp",
        "fagx",
        "fclo",
        "fcnp",
        "fcnpx",
        "fdes",
        "fdmg",
        "ffnt",
        "fmdl",
        "fmdlb",
        "fmtt",
        "fnt",
        "fova",
        "fox",
        "fox2",
        "fpk",
        "fpkd",
        "fpkl",
        "frdv",
        "fre.lng",
        "frig",
        "frt",
        "fsd",
        "fsm",
        "fsml",
        "fsop",
        "fstb",
        "ftex",
        "fv2",
        "fx.evf",
        "fxp",
        "gani",
        "geom",
        "ger.lng",
        "gpfp",
        "grxla",
        "grxoc",
        "gskl",
        "htre",
        "info",
        "ita.lng",
        "jpn.lng",
        "json",
        "lad",
        "ladb",
        "lani",
        "las",
        "lba",
        "lng",
        "lpsh",
        "lua",
        "mas",
        "mbl",
        "mog",
        "mtar",
        "mtl",
        "nav2",
        "nta",
        "obr",
        "obrb",
        "parts",
        "path",
        "pftxs",
        "ph",
        "phep",
        "phsd",
        "por.lng",
        "qar",
        "rbs",
        "rdb",
        "rdf",
        "rnav",
        "rus.lng",
        "sad",
        "sand",
        "sani",
        "sbp",
        "sd.evf",
        "sdf",
        "sim",
        "simep",
        "snav",
        "spa.lng",
        "spch",
        "sub",
        "subp",
        "tgt",
        "tre2",
        "txt",
        "uia",
        "uif",
        "uig",
        "uigb",
        "uil",
        "uilb",
        "utxl",
        "veh",
        "vfx",
        "vfxbin",
        "vfxdb",
        "vnav",
        "vo.evf",
        "vpc",
        "wem",
        "xml"
    };

    private static readonly Dictionary<ulong, string> ExtensionsMap = FileExtensions.ToDictionary(HashFileExtension);

    public static ulong HashFileExtension(string fileExtension) //from private to public
    {
        return HashFileName(fileExtension, false) & 0x1FFF;
    } //HashFileExtension

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

    public static ulong HashFileName(string text, bool removeExtension = true)
    {
        if (removeExtension)
        {
            int index = text.IndexOf('.');
            text = index == -1 ? text : text.Substring(0, index);
        }

        bool metaFlag = false;
        const string assetsConstant = "/Assets/";
        if (text.StartsWith(assetsConstant))
        {
            text = text.Substring(assetsConstant.Length);

            if (text.StartsWith("tpptest"))
            {
                metaFlag = true;
            }
        }
        else
        {
            metaFlag = true;
        }

        text = text.TrimStart('/');

        const ulong seed0 = 0x9ae16a3b2f90404f;
        byte[] seed1Bytes = new byte[sizeof(ulong)];
        for (int i = text.Length - 1, j = 0; i >= 0 && j < sizeof(ulong); i--, j++)
        {
            seed1Bytes[j] = Convert.ToByte(text[i]);
        }
        ulong seed1 = BitConverter.ToUInt64(seed1Bytes, 0);
        ulong maskedHash = CityHash.CityHash.CityHash64WithSeeds(text, seed0, seed1) & 0x3FFFFFFFFFFFF;

        return metaFlag
            ? maskedHash | MetaFlag
            : maskedHash;
    } //HashFileName

    public static ulong HashFileNameWithExtension(string filePath)
    {
        filePath = DenormalizeFilePath(filePath);
        string hashablePart;
        string extensionPart;
        int extensionIndex = filePath.IndexOf(".", StringComparison.Ordinal);
        if (extensionIndex == -1)
        {
            hashablePart = filePath;
            extensionPart = "";
        }
        else
        {
            hashablePart = filePath.Substring(0, extensionIndex);
            extensionPart = filePath.Substring(extensionIndex + 1, filePath.Length - extensionIndex - 1);
        }

        ulong typeId = 0;
        var extensions = ExtensionsMap.Where(e => e.Value == extensionPart).ToList();
        if (extensions.Count == 1)
        {
            var extension = extensions.Single();
            typeId = extension.Key;
        }
        ulong hash = HashFileName(hashablePart);
        hash = (typeId << 51) | hash;
        return hash;
    } //HashFileNameWithExtension

    private static string DenormalizeFilePath(string filePath)
    {
        return filePath.Replace("\\", "/");
    } //DenormalizeFilePath

    public static void ReadStringDictionary(string path)
    {
        stringDictionary.Clear();
        stringHashDictionary.Clear();

        foreach (string line in File.ReadAllLines(path))
        {
            stringDictionary.Add(line);
            stringHashDictionary.Add(HashFileNameLegacy(line));
        } //foreach
    } //ReadStringDictionary

	public static void ReadPathDictionary(string path)
    {
        pathDictionary.Clear();
        pathHashDictionary.Clear();

        foreach (string line in File.ReadAllLines(path))
        {
            pathDictionary.Add(line);
            pathHashDictionary.Add(HashFileNameWithExtension(line));
        } //foreach
    } //ReadPathDictionary

	public static string TryGetStringName(ulong hash)
    {
        for (int i = 0; i < stringHashDictionary.Count; i++)
            if (hash == stringHashDictionary[i])
                return stringDictionary[i];

        return hash.ToString("x");
    } //TryGetStringName

    public static string TryGetPathName(ulong hash)
    {
        for (int i = 0; i < pathHashDictionary.Count; i++)
            if (hash - 0x1568000000000000 == pathHashDictionary[i])
                return pathDictionary[i];

        return (hash - 0x1568000000000000).ToString("x");
    } //TryGetStringName
} //class
