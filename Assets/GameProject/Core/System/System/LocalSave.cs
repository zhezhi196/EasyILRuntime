using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Module;
using UnityEngine;

public static class LocalSave
{
    public const string savePath = "SaveData";
    public const string save2Path = "SaveData2";

    public static bool hasSave
    {
        get { return File.Exists($"{Application.persistentDataPath}/{savePath}"); }
    }

    private static string GetKey(string key)
    {
        return BattleController.Instance.missionId + key;
    }

    public static string Read(ILocalSave save)
    {
        return LocalSaveFile.GetString(save.localFileName, GetKey(save.localUid));
    }

    public static string Read(string path , string save)
    {
        return LocalSaveFile.GetString(path, GetKey(save));
    }

    public static string[] ReadGroup(string fileName , string group)
    {
        return LocalSaveFile.GetGroup(fileName, group);
    }

    public static void Write(ILocalSave save)
    {
        Write(save.localFileName , save.localUid, save.GetWriteDate(), save.localGroup);
    }

    public static void Write(string fileName , string key, string writeData, string group)
    {
        LocalSaveFile.SetString(fileName, new LocalSaveInfo() {@group = group, key = GetKey(key), value = writeData});
    }

    public static void Delete(string fileName , string key)
    {
        LocalSaveFile.DeleteKey(fileName, key);
    }

    public static void DeleteFile()
    {
        if (hasSave)
        {
            LocalSaveFile.DeleteFile(savePath);
        }
    }

    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        LocalSaveFile.InitLocalFile(savePath);
        LocalSaveFile.InitLocalFile(save2Path);
        
        return process;
    }

    public static void DeleteGroup(string fileName , string group)
    {
        LocalSaveFile.DeleteGroup(fileName, group);
    }
}