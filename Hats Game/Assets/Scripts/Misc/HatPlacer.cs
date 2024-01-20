using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HatPlacer : MonoBehaviour
{
    public Color[] colors;

    private GameManager gameManager;
    private HexGrid hexGrid;
    private Rigidbody rb;

    public bool isSelected;

    public HexCell currentCell;
    public HexCell landCell;

    public Vector3 thisHatRot;
    public int thisHatRotInt;

    [SerializeField] HexCoordinates cellCo;

    GameObject HatTab;
    public List<Button> buttons = new List<Button>();

    private ChecksValid validityCheck;

    public Material mat1;
    public Material mat2;
    public bool matBool;

    private String normalTag = "Hat";
    private String reverseTag = "Reverse Hat";

    void Start()
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        landCell = hexGrid.GetCell(transform.position);
        currentCell = landCell;
        validityCheck = this.GetComponent<ChecksValid>();
        HatTab = GameObject.Find("HatTab");
        if (HatTab)
        {
            foreach (Button button in HatTab.transform.GetChild(0).GetChild(1).GetComponentsInChildren<Button>())
            {
                buttons.Add(button);
            }
        }
    }

    void FixedUpdate()
    {
        if (isSelected)
        {
            MouseMove();
        }
    }
    private void Update()
    {
        if (isSelected)
        {
            Spin();
            FlipHat();
        }
        if (isSelected)
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
            {
                Deselect();
            }
        }
            
    }

    void MouseMove()
    {
        if (GetCellUnderCursor() != null)
        {
            HexCell currentCell = GetCellUnderCursor();
            HexCell stayCell = hexGrid.GetCell(transform.position);
            if (isSelected)
            {
                this.transform.position = (currentCell.transform.position);
            }
        }
    }
    void Spin()
    {
        if (Input.GetMouseButtonDown((1)) || Input.GetKeyDown(KeyCode.S))
        {
            Vector3 m_EulerAngleVelocity = new Vector3(0, 60, 0);
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity);
            this.transform.rotation *= deltaRotation;
            thisHatRot = transform.eulerAngles;
            thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;
        }
    }
    
    private void Deselect()
    {
        thisHatRot = transform.eulerAngles;
        thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;

        landCell = hexGrid.GetCell(transform.position);
        currentCell = landCell;
        if (landCell.isBinHat)
        {
            
            gameManager.tileSelected = false;
            gameManager.selectedTile = null;
            Destroy(gameObject);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = true;
            }
        }
        else
        {
            if (!landCell.hasHat && !landCell.hasReverseHat)
            {
                
                validityCheck.IsPlacementValid(landCell);

                if (gameManager.GetComponent<TilingHoleMaker>())
                {
                    TilingHoleMaker tilingHoleMaker = gameManager.GetComponent<TilingHoleMaker>();
                   
                    if ((transform.position.x > 530 || transform.position.x < 20 || transform.position.z > 350 || transform.position.z < 50))
                    {
                        validityCheck.isValid = false;
                    }

                    if (this.CompareTag("Hat") && tilingHoleMaker.hats < 1)
                    {
                        validityCheck.isValid = false;
                        HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<SpawnManager>().FlashButtonFunction(5, 0.1f);
                    }
                    else if (this.CompareTag("Reverse Hat") && tilingHoleMaker.reverseHats < 1)
                    {
                        validityCheck.isValid = false;
                        Debug.Log("No more reverseHats");
                        HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SpawnManager>().FlashButtonFunction(5, 0.1f);
                    }
                    
                }

                if (validityCheck.isValid)
                {
                    if (this.CompareTag("Hat"))
                    {
                        landCell.hasHat = true;
                    }
                    else if (this.CompareTag("Reverse Hat"))
                    {
                        landCell.hasReverseHat = true;
                    }

                    landCell.hatRot = Mathf.Round(transform.eulerAngles.y);
                    landCell.hatRotInt = Mathf.RoundToInt(landCell.hatRot / 60) % 6;
                    landCell.hatAbove = this.gameObject;
                    landCell.hatMatIndex = validityCheck.hatMatIndex;
                    isSelected = false;
                    gameManager.tileSelected = false;
                    gameManager.selectedTile = null;
                    
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        buttons[i].interactable = true;
                    }

                    if (gameManager.GetComponent<ChainManager>())
                    {
                        gameManager.GetComponent<ChainManager>().AddHatsToList();
                    }
                    GameObject hatButton = HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;
                    GameObject reverseHatButton = HatTab.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;

                    TilingHoleMaker tilingHoleMaker = gameManager.GetComponent<TilingHoleMaker>();
                    if (tilingHoleMaker)
                    {
                        
                        if (this.CompareTag("Hat"))
                        {
                           gameManager.GetComponent<TilingHoleMaker>().hats -= 1;
                           
                        }
                        else if (this.CompareTag("Reverse Hat"))
                        {
                          
                            gameManager.GetComponent<TilingHoleMaker>().reverseHats -= 1;
                          
                            
                        }
                        if (gameManager.GetComponent<TilingHoleMaker>().hats < 1)
                        {
                            hatButton.GetComponent<Button>().interactable = false;
                        }
                        else if (gameManager.GetComponent<TilingHoleMaker>().reverseHats < 1)
                        {
                            reverseHatButton.gameObject.GetComponent<Button>().interactable = false;
                        }
                        if (gameManager.GetComponent<TilingHoleMaker>().hats + gameManager.GetComponent<TilingHoleMaker>().reverseHats == 0)
                        {
                            tilingHoleMaker.LevelComplete();
                        }
                    }
                }
                else
                {
                    Debug.Log("Invalid");
                    FlashOutlineFunction(3);
                }


            }
            else
            {
                Debug.Log("Hat Already here");
                FlashOutlineFunction(3);
            }
        }
    }

    private void FlipHat()
    {
        if (!gameManager.gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 m_EulerAngleVelocityPos = new Vector3(0, 60, 0);
                Vector3 m_EulerAngleVelocityNeg = new Vector3(0, -60, 0);
                Quaternion deltaRotationPos = Quaternion.Euler(m_EulerAngleVelocityPos);
                Quaternion deltaRotationNeg = Quaternion.Euler(m_EulerAngleVelocityNeg);

                if (transform.localScale.x < 0)
                {
                    transform.rotation *= deltaRotationNeg;
                }
                else
                {
                    transform.rotation *= deltaRotationPos;
                }

                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                if (matBool)
                {
                    transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = mat2;
                    matBool = !matBool;
                    tag = reverseTag;
                }
                else
                {
                    transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = mat1;
                    matBool = !matBool;
                    tag = normalTag;
                }

                Selecter selecter = GetComponentInChildren<Selecter>();
                if (selecter.birthButton != null)
                {
                    SpawnManager spawnManager = selecter.birthButton.GetComponent<SpawnManager>();
                    selecter.birthButton.GetComponent<Button>().interactable = true;
                    selecter.birthButton = spawnManager.oppositeButton;
                    selecter.birthButton.GetComponent<Button>().interactable = false;
                }
            }

        }
    }

    HexCell GetCellUnderCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 200;
        Ray inputRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }
    IEnumerator TrueSelecta()
    {
        yield return new WaitForSeconds(0.05f);
        isSelected = true;
    }

    private void FlashOutlineFunction (int amountTimes)
    {
        StartCoroutine(FlashOutline(amountTimes));
    }

    IEnumerator FlashOutline(int amountTimes)
    {
        for (int i = 0; i < amountTimes; i++)
        {
            GetComponent<LineRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            GetComponent<LineRenderer>().material.color = Color.black;
            yield return new WaitForSeconds(0.2f);
        }
        
    }
}
