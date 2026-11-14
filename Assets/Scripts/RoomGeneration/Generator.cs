using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Transform Player;
    private Room startInstance;
    public int size = 4;
    public List<Room> rooms = new List<Room>();

    private int pathValue;
    private int[,] generatorGrid;
    
    //localDirection z roomslotu (otocit o 180 stupnu bc enter a exit jsou opaky)
    //jen prave dalsi + vsechny kde se muze udelat fork
    private List<(Vector2Int, Vector2Int, RoomSlot)> nextRoomSlotList = new List<(Vector2Int, Vector2Int, RoomSlot)>();
    private List<(Vector2Int, Vector2Int, RoomSlot)> forkRoomSlotList = new List<(Vector2Int, Vector2Int, RoomSlot)>();
    // pozice slotu v gridu ,  smer ven , roomslot     
    //  pozice  ,    smer   , roomslot    
    private List<Room> startRoomList = new List<Room>();
    private List<Room> endRoomList = new List<Room>();
    private List<Room> normalRoomList = new List<Room>();
    private HashSet<Room> usedRoomList = new HashSet<Room>();




    private void Awake()
    {
        //pathValue = Random.Range(size, Mathf.FloorToInt(size*2.5f));
        

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
        int attempts = 100;
        for (int i = 0; i < attempts; i++)
        {
            Restart();
            //start
            if (!StartRoom()) continue;
            //path
            bool pathFailed = false;
            while (pathValue > 0)
            {
                if (!NormalRooms(nextRoomSlotList)) { pathFailed = true; break; }
            }
            if (pathFailed) continue;
            //end
            if (!EndRoom()) continue;

            //forkovani
            while(!isGridFull(generatorGrid))
            {
                if (!NormalRooms(forkRoomSlotList)) break;
            }
            //zbyla dira v generaci -> restart
            if (!isGridFull(generatorGrid)) continue;

            MovePlayer();


            return;
        }

        Debug.LogError("Generování se nepodaøilo ani po " + attempts + " pokusech.");
    }

    private bool StartRoom()
    {
        startInstance = PlaceRoom(startRoomList, nextRoomSlotList);
        return startInstance != null;
    }
    private bool NormalRooms(List<(Vector2Int, Vector2Int, RoomSlot)> RoomSlotList)
    {
        Room placedRoom = PlaceRoom(normalRoomList, RoomSlotList);
        if (placedRoom == null)
        {
            return false;
        }
        pathValue = pathValue - placedRoom.value;
        return true;
    }
    private bool EndRoom()
    {
        return PlaceRoom(endRoomList, nextRoomSlotList) != null;
    }

    private Room PlaceRoom(List<Room> roomList, List<(Vector2Int, Vector2Int, RoomSlot)> RoomSlotList)
    {
        List<Room> candidates = roomList.Where(r => !usedRoomList.Contains(r)).OrderBy(r => Random.value).ToList();
        List<(Vector2Int, Vector2Int, RoomSlot)> shuffledRoomSlots = RoomSlotList.OrderBy(r => Random.value).ToList();
        Room placed = null;

        foreach (Room candidate in candidates)
        {
            foreach ((Vector2Int position, Vector2Int direction, RoomSlot slot) shuffledSlot in shuffledRoomSlots)
            {
                placed = TryPlace(candidate, shuffledSlot, RoomSlotList, generatorGrid);
                if (placed != null)
                {
                    usedRoomList.Add(candidate);
                    return placed;

                    
                }

            }
        }

        return null;
    }

    private Room TryPlace(Room room, (Vector2Int pos, Vector2Int dir, RoomSlot slot) shuffledSlot, List<(Vector2Int, Vector2Int, RoomSlot)> RoomSlotList, int[,] grid)
    {
        bool isStart = room.type == Room.RoomType.Start;
        Vector2Int targetCell = shuffledSlot.pos + shuffledSlot.dir;

        
        List<int> randomDir = RandomDirections();
        foreach (int angle in randomDir)
        {
            //prvni cell mistnosti kde se budou propojovat
            List<Vector2Int> newcells = new List<Vector2Int>();
            foreach (Vector2Int cell in room.localCells)
            {
                Vector2 rotatedCell = RotationToVector(cell, Quaternion.Euler(0, angle, 0));
                Vector2Int rotatedIntCell = new Vector2Int(Mathf.RoundToInt(rotatedCell.x), Mathf.RoundToInt(rotatedCell.y));
                newcells.Add(rotatedIntCell);
            }
            List<Vector2Int> cells = new List<Vector2Int>(newcells).OrderBy(r => Random.value).ToList();


            //stred prefabu pro placenuti
            Vector2 center = new Vector2(
                    (cells.Min(v => v.x) + cells.Max(v => v.x)) / 2f,
                    (cells.Min(v => v.y) + cells.Max(v => v.y)) / 2f
                                         );

            //moznosti umisteni = offset (pozice) + index vstupniho slotu (-1 = start, zadny vstup)
            List<(Vector2Int offset, int entryIndex)> options = new List<(Vector2Int, int)>();
            if (isStart)
            {
                //start muze byt kdekoliv
                foreach (Vector2Int cell in cells)
                {
                    options.Add((targetCell - cell, -1));
                }
            }
            else
            {
                //ostatni mistnosti musi sedet na slotu
                //ktery po otoceni miri opacne nez vychod predchozi
                for (int i = 0; i < room.roomSlots.Count; i++)
                {
                    RoomSlot slot = room.roomSlots[i];
                    if (RotateInt(slot.localDirection, angle) != -shuffledSlot.dir) continue;
                    options.Add((targetCell - RotateInt(slot.localCell, angle), i));
                }
            }


            foreach ((Vector2Int offset, int entryIndex) option in options.OrderBy(o => Random.value).ToList())
            {
                if (!CanPlace(cells, option.offset, grid)) continue;
                if (isStart && DoorsLookOutside(room, angle, option.offset, grid)) continue;
                //od ted je jiste ze se mistnost umisti
                foreach (Vector2Int cell in cells)
                {
                    grid[cell.x + option.offset.x, cell.y + option.offset.y] = 1;
                }

                Vector3 roomPosition = new Vector3(
                    (center.x + option.offset.x) * GridSettings.Cell,
                    0,
                    (center.y + option.offset.y) * GridSettings.Cell
                );
                
                
                Room instance = Instantiate(room, roomPosition, Quaternion.Euler(0, angle, 0), transform);


                //otevreni pruchodu na obou stranach
                if (shuffledSlot.slot != null) shuffledSlot.slot.Open();
                if (option.entryIndex >= 0) instance.roomSlots[option.entryIndex].Open();

                //nove vychody z mistnosti
                List<(Vector2Int, Vector2Int, RoomSlot)> newExits = new List<(Vector2Int, Vector2Int, RoomSlot)>();
                for (int i = 0; i < instance.roomSlots.Count; i++)
                {
                    if (i == option.entryIndex) continue;

                    RoomSlot slot = instance.roomSlots[i];
                    
                    newExits.Add((
                        RotateInt(slot.localCell, angle) + option.offset,
                        RotateInt(slot.localDirection, angle),
                        slot
                    ));
                    
                }
                
                //prace s listy
                RoomSlotList.Remove(shuffledSlot);
                if (ReferenceEquals(RoomSlotList, nextRoomSlotList))
                {
                    forkRoomSlotList.AddRange(RoomSlotList);
                    RoomSlotList.Clear();
                }
                //nove vychody do listu dalsich slotu
                RoomSlotList.AddRange(newExits);




                return instance;

                //pokud je vsechno v pohode tak se odesle room a vytvori se ve svete nova roomka
            }
            //pokud ne tak se udela jina rotace a zkusi vsechny rotace
        }
        //pokud ne tak se posle null a udela se dalsi pokus
        return null;
    }




    private void MovePlayer()
    {
        if (startInstance == null)
        {
            Debug.LogError("Start instance is null. Cannot move player.");
            return;
        }
        if (startInstance.playerSpawn == null)
        {
            Debug.LogError("Player spawn point is not set in the start room.");
            return;
        }
        Player.position = startInstance.playerSpawn.position;
        Player.rotation = startInstance.playerSpawn.rotation;
    }


    private bool isGridFull(int[,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool DoorsLookOutside(Room room, int angle, Vector2Int offset, int[,] grid)
    {
        foreach (RoomSlot slot in room.roomSlots)
        {
            //buòka za slotem v gridu
            Vector2Int target = RotateInt(slot.localCell, angle) + offset + RotateInt(slot.localDirection, angle);

            if (target.x < 0 || target.x >= grid.GetLength(0) ||
                target.y < 0 || target.y >= grid.GetLength(1))
            {
                return true;
            }
        }
        return false;
    }

    private bool CanPlace(List<Vector2Int> cells, Vector2Int offset, int[,] grid)
    {
        foreach (Vector2Int c in cells)
        {
            int x = c.x + offset.x;
            int y = c.y + offset.y;

            if (x < 0 || x >= grid.GetLength(0) ||
                y < 0 || y >= grid.GetLength(1) ||
                grid[x, y] != 0)
            {
                return false;
            }
        }

        return true;
    }

    private Vector2Int RotateInt(Vector2Int v, int angle)
    {
        Vector2 rotated = RotationToVector(v, Quaternion.Euler(0, angle, 0));
        return new Vector2Int(Mathf.RoundToInt(rotated.x), Mathf.RoundToInt(rotated.y));
    }


    private List<int> RandomDirections()
    {
        List<int> directions1 = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            directions1.Add(i*90);
        }
        List<int> directions2 = new List<int>(directions1).OrderBy(r => Random.value).ToList();
        //Debug.Log(directions2[0]);
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
        nextRoomSlotList.Clear();
        forkRoomSlotList.Clear();

        pathValue = Mathf.FloorToInt(size * size / 2);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        int posX = Random.Range(0, generatorGrid.GetLength(0));
        int posY = Random.Range(0, generatorGrid.GetLength(1));
        nextRoomSlotList.Add((new Vector2Int(posX, posY), new Vector2Int(0, 0), null));
    }
}