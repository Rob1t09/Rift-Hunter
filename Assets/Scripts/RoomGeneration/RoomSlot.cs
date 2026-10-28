using UnityEngine;

public class RoomSlot : MonoBehaviour
{
    public GameObject closedWall;
    public GameObject doorway;

    public Vector2Int localCell;
    public Vector2Int localDirection;

    //cist smi kdokoliv, zapis jen zde
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Close();
    }

    public void Open()
    {
        if (doorway != null) doorway.SetActive(true);
        if (closedWall != null) closedWall.SetActive(false);
        IsOpen = true;
    }
    public void Close()
    {
        if (doorway != null) doorway.SetActive(false);
        if (closedWall != null) closedWall.SetActive(true);
        IsOpen = false;
    }
}
