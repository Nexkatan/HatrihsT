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

    public void LoadGame()
    {
        grid.LoadHatrisHex();
        scoreKeeper.LoadNewGame();
        MainScreen.SetActive(false);
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

        int newIndex = currentIndex + direction;

        if (newIndex < 0)
        {
            newIndex = objectivesTrigger.objectives.Count - 1;
        }
        else if (newIndex >= objectivesTrigger.objectives.Count)
        {
            newIndex = 0;
        }

        objectivesTrigger.TriggerNotification(objectivesTrigger.objectives[newIndex]);
    }
}
