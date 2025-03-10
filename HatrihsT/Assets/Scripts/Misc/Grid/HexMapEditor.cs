using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour
{

    public HexGrid hexGrid;


    private HexCell previousCell;

    int activeTerrainTypeIndex;

    void Awake()
    {
        SetEditMode();
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject()
        )
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
		}
        else
        {
            previousCell = null;
        }

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(inputRay, out hit))
            {
                EditCell(hexGrid.GetCell(hit.point));
            }
        }

    }

    public void SetEditMode()
    {
        enabled = !enabled;
    }

    HexCell GetCellUnderCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 200;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (activeTerrainTypeIndex >= 0)
            {;
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
                cell.GetComponent<MeshRenderer>().material = cell.Color;
            }
        }
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

}
