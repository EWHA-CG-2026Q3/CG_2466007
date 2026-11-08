using System.Collections.Generic;
using UnityEngine;

public class Mid_Coal_code : MonoBehaviour
{
    [Header("½¡ ¼³Á¤")]
    public float radius = 1f;
    public float height = 0.5f;
    [Range(1f, 359f)] public float angle = 90f;
    [Range(1, 64)] public int segments = 12;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    void Start()
    {
        Mesh mesh = CreateSectorPrism();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    Mesh CreateSectorPrism()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        Vector3[] bottomArc = new Vector3[segments + 1];
        Vector3[] topArc = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float rad = angle / segments * i * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(rad);
            float z = radius * Mathf.Sin(rad);

            bottomArc[i] = new Vector3(x, 0f, z);
            topArc[i] = new Vector3(x, height, z);
        }

        Vector3 bottomCenter = new Vector3(0f, 0f, 0f);
        Vector3 topCenter = new Vector3(0f, height, 0f);

        for (int i = 0; i < segments; i++)
        {
            AddTriangle(vertices, uvs, triangles,
                topCenter, topArc[i + 1], topArc[i],
                CapUV(topCenter), CapUV(topArc[i + 1]), CapUV(topArc[i]));

            AddTriangle(vertices, uvs, triangles,
                bottomCenter, bottomArc[i], bottomArc[i + 1],
                CapUV(bottomCenter), CapUV(bottomArc[i]), CapUV(bottomArc[i + 1]));

            float u0 = (float)i / segments;
            float u1 = (float)(i + 1) / segments;
            AddQuad(vertices, uvs, triangles,
                bottomArc[i], topArc[i], topArc[i + 1], bottomArc[i + 1],
                new Vector2(u0, 0f), new Vector2(u0, 1f), new Vector2(u1, 1f), new Vector2(u1, 0f));
        }

        AddQuad(vertices, uvs, triangles,
            bottomCenter, topCenter, topArc[0], bottomArc[0],
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f));

        AddQuad(vertices, uvs, triangles,
            bottomArc[segments], topArc[segments], topCenter, bottomCenter,
            new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, 0f));

        Mesh mesh = new Mesh();
        mesh.name = "CharcoalSector";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    Vector2 CapUV(Vector3 p)
    {
        return new Vector2(p.x / (2f * radius) + 0.5f, p.z / (2f * radius) + 0.5f);
    }

    void AddTriangle(List<Vector3> v, List<Vector2> uv, List<int> t,
                     Vector3 a, Vector3 b, Vector3 c,
                     Vector2 uvA, Vector2 uvB, Vector2 uvC)
    {
        int start = v.Count;
        v.Add(a); uv.Add(uvA);
        v.Add(b); uv.Add(uvB);
        v.Add(c); uv.Add(uvC);
        t.Add(start);
        t.Add(start + 1);
        t.Add(start + 2);
    }

    void AddQuad(List<Vector3> v, List<Vector2> uv, List<int> t,
                 Vector3 a, Vector3 b, Vector3 c, Vector3 d,
                 Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
    {
        AddTriangle(v, uv, t, a, b, c, uvA, uvB, uvC);
        AddTriangle(v, uv, t, a, c, d, uvA, uvC, uvD);
    }
}
