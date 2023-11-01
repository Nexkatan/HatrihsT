using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public bool HexCoordinatesOn;

    public int width;
    public int height;

    public HexCell cellPrefab;

    HexCell[] cells;

    public TextMeshProUGUI cellLabelPrefab;

    Canvas gridCanvas;
    HexMesh hexMesh;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);

            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position); 
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        if (index > 0 && index < cells.Length)
        {
            HexCell cell = cells[index];
            return cell;
        }
        return null;
    }



    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f * cellPrefab.inlaySize);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f * cellPrefab.inlaySize);



        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;


        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i-1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }


        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
       
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
        new Vector2(position.x, position.z / cellPrefab.inlaySize);
        if (HexCoordinatesOn)
        {
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }
        ;
    }

    

}
