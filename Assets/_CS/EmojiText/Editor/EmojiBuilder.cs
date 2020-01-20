﻿/*

 Description:Create the Atlas of emojis and its data texture.

 How to use?
 1)
  Put all emojies in Asset/Framework/Resource/Emoji/Input.
  Multi-frame emoji name format : Name_Index.png , Single frame emoji format: Name.png
 2)
  Excute EmojiText->Build Emoji from menu in Unity.
 3)
  It will outputs two textures and a txt in Emoji/Output.
  Drag emoji_tex to "Emoji Texture" and emoji_data to "Emoji Data" in UGUIEmoji material.
 4)
  Repair the value of "Emoji count of every line" base on emoji_tex.png.
 5)
  It will auto copys emoji.txt to Resources, and you can overwrite relevant functions base on your project.
 
 Author:zouchunyi
 E-mail:zouchunyi@kingsoft.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EmojiBuilder
{

    private const string OutputPath = "Assets/EmojiTex/Output/";
    private const string InputPath = "/EmojiTex/Input/";

    private static readonly Vector2[] AtlasSize = new Vector2[]{
  new Vector2(32,32),
  new Vector2(64,64),
  new Vector2(128,128),
  new Vector2(256,256),
  new Vector2(512,512),
  new Vector2(1024,1024),
  new Vector2(2048,2048)
 };

    struct EmojiInfo
    {
        public string key;
        public string x;
        public string y;
        public string size;
    }
    private const int EmojiSize = 32;//the size of emoji.

    [MenuItem("EmojiText/Build Emoji")]
    public static void BuildEmoji()
    {
        List<char> keylist = new List<char>();
        for (int i = 48; i <= 57; i++)
        {
            keylist.Add(System.Convert.ToChar(i));//0-9
        }
        for (int i = 65; i <= 90; i++)
        {
            keylist.Add(System.Convert.ToChar(i));//A-Z
        }
        for (int i = 97; i <= 122; i++)
        {
            keylist.Add(System.Convert.ToChar(i));//a-z
        }

        //search all emojis and compute they frames.
        HashSet<string> sourceDic = new HashSet<string>();
        //Dictionary<string, int> sourceDic = new Dictionary<string, int>();
        string[] files = Directory.GetFiles(Application.dataPath + InputPath, "*.png");
        for (int i = 0; i < files.Length; i++)
        {
            string[] strs = files[i].Split('/');
            string[] strs2 = strs[strs.Length - 1].Split('.');
            string filename = strs2[0];//kiss_1
            sourceDic.Add(filename);

        }

        //create the directory if it is not exist.
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }

        Dictionary<string, EmojiInfo> emojiDic = new Dictionary<string, EmojiInfo>();

        int totalFrames = sourceDic.Count;//总帧数

        Vector2 texSize = ComputeAtlasSize(totalFrames);
        Texture2D newTex = new Texture2D((int)texSize.x, (int)texSize.y, TextureFormat.ARGB32, false);
        //Texture2D dataTex = new Texture2D((int)texSize.x / EmojiSize, (int)texSize.y / EmojiSize, TextureFormat.ARGB32, false);
        int x = 0;
        int y = 0;
        int keyindex = 0;
        foreach (string key in sourceDic)
        {



            string path = "Assets" + InputPath + key + ".png";

            Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);//加载小图
            //Debug.Log(asset.width);
            asset = ScaleTexture(asset, EmojiSize, EmojiSize);
            //asset.(EmojiSize,EmojiSize);
            Color[] colors = asset.GetPixels(0);

            for (int i = 0; i < EmojiSize; i++)
            {
                for (int j = 0; j < EmojiSize; j++)
                {
                    newTex.SetPixel(x + i, y + j, colors[i + j * EmojiSize]);//像素级拷贝到大图集里
                }
            }


            //写入到数据贴图里

            if (!emojiDic.ContainsKey(key))
            {
                EmojiInfo info;
                info.key = "["+key+"]";
                //if (keyindex < keylist.Count)
                //{
                //    info.key = "[" + char.ToString(keylist[keyindex]) + "]";
                //}
                //else//从0-9 a-z A-Z都用完了，就拼接2维的向量
                //{
                //    info.key = "[" + char.ToString(keylist[keyindex / keylist.Count]) + char.ToString(keylist[keyindex % keylist.Count]) + "]";
                //}
                info.x = (x * 1.0f / texSize.x).ToString();//计算成UV
                info.y = (y * 1.0f / texSize.y).ToString();//计算成UV
                info.size = (EmojiSize * 1.0f / texSize.x).ToString();//尺寸转成UV比例

                emojiDic.Add(key, info);
                keyindex++;
            }

            x += EmojiSize;
            if (x >= texSize.x)
            {
                x = 0;
                y += EmojiSize;
            }
            
        }

        byte[] bytes1 = newTex.EncodeToPNG();
        string outputfile1 = OutputPath + "emoji_tex.png";
        File.WriteAllBytes(outputfile1, bytes1);



        using (StreamWriter sw = new StreamWriter(OutputPath + "emoji.txt", false))
        {
            sw.WriteLine("Name\tKey\tX\tY\tSize");
            foreach (string key in emojiDic.Keys)
            {
                sw.WriteLine("{" + key + "}\t" + emojiDic[key].key + "\t" + emojiDic[key].x + "\t" + emojiDic[key].y + "\t" + emojiDic[key].size);
            }
            sw.Close();
        }

        File.Copy(OutputPath + "emoji.txt", "Assets/Resources/emoji.txt", true);

        AssetDatabase.Refresh();
        FormatTexture();

        EditorUtility.DisplayDialog("Success", "Generate Emoji Successful!", "OK");
    }

    /// <summary>
    /// 计算一下需要多大的图集才能装得下
    /// </summary>
    private static Vector2 ComputeAtlasSize(int count)
    {
        long total = count * EmojiSize * EmojiSize;
        for (int i = 0; i < AtlasSize.Length; i++)
        {
            if (total <= AtlasSize[i].x * AtlasSize[i].y)
            {
                return AtlasSize[i];
            }
        }
        return Vector2.zero;
    }

    private static void FormatTexture()
    {
        TextureImporter emojiTex = AssetImporter.GetAtPath(OutputPath + "emoji_tex.png") as TextureImporter;
        emojiTex.filterMode = FilterMode.Point;
        emojiTex.mipmapEnabled = false;

        emojiTex.sRGBTexture = true;
        emojiTex.alphaSource = TextureImporterAlphaSource.FromInput;
        emojiTex.textureCompression = TextureImporterCompression.Uncompressed;

        emojiTex.SaveAndReimport();


    }


    static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear(j * 1.0f / result.width, i * 1.0f / result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }

}