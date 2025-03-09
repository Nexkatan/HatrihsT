using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool tileSelected;
    public GameObject selectedTile;
    public bool gameOver = false;

    public static bool AIMode;

    public GameObject MainScreen;
    public Slider hatris_slider;
    public TextMeshProUGUI hatrisSliderText;

    public HexGrid grid;
    public HatrisScoreKeeper scoreKeeper;

    [SerializeField]
    public static int hatrisBoardSize = 3;

    public NotificationTriggerScriptable objectivesTrigger;
    public NotificationScriptable currentObjective;

    public bool tutorialMode;

    public void Start()
    {
        if (hatris_slider != null)
        {
            hatris_slider.onValueChanged.AddListener(value =>
            {
                hatrisBoardSize = (int)value;
                hatrisSliderText.text = value.ToString();
            });
        }
    }

    void Update()
    {
        if (!tutorialMode) return; // Early exit if not in tutorial mode

        // Get the current objective index once instead of calling IndexOf multiple times
        int objectiveIndex = objectivesTrigger.objectives.IndexOf(currentObjective);

        // Check if objective is completed
        if (objectivesTrigger.progressItem.progress.value == 1f)
        {
            ResetProgress();
            CycleObjective(1);
        }

        // Handle progress updates based on objective type
        switch (objectiveIndex)
        {
            case 0:
                HandleZoomProgress();
                break;
            case 1:
                HandleMouseMovementProgress(objectiveIndex);
                break;
            case 2:
                HandleMouseMovementProgress(objectiveIndex);
                break;
            case 3:
                break;
                case 4:
                break;
        }
    }

    // Reset progress for a new objective
    private void ResetProgress()
    {
        objectivesTrigger.progressItem.progress.value = 0f;
        objectivesTrigger.progressItem.targetProgress = 0f;
    }

    // Handles zoom progress for objective 0
    private void HandleZoomProgress()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.Q))
        {
            zoomDelta -= 0.001f; // Adjust value as needed for smooth progression
        }

        if (Input.GetKey(KeyCode.E))
        {
            zoomDelta += 0.001f;
        }
        if (Mathf.Abs(zoomDelta) > 0f)
        {
            objectivesTrigger.progressItem.IncrementProgress(Mathf.Abs(zoomDelta));
        }
    }

    // Handles mouse movement progress for objectives 1 and 2
    private void HandleMouseMovementProgress(int objectiveIndex)
    {
        int mouseButton = (objectiveIndex == 1) ? 1 : 2; // Right-click for obj 1, Middle-click for obj 2


        if (Input.GetMouseButton(mouseButton))
        {
            float movement = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));

            if (movement > 0f)
            {
                objectivesTrigger.progressItem.IncrementProgress(movement);
            }
        }
    }

    public void LoadGame()
    {
        grid.LoadHatrisHex();
        scoreKeeper.LoadNewGame();
        MainScreen.SetActive(false);
        tutorialMode = false;
    }

    public void ReloadGame()
    {
        scoreKeeper.ResetGame();
        LoadGame();
     }

    public void AIModeAction()
    {
        if (AIMode)
        {
            AIMode = false;
        }
        else
        {
            AIMode = true;
        }
    }

    public void LoadTutorial()
    {
        grid.LoadHatrisHex();
        scoreKeeper.LoadNewGame();
        MainScreen.SetActive(false);
        scoreKeeper.HatTab.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.Invoke();
        tutorialMode = true;
        TriggerObjective(1);
    }

    void TriggerObjective(int objective)
    {
        objectivesTrigger.TriggerNotification(objectivesTrigger.objectives[objective-1]);
        currentObjective = objectivesTrigger.objectives[objective-1];
    }

    public void CycleObjective(int direction)
    {
        int currentIndex = objectivesTrigger.objectives.IndexOf(currentObjective);

        Debug.Log("current: " + currentIndex);

        int newIndex = currentIndex + direction;

        Debug.Log("new: " + newIndex);

        if (newIndex+1 < 0)
        {
            newIndex = 0;
        }
        else if (newIndex + 1f >= objectivesTrigger.objectives.Count)
        {
            newIndex = objectivesTrigger.objectives.Count - 1;
        }

        TriggerObjective(newIndex+1);
    }
}
