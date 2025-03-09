using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public int terrainTypeIndex, hatMatIndex;

    public float inlaySize;

    public bool hasHat;
    public bool hasReverseHat;
    public bool hasSuperTile1;
    public bool hasSuperTile2;
    public bool hasSuperTile3;
    public bool hasSuperTile4;
    public bool hasWallHat;
    public bool isBinHat;
    public float hatRot;
    public int hatRotInt;
    public GameObject hatAbove;
    public Material hatAboveMat;

    public ParticleSystem particle;

    [SerializeField]
    HexCell[] neighbors;
    HexCell[] longbois;

    public RectTransform uiRect;

    public HexGridChunk chunk;

    public bool isHatrisCell;
    public int playerCellScored;

    public bool isReverseHatMove;
    public int moveIntRotation;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public HexCell GetLongboi(LongboiDirection direction)
    {
        return longbois[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public Material Color
    {
        get
        {
            return HexMetrics.materials[terrainTypeIndex];
        }
    }

    public Color MatToColor
    {
        get
        {
            return HexMetrics.materials[terrainTypeIndex].color;
        }
    }

    public Material HatColor
    {
        get
        {
            return HexMetrics.hatMats[hatMatIndex];
        }
    }

    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                Refresh();
            }
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write(hasHat);
        writer.Write(hasReverseHat);
        writer.Write((byte)hatRotInt);
        writer.Write((byte)hatMatIndex);
    }

    public void Load(BinaryReader reader)
    {
        terrainTypeIndex = reader.ReadByte();
        hasHat = reader.ReadBoolean();
        hasReverseHat = reader.ReadBoolean();
        hatRotInt = reader.ReadByte();
        hatMatIndex = reader.ReadByte();


        GetComponent<MeshRenderer>().material = HexMetrics.materials[terrainTypeIndex];
    }
    void Refresh()
    {
        chunk.Refresh();
    }
}