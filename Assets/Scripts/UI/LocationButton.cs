using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocationButton : MonoBehaviour
{
    [SerializeField] private LocationData locationData;

    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite finishedSprite;

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private WorldMap worldMap;

    private void Start()
    {
        worldMap = GetComponentInParent<WorldMap>();
        label.text = locationData.DisplayName;
        button.onClick.AddListener(OnButtonClicked);

        Image image = GetComponent<UnityEngine.UI.Image>();

        if (!SaveManager.IsLocationUnlocked(locationData))
        {
            button.interactable = false;
        }


        if (SaveManager.IsLocationCompleted(locationData.Id))
        {
            image.sprite = finishedSprite;
        }
        else if (SaveManager.IsLocationUnlocked(locationData))
        {
            image.sprite = unlockedSprite;
        }
        else
        {
            image.sprite = lockedSprite;
        }
    }

    private void OnButtonClicked()
    {
        worldMap.OpenLocation(locationData);
    }
}
