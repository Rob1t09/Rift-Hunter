using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPopup : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private Transform difficultyContainer;

    [SerializeField] private Button difficultyButtonPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button startButton;

    private LocationData currentLocation;
    private int selectedDifficulty;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;


    private List<Button> difficultyButtons = new List<Button>();

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void Open(LocationData location)
    {
        gameObject.SetActive(true);
        currentLocation = location;
        titleText.text = location.DisplayName;
        selectedDifficulty = location.MinDifficulty;

        BuildDifficultyButtons(location);
    }

    private void BuildDifficultyButtons(LocationData location)
    {
        difficultyButtons.Clear();
        foreach (Transform child in difficultyContainer)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 10; i++)
        {
            int difficulty = i + 1;
            Button button = Instantiate(difficultyButtonPrefab, difficultyContainer);

            if (difficulty < location.MinDifficulty)
            {
                button.interactable = false;
                button.image.sprite = emptySprite;
            }
            else if (difficulty <= SaveManager.GetMaxDifficulty())
            {
                button.interactable = true;
                button.image.sprite = unlockedSprite;
            }
            else
            {
                button.interactable = false;
                //nebo true a nejaky info "beat previous difficulties"
                button.image.sprite = lockedSprite;
            }

            difficultyButtons.Add(button);
            button.GetComponentInChildren<TMPro.TMP_Text>().text = difficulty.ToString();
            button.onClick.AddListener(() => OnDifficultyButtonClicked(difficulty));
        }
        UpdateSelection();
    }

    private void OnStartButtonClicked()
    {
        //logika pro spuštìní hry s vybranou obtížností
        Debug.Log($"Starting game at location: {currentLocation.DisplayName}, " +
            $"difficulty: {selectedDifficulty}");

        gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    private void OnDifficultyButtonClicked(int difficultyIndex)
    {
        selectedDifficulty = difficultyIndex;
        Debug.Log($"Selected difficulty: {selectedDifficulty}");
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        int i = 1;
        foreach (Button button in difficultyButtons)
        {
            button.transform.Find("Selected").gameObject.SetActive(i == selectedDifficulty);
            i++;
        }
    }
}
