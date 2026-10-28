using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        End,
        Normal
    }

    public RoomType type = RoomType.Normal;
    public int value = 1; //grid value: 1x1 = 1, 2x2 = 4,
    public float weight = 1f; //jak moc casto se vybere, zatim nevyuzito (vsude 1)
    public List<Transform> cells = new List<Transform>(); //znacky objektu (bunek)
    public List<Vector2Int> localCells = new List<Vector2Int>(); //souradnice bunek
    public List<RoomSlot> roomSlots = new List<RoomSlot>();


    [ContextMenu("Naplnit")]
    public void Fill()
    {
        roomSlots.Clear();
        localCells.Clear();

        roomSlots.AddRange(GetComponentsInChildren<RoomSlot>(true));
        if (cells.Count == 0)
        {
            Debug.LogWarning("Room " + name + " has no cells assigned.");
            return;
        }

        //firstCellLocal: transform.InverseTransformPoint(cells[0].position);
        foreach (Transform marker in cells)
        {
            
        }

    }
}
