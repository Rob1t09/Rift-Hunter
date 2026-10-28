using UnityEditor;
using UnityEngine;

//kdyz neco nefunguje spustit tento script
//kdyz vytvorim novy prefaby tak spustit
//tento script bezi jen v editoru Unity a do hry se nikdy nedostane
//spousti se v menu: Tools -> Rooms -> Nastavit prefaby
public static class RoomPrefabSetup
{
    // ---------- Settings ----------
    private const string RoomsFolder = "Assets/Prefabs/Rooms"; //prefaby mistnosti
    private const string CellsName = "Grid Cells";             //rodic se znackami bunek
    private const string SlotsName = "Door Area";              //rodic se sloty (pruchody)
    private const string ClosedWallName = "Block Wall";        //zed ve slotu
    private const string DoorwayName = "Hallway";              //pruchod ve slotu
    // ------------------------------

    private static int warnings; //pocet varovani

    [MenuItem("Tools/Rooms/Nastavit prefaby")]
    public static void SetupAll()
    {
        warnings = 0;
        int processed = 0;

        //projede celou slozku prefabu (RoomsFolder)
        //najde vsechny prefaby v ni (t:Prefab, kde t je type) a udela to do pole
        //guid je 16kova soustava takze string
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { RoomsFolder });

        foreach (string id in ids)
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            
            //nacte prefab do pameti
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

            try
            {
                //ulozi se prefab
                if (SetupPrefab(prefabRoot, path))
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    processed++;
                }
            }
            catch (System.Exception e)
            {
                Warn(path + ": chyba pøi nastavování: " + e.Message);
            }
            finally
            {
                //uvolni prefab z pameti
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Nastavení prefabù hotovo. Zpracováno: " + processed +
                  " z " + ids.Length + ", varování: " + warnings);
    }

    // Upraví jeden prefab. Vrací true, pokud to byla místnost a má se uložit.
    private static bool SetupPrefab(GameObject prefabRoot, string path)
    {
        //hleda potomkak s jmenem SlotsName
        Transform slotsParent = prefabRoot.transform.Find(SlotsName);
        //kouka jestli prefab ma room script
        Room room = prefabRoot.GetComponent<Room>();

        //prefab nema v sobe Door area takze neni mistnost, kdyztak 'path' at vim kde chyba 
        if (slotsParent == null && room == null)
        {
            Debug.Log("Pøeskoèeno (není místnost): " + path);
            return false;
        }

        //prida skript room na koren prefabu at to je automaticky
        if (room == null)
        {
            room = prefabRoot.AddComponent<Room>();
        }


        //puvodne kdyz je seznam bunek (bod gridu) prazdny tak ho naplnime
        //ted mazu kvuli nesikovnosti
        room.cells.Clear();
        
        Transform cellsParent = prefabRoot.transform.Find(CellsName);
        if (cellsParent == null)
        {
            Warn(prefabRoot.name + ": chybí objekt '" + CellsName + "' se znaèkami bunìk.");
        }
        else
        {
            foreach (Transform marker in cellsParent)
            {
                room.cells.Add(marker);
            }
        }
        





        if (slotsParent == null)
        {
            Warn(prefabRoot.name + ": chybí objekt '" + SlotsName + "'.");
        }
        else
        {
            //prime potomky SlotsName/slotsParent, kazdy je slot pro mozne dvere/zed
            foreach (Transform slotTransform in slotsParent)
            {
                //pokud nema RoomSlot script tak se prida
                RoomSlot slot = slotTransform.GetComponent<RoomSlot>();
                if (slot == null)
                {
                    slot = slotTransform.gameObject.AddComponent<RoomSlot>();
                }

                //ve skriptu RoomSlot nastavy promeny na spravnou cast hiearchie
                if (slot.closedWall == null)
                {
                    Transform closed = slotTransform.Find(ClosedWallName);
                    if (closed == null)
                        Warn(prefabRoot.name + "/" + slotTransform.name + ": chybí '" + ClosedWallName + "'.");
                    else
                        slot.closedWall = closed.gameObject;
                }

                if (slot.doorway == null)
                {
                    Transform door = slotTransform.Find(DoorwayName);
                    if (door == null)
                        Warn(prefabRoot.name + "/" + slotTransform.name + ": chybí '" + DoorwayName + "'.");
                    else
                        slot.doorway = door.gameObject;
                }
            }
        }

        //vypocita promeny
        room.Fill();

        return true;
    }

    private static void Warn(string message)
    {
        warnings++;
        Debug.LogWarning(message);
    }
}