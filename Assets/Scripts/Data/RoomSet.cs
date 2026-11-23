using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSet", menuName = "Rift Hunter/RoomSet")]
public class RoomSet : ScriptableObject
{
    [SerializeField] private List<Room> rooms = new List<Room>();
    public List<Room> Rooms => rooms;
}
