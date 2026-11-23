using UnityEngine;

public static class SaveManager
{
    private static SaveData saveData = new SaveData();

    public static void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
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
        foreach(LocationData loc in location.RequiredLocation)
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
