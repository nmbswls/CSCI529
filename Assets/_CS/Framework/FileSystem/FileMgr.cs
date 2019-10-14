using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveData
{

}

public class FileMgr
{
    public static void ReadFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Save1.save");
        bf.Serialize(file, new SaveData());
        file.Close();
    }

    //public static void ReadFile()
    //{
    //    if (File.Exists(Application.persistentDataPath + "/Save1.save"))
    //    {
    //        //重置场景

    //        // 读取数据
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Open(Application.persistentDataPath + "/Save1.save", FileMode.Open);
    //        SaveLoad sl = (SaveLoad)bf.Deserialize(file);
    //        file.Close();

    //        //加载场景

    //    }
    //    else
    //    {
    //        Debug.Log("No game Save1!");
    //    }
    //}
}
