using System;
using System.IO;
using UnityEngine;

public class JsonDataSaver
{
    public static void Save<T>(T dataToSave, string dataFileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "data", $"{dataFileName}.json");
        string jsonData = JsonUtility.ToJson(dataToSave);

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        File.WriteAllText(filePath, jsonData);
    }

    public static T Load<T>(string dataFileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "data", $"{dataFileName}.json");

        if (!Directory.Exists(Path.GetDirectoryName(filePath)) || !File.Exists(filePath))
            return default(T);

        var jsonData = File.ReadAllText(filePath);
        
        object resultValue = JsonUtility.FromJson<T>(jsonData);
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }
}