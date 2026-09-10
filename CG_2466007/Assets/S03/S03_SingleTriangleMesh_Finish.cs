using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S03_SingleTriangleMesh_Finish : MonoBehaviour
{
    void Start()
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f), // 0
            new Vector3(1f, 0f, 0f), // 1
            new Vector3(1f, 1f, 0f), // 2
            new Vector3(0f, 1f, 0f), // 3
            new Vector3(-1f, 0f, 0f), // 4
            new Vector3(0f, -1f, 0f), // 5
            new Vector3(-1f, -1f, 0f), // 6
            new Vector3(-1f, 1f, 0f), // 7
            new Vector3(1f, -1f, 0f), // 8
        };

        int[] triangles = new int[]
        {
            7, 3, 4,
            3, 0, 4,
            4, 0, 5,
            3, 1, 0,
            0, 1, 5,
            5, 1, 8
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}

