using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static SaveData saveData = new SaveData();

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void Load()
    {
        Debug.Log("Save file: " + SavePath);

        if (!File.Exists(SavePath))
        {
            saveData = new SaveData();
            Save();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save file se nepodaøilo naèíst, zaèíná se znovu: " + e.Message);
            saveData = null;
        }

        if (saveData == null)
        {
            saveData = new SaveData();
        }
    }

    public static void CompleteLocation(string locationId, int difficulty)
    {
        if (!saveData.finishedLocationIds.Contains(locationId))
        {
            saveData.finishedLocationIds.Add(locationId);
        }
        if (difficulty > saveData.highestFinishedDifficulty)
        {
            saveData.highestFinishedDifficulty = difficulty;
        }

        Save();
    }

    public static bool IsLocationCompleted(string locationId)
    {
        return saveData.finishedLocationIds.Contains(locationId);
    }

    public static bool IsLocationUnlocked(LocationData location)
    {
        foreach (LocationData loc in location.RequiredLocation)
        {
            if (!IsLocationCompleted(loc.Id))
            {
                return false;
            }
        }
        return true;
    }

    public static int GetMaxDifficulty()
    {
        int maxDifficulty = saveData.highestFinishedDifficulty + 2;
        if (maxDifficulty > 10)
        {
            maxDifficulty = 10;
        }
        return maxDifficulty;
    }

    public static void ResetSaveData()
    {
        saveData = new SaveData();
        Save();
    }
}