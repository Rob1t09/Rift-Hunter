using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class SaveData
{
    public List<string> finishedLocationIds = new List<string>();
    public int highestFinishedDifficulty = 0;
}
