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
        value = cells.Count;

        roomSlots.AddRange(GetComponentsInChildren<RoomSlot>(true));
        if (cells.Count == 0)
        {
            Debug.LogWarning("Room " + name + " has no cells assigned." + this);
            return;
        }

        Vector3 firstCellLocal = transform.InverseTransformPoint(cells[0].position);
        foreach (Transform marker in cells)
        {
            Vector3 markerLocal = transform.InverseTransformPoint(marker.position);
            float gridX = (markerLocal.x - firstCellLocal.x) / GridSettings.Cell;
            float gridZ = (markerLocal.z - firstCellLocal.z) / GridSettings.Cell;

            int cellX = Mathf.RoundToInt(gridX);
            int cellZ = Mathf.RoundToInt(gridZ);

            if (Mathf.Abs(cellX-gridX) > 0.05 || Mathf.Abs(cellZ - gridZ) > 0.05f)
            {
                Debug.LogWarning(name + " " + marker.name + " " + this);
            }

            Vector2Int grid = new (cellX, cellZ);


            localCells.Add(grid);
        }


        foreach (RoomSlot slot in roomSlots)
        {
            //nejblizsi local cell
            Vector3 slotLocal = transform.InverseTransformPoint(slot.transform.position);
            Vector2 slotPosition = new Vector2(slotLocal.x, slotLocal.z);

            Vector2 closestCell = Vector2.zero;
            float closestDistance = float.MaxValue;
            int closestCellIndex = 0;

            Vector2 newCell;
            float newCellDistance;

            Vector3 markerLocal;

            for (int i = 0; i < cells.Count; i++)
            {
                markerLocal = transform.InverseTransformPoint(cells[i].position);
                newCell = new Vector2(markerLocal.x, markerLocal.z);
                newCellDistance = Vector2.Distance(slotPosition, newCell);

                if (newCellDistance < closestDistance)
                {
                    closestCell = newCell;
                    closestDistance = newCellDistance;
                    closestCellIndex = i;
                }
            }
            
            //vysledek je nejblizsi stred mistnosti k slotu
            slot.localCell = localCells[closestCellIndex];


            Vector2 slotDirection = slotPosition - closestCell;

            float expectedDistance = GridSettings.Cell / 2f - GridSettings.SlotInset;
            const float tolerance = 0.1f;

            float absX = Mathf.Abs(slotDirection.x);
            float absY = Mathf.Abs(slotDirection.y);

            //zjistuje ktera je delsi kdyby to nahodou nebylo primo [0 6.5]
            if (absX > absY)
            {
                //vychod (+) nebo zapad (-)
                int dirX;
                if (slotDirection.x > 0) dirX = 1;
                else dirX = -1;
                slot.localDirection = new Vector2Int(dirX, 0);
            }
            else
            {
                //sever (+) nebo jih (-)
                int dirY;
                if (slotDirection.y > 0) dirY = 1;
                else dirY = -1;
                slot.localDirection = new Vector2Int(0, dirY);
            }

            //kontrola, ze slot sedi uprostred hrany ve spravne vzdalenosti
            float high = Mathf.Max(absX, absY); //vetsi osa, ma byt expectedDistance
            float low = Mathf.Min(absX, absY); //mensi osa, ma byt skoro 0

            if (Mathf.Abs(high - expectedDistance) > tolerance || low > tolerance)
            {
                Debug.LogWarning(name + ": slot " + slot.name + " je spatne umisteny (vzdalenost od stredu bunky "
                    + high + ", ocekavano " + expectedDistance + ", vedlejsi osa " + low + ").", slot);
            }
        }
    }
}
