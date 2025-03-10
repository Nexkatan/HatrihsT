using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using System;
using Unity.VisualScripting;
using System.Linq;

public class HexGrid : MonoBehaviour
{
    public bool HexCoordinatesOn;

    int chunkCountX, chunkCountZ;

    public HexCell cellPrefab;
    public HatrisHexCell cellCellPrefab;

    public HexCell[] cells;

    public Material[] materials;
    public Material[] hatMats;

    public bool cellCell;

    public HexGridChunk chunkPrefab;

    public int cellCountX = 20, cellCountZ = 15;

    HexGridChunk[] chunks;

    public GameObject hatPrefab;
    public GameObject SuperTile1Prefab;
    public GameObject SuperTile2Prefab;
    public GameObject SuperTile3Prefab;
    public GameObject SuperTile4Prefab;

    public bool isHatris;
    public Vector2 centrePoint;
    public int HexMapRadius;

    public HexMapCamera cam;

    public GameObject backgrounds;
    public int backgroundNumber;

    public ParticleSystem[] scoreParticles = new ParticleSystem[3];

    void Awake()
    {
        HexMetrics.materials = materials;
        
        cam.transform.position = new Vector3(170f, 0f, 110f);

        cam.zoom = 0.925f;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
    }

    void OnEnable()
    {
            HexMetrics.materials = materials;
    }

    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position); 
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
    }

    public HexCell GetCell(Vector3 position)
    {
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        if (index > -1 && index < cells.Length)
        {
            HexCell cell = cells[index];
            return cell;
        }
        return null;
    }


    public void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f * cellPrefab.inlaySize);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f * cellPrefab.inlaySize);



        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = cellPrefab.name + " " + i;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i-1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }


        AddCellToChunk(x, z, cell);


        if (cellCell)
        {
            CreateCellCellsHex(cell);
        }
    }

    public void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }


    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public bool CreateMap(int x, int z)
    {
        if (
            x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
            z <= 0 || z % HexMetrics.chunkSizeZ != 0
        )
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }

        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                Destroy(chunks[i].gameObject);
            }
        }

        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
        CreateChunks();
        CreateCells();

        DestroyUnusedCellsHex(cells);
        return true;
    }
    
    public void CreateCellCellsHex(HexCell cell)
    {
            Transform cellChild = cell.transform.GetChild(0);

        if (cell.coordinates.X > centrePoint.x - HexMapRadius && cell.coordinates.X < centrePoint.x + HexMapRadius && cell.coordinates.Z > centrePoint.y - HexMapRadius && cell.coordinates.Z < centrePoint.y + HexMapRadius && -(cell.coordinates.X + cell.coordinates.Z) > -(centrePoint.x + centrePoint.y) - HexMapRadius && -(cell.coordinates.X + cell.coordinates.Z) < -(centrePoint.x + centrePoint.y) + HexMapRadius)
        {
            for (int j = 0; j < 6; j++)
            {
                cell.isHatrisCell = true;
                HatrisHexCell hatrisCell = Instantiate<HatrisHexCell>(cellCellPrefab);

                cell.uiRect = new Vector3(1 * cellCountZ, 0.5f, 0);

                hatrisCell.transform.position = cell.transform.position;

                hatrisCell.name = ("hatrisCell" + j);
                hatrisCell.transform.SetParent(cellChild);
                hatrisCell.transform.RotateAround(cell.transform.position, Vector3.up, 60 * j);
            }
        }
        
    }
    

    public void DestroyUnusedCellsHex(HexCell[] hexCells)
    {
        int neighbourLength = Enum.GetValues(typeof(HexDirection)).Length;

        for (int i = 0; i < hexCells.Length; i++)
        {
            if (hexCells[i].coordinates.X < centrePoint.x - HexMapRadius || hexCells[i].coordinates.X > centrePoint.x + HexMapRadius || hexCells[i].coordinates.Z < centrePoint.y - HexMapRadius || hexCells[i].coordinates.Z > centrePoint.y + HexMapRadius || -(hexCells[i].coordinates.X + hexCells[i].coordinates.Z) < -(centrePoint.x + centrePoint.y) - HexMapRadius || -(hexCells[i].coordinates.X + hexCells[i].coordinates.Z) > -(centrePoint.x + centrePoint.y) + HexMapRadius)
            {
                for (int j = 0; j < neighbourLength; j++)
                {
                    cells[i].ResetNeighbour(hexCells[i]);
                }
                Destroy(hexCells[i].gameObject);
                cells[i] = null;
            }
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }
    }

    public void LoadHatrisHex()
    {
        HexMapRadius = GameManager.hatrisBoardSize;

        CreateMap(20, 15);
    }

}
