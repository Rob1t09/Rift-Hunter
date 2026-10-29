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

        //start mistnost
        do
        {
            int posX = Random.Range(0, generatorGrid.GetLength(0));
            int posY = Random.Range(0, generatorGrid.GetLength(1));

            Instantiate(
                startRoomList[0], 
                TryPlace(startRoomList, generatorGrid, new Vector2(posX, posY)), 
                Quaternion.identity
                        );


        }
        while (false); // zatim nevim jak udelat podminku


    }


    private Vector3 TryPlace(List<Room> list, int[,] grid, Vector2 start)
    {
        do
        {
            Room room = list[Random.Range(0, list.Count)];
            bool badCell = false;

            //start grid pozice kde bude prvni cell

            //prvni cell mistnosti kde se budou propojovat
            Vector2 cell = room.localCells[Random.Range(0, room.localCells.Count)];

            Vector2 avgVector = new Vector2
            (
                (float)room.localCells.Average(v => v.x),
                (float)room.localCells.Average(v => v.y)
            );



            foreach (Vector2 localCell in room.localCells)
            {
                int posX = Mathf.RoundToInt(start.x + localCell.x);
                int posY = Mathf.RoundToInt(start.y + localCell.y);
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

            if (badCell)
            {
                continue;
            }

            //melo by jit dal jen kdyz by zadna cast roomek nevypadla z mapy/nebyla na jiny roomce
            Vector3 roomPosition = new Vector3
                (
                    (start.x + (cell.x - avgVector.x)) * GridSettings.Cell,
                    0,
                    (start.y + (cell.y - avgVector.y)) * GridSettings.Cell
                );


            foreach (Vector2 localCell in room.localCells)
            {
                int posX = Mathf.RoundToInt(start.x + localCell.x);
                int posY = Mathf.RoundToInt(start.y + localCell.y);
                grid[posX, posY] = 1;
            }

            return roomPosition;


        } while (true); 
    }

    private int RandomDirection()
    {
        int randomPos = Random.Range(0, 4);
        return randomPos;
    }

    private Vector2 RotationToVector(Vector2 position, Quaternion rotation)
    {
        Vector3 pos = new Vector3(position.x, 0, position.y);
        Vector3 rotated = rotation * pos;

        return new Vector2(Mathf.RoundToInt(rotated.x), Mathf.RoundToInt(rotated.z));
    }
}