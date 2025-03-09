using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HatrisHatPlacer : MonoBehaviour
{
    public Color[] colors;

    private GameManager gameManager;
    private HexGrid hexGrid;
    private Rigidbody rb;

    public bool isSelected;

    public HexCell currentCell;
    public HexCell landCell;

    public HexCell neighbour1;
    public HexCell neighbour2;

    public Vector3 thisHatRot;
    public int thisHatRotInt;

    [SerializeField] HexCoordinates cellCo;

    public List<Button> player1buttons = new List<Button>();
    public List<Button> player2buttons = new List<Button>();

    private ChecksValid validityCheck;

    private String normalTag = "Hat";
    private String reverseTag = "Reverse Hat";

    private Vector3 lastMousePosition;

    private Dictionary<GameObject, bool> flashingObjects = new Dictionary<GameObject, bool>();


    public enum Team
    {
        None,
        Pink,
        Purple
    }

    public Team team;

    private Material teamMat;
    public Material defaultMat;

    private int playerCount;

    public HatrisScoreKeeper scoreKeeper;

    private SFXClips rotateClips;

    private bool isCameraMoving;
    public void Start()
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        landCell = hexGrid.GetCell(transform.position);
        currentCell = landCell;
        validityCheck = this.GetComponent<ChecksValid>();
        HatTab hatTab = GameObject.Find("HatTab").GetComponent<HatTab>();
        if (hatTab)
        {
            foreach (Button button in hatTab.pinks.GetComponentsInChildren<Button>())
            {
                player1buttons.Add(button);
            }
            foreach (Button button in hatTab.purples.GetComponentsInChildren<Button>())
            {
                player2buttons.Add(button);
            }
        }

        teamMat = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;

        scoreKeeper = GameObject.Find("GameManager").GetComponent<HatrisScoreKeeper>();

        rotateClips = GetComponent<SFXClips>();
    }

    void FixedUpdate()
    {
        if (isSelected)
        {
            if (!Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            {
                MouseMove();
            }
        }
    }
    private void Update()
    {
        if (isSelected)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            Vector3 currentMousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = currentMousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                currentMousePosition = Input.mousePosition;

                    if ((currentMousePosition-lastMousePosition).magnitude < 100f)
                    {
                        Spin(1);
                    }
                lastMousePosition = currentMousePosition;
            }

            if (Input.GetMouseButtonDown(2))
            {
                lastMousePosition = currentMousePosition;
            }

            if (Input.GetMouseButtonUp(2))
            {
                currentMousePosition = Input.mousePosition;

                if ((currentMousePosition - lastMousePosition).magnitude < 100f)
                {
                    Spin(-1);
                }
                lastMousePosition = currentMousePosition;
            }

            FlipHat();
        }

        if (isSelected)
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
            {
                Deselect();
            }
        }
        DeleteHat();
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
            if (this.CompareTag("Hat"))
            {
                neighbour1 = currentCell.GetNeighbor((HexDirection)((thisHatRotInt + 4) % 6));
                neighbour2 = currentCell.GetNeighbor((HexDirection)((thisHatRotInt + 5) % 6));

                if (neighbour1 && neighbour2)
                {
                    if (currentCell.isHatrisCell && neighbour1.isHatrisCell && neighbour2.isHatrisCell)
                    {
                        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
                    }
                }
            }
            else if (this.CompareTag("Reverse Hat"))
            {
                neighbour1 = currentCell.GetNeighbor((HexDirection)((thisHatRotInt) % 6));
                neighbour2 = currentCell.GetNeighbor((HexDirection)((thisHatRotInt + 1) % 6));

                if (neighbour1 && neighbour2)
                {
                    if (currentCell.isHatrisCell && neighbour1.isHatrisCell && neighbour2.isHatrisCell)
                    {
                        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
                    }
                }
            }
        }
    }

    void Spin(float direction)
    {
            Vector3 m_EulerAngleVelocity = new Vector3(0, 60 * direction, 0);
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity);
            this.transform.rotation *= deltaRotation;
            thisHatRot = transform.eulerAngles;
            thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;
            if (rotateClips != null && rotateClips.rotateClips.Length > 0)
            {
                rotateClips.PlayRandomRotateClip();
            }
    }
    public void FlipHat()
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

                if (CompareTag("Hat"))
                {
                    tag = reverseTag; 
                }
                else
                {
                    tag = normalTag;
                    thisHatRotInt = (thisHatRotInt + 5) % 6;
                }

                thisHatRot = transform.eulerAngles;
                thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;
            }

        }
    }
    public void Deselect()
    {
        thisHatRot = transform.eulerAngles;
        thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;

        landCell = hexGrid.GetCell(this.transform.position);
        currentCell = landCell;
        HatrisHexCell[] meshCells = new HatrisHexCell[8];
        int landPiecesCount = 0;

                if (landCell != null && landCell.transform.GetChild(0).childCount == 6)
                {
                    if (CompareTag("Hat"))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            meshCells[i] = landCell.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 4) % 6)) % 6).GetComponent<HatrisHexCell>();
                        }

                        neighbour1 = landCell.GetNeighbor((HexDirection)((thisHatRotInt + 4) % 6));
                        neighbour2 = landCell.GetNeighbor((HexDirection)((thisHatRotInt + 5) % 6));

                        if (neighbour1 != null && neighbour1.transform.GetChild(0).childCount == 6 && neighbour2 != null && neighbour2.transform.GetChild(0).childCount == 6)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                meshCells[i + 4] = neighbour1.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 1) % 6)) % 6).GetComponent<HatrisHexCell>();
                                meshCells[i + 6] = neighbour2.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 3) % 6)) % 6).GetComponent<HatrisHexCell>();
                            }
                            for (int i = 0; i < meshCells.Length; i++)
                            {
                                if (meshCells[i].hatPieceAbove != null)
                                {
                                    landPiecesCount++;
                                }
                            }
                        }
                    }


                    else if (CompareTag("Reverse Hat"))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            meshCells[i] = landCell.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 5) % 6)) % 6).GetComponent<HatrisHexCell>();
                        }

                        neighbour1 = landCell.GetNeighbor((HexDirection)((thisHatRotInt) % 6));
                        neighbour2 = landCell.GetNeighbor((HexDirection)((thisHatRotInt + 1) % 6));

                        if (neighbour1 != null && neighbour1.transform.GetChild(0).childCount == 6 && neighbour2 != null && neighbour2.transform.GetChild(0).childCount == 6)
                        { 
                            for (int i = 0; i < 2; i++)
                            {
                                meshCells[i + 4] = neighbour2.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 4) % 6)) % 6).GetComponent<HatrisHexCell>();
                                meshCells[i + 6] = neighbour1.transform.GetChild(0).GetChild((thisHatRotInt + ((i + 2) % 6)) % 6).GetComponent<HatrisHexCell>();
                            }
                            for (int i = 0; i < meshCells.Length; i++)
                            {
                                if (meshCells[i].hatPieceAbove != null)
                                {
                                    landPiecesCount++;

                                }
                            }
                        }
                    }

                    int meshPiecesCount = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        if (meshCells[i] == null)
                        {
                            meshPiecesCount++;
                        }
                    }

                    if (meshPiecesCount > 0)
                    {
                        Debug.Log("Neighbour invalid");
                    }
                    else
                    {
                        for (int i = 0; i < meshCells.Length; i++)
                        {
                            if (meshCells[i].hatPieceAbove != null)
                            {
                                PlacementError(meshCells[i].hatPieceAbove.gameObject);

                        Debug.Log("object " + i + ": " + meshCells[i].hatPieceAbove.gameObject);
                            }
                        }

                        if (landPiecesCount == 0)
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
                            isSelected = false;
                            gameManager.tileSelected = false;
                            gameManager.selectedTile = null;

                            GameObject[] hatPieces = new GameObject[8];

                           
                            for (int i = 0; i < 8; i++)
                            {
                                hatPieces[i] = transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
                                hatPieces[i].name = "hatPiece " + i;
                            }

                            if (CompareTag("Hat"))
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    meshCells[i].hatPieceAbove = hatPieces[i];
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    meshCells[j].hatPieceAbove = hatPieces[3 - j];
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    meshCells[4 + k].hatPieceAbove = hatPieces[5 - k];
                                    meshCells[6 + k].hatPieceAbove = hatPieces[7 - k];
                                }
                            }
                            
                            Destroy(GetComponent<LineRenderer>());

                            int count2 = 0, count3 = 0, count4 = 0;

                            for (int i = 0; i < 6; i++)
                            {
                                if (landCell.transform.GetChild(0).GetChild(i).GetComponent<HatrisHexCell>().hatPieceAbove != null)
                                {
                                    count2++;
                                }
                                if (neighbour1.transform.GetChild(0).GetChild(i).GetComponent<HatrisHexCell>().hatPieceAbove != null)
                                {
                                    count3++;
                                }
                                if (neighbour2.transform.GetChild(0).GetChild(i).GetComponent<HatrisHexCell>().hatPieceAbove != null)
                                {
                                    count4++;
                                }
                            }

                            if (count2 == 6)
                            {
                                Score(landCell);
                            }
                            if (count3 == 6)
                            {
                                Score(neighbour1);
                            }
                            if (count4 == 6)
                            {
                                Score(neighbour2);
                            }

                            scoreKeeper.KeepScore();
                            ResetButton();


                        scoreKeeper.CheckGameOver();
                        if (!gameManager.gameOver)
                        {
                        bool AImodeCheck = GameManager.AIMode;
                        if (GameManager.AIMode)
                        {
                            scoreKeeper.MoveAIPlayerHat();
                        }
                        }
                        }
                    }
                }
    }

    void CheckValid()
    {
        thisHatRot = transform.eulerAngles;
        thisHatRotInt = Mathf.RoundToInt(thisHatRot.y / 60) % 6;

        landCell = hexGrid.GetCell(this.transform.position);
        currentCell = landCell;
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

    void Score(HexCell cell)
    {
        Color teamColour = teamMat.color;

        for (int i = 0; i < 6; i++)
        {
            Destroy(cell.transform.GetChild(0).GetChild(i).GetComponent<HatrisHexCell>().hatPieceAbove);
            cell.transform.GetChild(0).GetChild(i).gameObject.GetComponent<MeshRenderer>().material.color = teamColour;
            cell.playerCellScored = (int)team;
            cell.transform.GetChild(0).GetChild(i).GetComponent<HatrisHexCell>().hatPieceAbove = null;
            cell.hasHat = false;
            cell.hasReverseHat = false;
        }
    }

    void ResetButton()
    {
        if (!GameManager.AIMode)
        {
            if (scoreKeeper.playerCount == 0)
            {
                for (int i = 0; i < player1buttons.Count; i++)
                {
                    player1buttons[i].interactable = true;
                    player2buttons[i].interactable = false;
                }
            }
            else
            {
                for (int i = 0; i < player1buttons.Count; i++)
                {
                    player1buttons[i].interactable = false;
                    player2buttons[i].interactable = true;
                }
            }
        }
    }

    void DeleteHat()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (gameManager.tileSelected)
            {
                Destroy(gameManager.selectedTile.gameObject);
                gameManager.tileSelected = false;
            }
            ResetButton();
        }
    }

    IEnumerator TrueSelecta()
    {
        yield return new WaitForSeconds(0.05f);
        isSelected = true;
    }

    void PlacementError(GameObject piece)
    {
        StartCoroutine(FlashPiece(3, piece));
    }

    IEnumerator FlashPiece(int amountTimes, GameObject piece)
    {
        if (piece == null) yield break;

        // Check if this specific piece is already flashing
        if (flashingObjects.TryGetValue(piece, out bool isFlashing) && isFlashing)
        {
            yield break;
        }

        flashingObjects[piece] = true; // Mark this piece as flashing

        MeshRenderer renderer = piece.GetComponent<MeshRenderer>();
        if (renderer == null) yield break;

        Color originalColour = renderer.material.color;
        Color flashColour = Color.red;

        for (int i = 0; i < amountTimes; i++)
        {
            if (piece == null) break;

            renderer.material.color = flashColour;
            piece.transform.localScale *= 1.5f;
            yield return new WaitForSeconds(0.2f);

            if (piece == null) break;

            renderer.material.color = originalColour;
            piece.transform.localScale /= 1.5f;
            yield return new WaitForSeconds(0.2f);
        }

        // Reset flashing state
        if (piece != null)
        {
            renderer.material.color = originalColour;
        }

        flashingObjects[piece] = false;
    }
 }
