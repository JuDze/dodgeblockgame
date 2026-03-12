using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
        Debug.Log("Game saved to " + Path);
    }

    public static SaveData Load()
    {
        if (!File.Exists(Path)) return null;
        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave() => File.Exists(Path);

    public static void Delete() => File.Delete(Path);

}