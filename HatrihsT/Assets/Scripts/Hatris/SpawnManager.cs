using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{

    public GameObject hat;
    public bool isSelected;
    public bool isPlaced;
    private Vector3 buttonPos;

    private GameManager gameManager;

    private GameObject HatTab;
    private List<Button> buttons = new List<Button>();

    private GameObject hatObj;

    public Button oppositeButton;

    public int numberHats;

    public HexMapCamera cam;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        buttonPos = transform.position;
    }

    private void Start()
    {
        HatTab = GameObject.FindGameObjectWithTag("Hat Tab");
        if (HatTab)
        {
            foreach (Button button in HatTab.transform.GetChild(0).GetChild(1).GetComponentsInChildren<Button>())
            {
                buttons.Add(button);
            }
        }
    }

    public void SpawnHat()
    {
        if (gameManager.tileSelected == false)
        {
            SpawnHatInternal();
        }
        else
        {
            Destroy(gameManager.selectedTile.gameObject);
            SpawnHatInternal();
        }
    }

    private void SpawnHatInternal()
    {
        if (!isSelected)
        {
            float rotDelta = 0f;

            if (this.transform.localScale.x < 0)
            {
                rotDelta = Mathf.Round((cam.currentYaw + 60f) / 60f) * 60f - 60f;
            }
            else
            {
                rotDelta = Mathf.Round((cam.currentYaw) / 60f) * 60f;
            }

            Quaternion yawRotation = Quaternion.Euler(0f, rotDelta, 0f);

            Quaternion savedRotation = hat.transform.rotation;

            Quaternion combinedRotation = savedRotation * yawRotation;

            Vector3 mousePos = Input.mousePosition;
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, 0f, mousePos.y));

            // Instantiate the hat object with the calculated position and rotation
            hatObj = Instantiate(hat, spawnPosition, combinedRotation);

            // Mark the tile as selected and assign the spawned object to the selected tile
            gameManager.tileSelected = true;
            gameManager.selectedTile = hatObj.gameObject;
        }
    }

    HexCell GetCellUnderCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 200;
        Ray inputRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        return null;
    }

    public void ResetHatrisHatTab()
    {
        HatTab.SetActive(true);
        HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Button>().interactable = true;
        HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void FlashButtonFunction(int amountTimes, float flashDuration)
    {
        StartCoroutine(FlashButton(amountTimes, flashDuration));
    }

    IEnumerator FlashButton(int amountTimes, float flashDuration)
    {
        Button button = this.GetComponent<Button>();
        for (int i = 0; i < amountTimes; i++)
        {
            button.interactable = true;
            yield return new WaitForSeconds(flashDuration);
            button.interactable = false;
            yield return new WaitForSeconds(flashDuration);
        }
    }
        
}
