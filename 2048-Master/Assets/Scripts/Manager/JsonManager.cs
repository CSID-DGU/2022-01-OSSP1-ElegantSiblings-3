using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.IO;

public class JsonManager
{
    public static string Serialize<T>(T obj) => JsonUtility.ToJson(obj, true);
    public static T Deserialize<T>(string serializedObj) => JsonUtility.FromJson<T>(serializedObj);

    public static void Write<T>(string path, T obj)
    {
        File.WriteAllText(path, JsonUtility.ToJson(obj, true));
    }

    public static void Delete(string path)
    {
        if (File.Exists(path)) File.Delete(path);     
    }

    public static T Read<T>(string path)
    {
        T obj = default(T);

        if (File.Exists(path))
        {
            obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
            if (obj == null) obj = default(T);
        }

        return obj;
    }
}


[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}