using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HatrisHexCell : MonoBehaviour
{
    public GameObject hatPieceAbove;
    public GameObject potentialPieceAbove;

    public float radius;
    public float floorRadius;
    public float height;


    void OnEnable()
    {
        var mesh = new Mesh
        {
            name = "Procedural Hat Diamond Mesh"
        };
        mesh.vertices = new Vector3[] {
            new Vector3(0f, height, 0f),
            new Vector3(-(Mathf.Sqrt(3f) / 2f) * radius, height, 1.5f * radius),
            new Vector3(0f, height, 2f * radius),
            new Vector3((Mathf.Sqrt(3f) / 2f) * radius, height, 1.5f * radius),

            new Vector3(-(Mathf.Sqrt(3f) / 2f) * floorRadius, 0f, 1.5f * floorRadius),
            new Vector3(0f, 0f, 2f * floorRadius),
            new Vector3((Mathf.Sqrt(3f) / 2f) * floorRadius, 0f, 1.5f * floorRadius)
        };
        mesh.triangles = new int[] {

            0, 1, 2,
            0, 2, 3,

            2, 1, 4,
            2, 4, 5,
            2, 5, 6,
            2, 6, 3

        };
        mesh.normals = new Vector3[] {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back,
            Vector3.back, Vector3.back, Vector3.back,
        };
        mesh.uv = new Vector2[] {
            new Vector2(0f, 0f),
            new Vector2(0f, 2f * radius),
            new Vector2((Mathf.Sqrt(3f) / 2f) * radius, 1.5f * radius),
            new Vector2(-(Mathf.Sqrt(3f) / 2f) * radius, 1.5f * radius),

            new Vector3(0f, 0f, 2f * floorRadius),
            new Vector3((Mathf.Sqrt(3f) / 2f) * floorRadius, 0f, 1.5f * floorRadius),
            new Vector3(-(Mathf.Sqrt(3f) / 2f) * floorRadius, 0f, 1.5f * floorRadius),
        };
        mesh.tangents = new Vector4[] {
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f),
            new Vector4(1f, 0f, 0f, 1f)

        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}