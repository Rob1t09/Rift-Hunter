using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "Rift Hunter/LocationData")]
public class LocationData : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;
    [SerializeField] private string displayName;
    public string DisplayName => displayName;
    [SerializeField] private int gridSize;
    public int GridSize => gridSize;
    [SerializeField] private RoomSet roomSet;
    public RoomSet RoomSet => roomSet;
    [SerializeField] private List<LocationData> requiredLocation = new List<LocationData>();
    public List<LocationData> RequiredLocation => requiredLocation;
    [SerializeField] private int minDifficulty;
    public int MinDifficulty => minDifficulty;

}