using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int size = 3;
    public List<Room> rooms = new List<Room>();

    private int pathValue;
    private int[,] generatorGrid;
    private List<Room> startRoomList = new List<Room>();
    private List<Room> endRoomList = new List<Room>();
    private List<Room> normalRoomList = new List<Room>();
    private HashSet<Room> usedRoomList = new HashSet<Room>();


    //TODO:
    //TryPlace:
    //  rotace
    //  otvirani dveri
    //obecne:
    //  dalsi mistnosti krome spawn
    private void Awake()
    {
        pathValue = Random.Range(size, Mathf.FloorToInt(size*2.5f));
        generatorGrid = new int[size, size];


        foreach (Room room in rooms)
        {
            switch (room.type)
            {
                case Room.RoomType.Start:
                    startRoomList.Add(room);
                    break;
                case Room.RoomType.End:
                    endRoomList.Add(room);
                    break;
                case Room.RoomType.Normal:
                    normalRoomList.Add(room);
                    break;
            }
        }


        GenPath();
    }


    private void GenPath()
    {
        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            Restart();

            if (!StartRoom()) continue;
            // if (!NormalRooms()) continue;
            // if (!EndRoom()) continue;

            return;
        }

        Debug.LogError("Generování se nepodaøilo ani po " + attempts + " pokusech.");
    }

    private bool StartRoom()
    {
        int posX = Random.Range(0, generatorGrid.GetLength(0));
        int posY = Random.Range(0, generatorGrid.GetLength(1));

        //novy typ spawnovani
        return PlaceRoom(startRoomList, new Vector2Int(posX, posY)) != null;
    }

    private Room PlaceRoom(List<Room> roomList, Vector2Int position)
    {
        List<Room> candidates = roomList.Where(r => !usedRoomList.Contains(r)).OrderBy(r => Random.value).ToList();
        Room placed = null;

        foreach (Room candidate in candidates)
        {
            placed = TryPlace(candidate, position, generatorGrid);
            if (placed != null)
            {
                usedRoomList.Add(candidate);
                return placed;
            }
        }

        return null;
    }

    private Room TryPlace(Room room, Vector2Int start, int[,] grid)
    {

        //prvni cell mistnosti kde se budou propojovat
        List<Vector2Int> cells = new List<Vector2Int>(room.localCells).OrderBy(r => Random.value).ToList();

        Vector2 center = new Vector2(
                (room.localCells.Min(v => v.x) + room.localCells.Max(v => v.x)) / 2f,
                (room.localCells.Min(v => v.y) + room.localCells.Max(v => v.y)) / 2f
                                     );

        foreach (Vector2Int cell in cells)
        {
            Vector2Int offset = start - cell;
            bool badCell = false;

            //start grid pozice kde bude prvni cell

            //prvni cell mistnosti kde se budou propojovat

            foreach (Vector2 localCell in room.localCells)
            {
                int posX = Mathf.RoundToInt(localCell.x + offset.x);
                int posY = Mathf.RoundToInt(localCell.y + offset.y);
                if (
                    posX < 0 || posX >= grid.GetLength(0) || //jestli je mimo grid
                    posY < 0 || posY >= grid.GetLength(1) ||
                    grid[posX, posY] != 0 //jestli neni obsazena
                    )
                {
                    badCell = true;
                    break;
                }
            }

            if (badCell) continue;


            //melo by jit dal jen kdyz by zadna cast roomek nevypadla z mapy/nebyla na jiny roomce
            foreach (Vector2Int localCell in room.localCells)
            {
                grid[localCell.x + offset.x, localCell.y + offset.y] = 1;
            }


            Vector3 roomPosition = new Vector3(
                (center.x + offset.x) * GridSettings.Cell,
                0,
                (center.y + offset.y) * GridSettings.Cell
            );

            Room instance = Instantiate(room, roomPosition, Quaternion.identity, transform);
            return instance;

            //pokud je vsechno v pohode tak se odesle room a vytvori se ve svete nova roomka
        }
        //pokud ne tak se posle null a udela se dalsi pokus
        return null;
    }



    private List<int> RandomDirections()
    {
        List<int> directions1 = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            directions1.Add(i*90);
        }
        List<int> directions2 = new List<int>(directions1).OrderBy(r => Random.value).ToList();
        return directions2;
    }

    private Vector2 RotationToVector(Vector2 position, Quaternion rotation)
    {
        Vector3 pos = new Vector3(position.x, 0, position.y);
        Vector3 rotated = rotation * pos;

        return new Vector2(rotated.x, rotated.z);
    }

    private void Restart()
    {
        Debug.Log("restartuje se cesta");


        System.Array.Clear(generatorGrid, 0, generatorGrid.Length);
        usedRoomList.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}