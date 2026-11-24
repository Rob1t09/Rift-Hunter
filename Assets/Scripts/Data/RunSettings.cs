using UnityEngine;

public static class RunSettings
{
    public static LocationData CurrentLocation { get; private set; }
    public static int SelectedDifficulty { get; private set; }


    public static void Set(LocationData location, int difficulty)
    {
        CurrentLocation = location;
        SelectedDifficulty = difficulty;
    }
}
