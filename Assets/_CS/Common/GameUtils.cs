using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GameUtils
{


    public static byte[] ReadAllBytesFromFile(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            //logger
            return null;
        }
        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes(fullPath);
        }
        catch (Exception ex)
        {
            //logger
            bytes = null;
        }
        return bytes;
    }

    public static Texture2D GetTextureFromBytes(byte[] bytes)
    {
        int width = 4;
        int height = 4;
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        tex.name = "nmb";
        tex.LoadImage(bytes);

        return tex;
    }

}
