using System.Security;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    [SerializeField] private DifficultyPopup difficultyPopup;



    private void Awake()
    {
        SaveManager.Load();
    }

    public void OpenLocation(LocationData location)
    {
        difficultyPopup.Open(location);
    }
}
