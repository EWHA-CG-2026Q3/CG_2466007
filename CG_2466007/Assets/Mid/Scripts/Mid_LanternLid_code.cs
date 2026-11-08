using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_LanternLid_code : MonoBehaviour
{
    [Header("ÁöºØ ´Ü¸é")]
    public Vector2[] roofProfile = new Vector2[]
    {
        new Vector2(1.00f, 0.00f),
        new Vector2(0.80f, 0.10f),
        new Vector2(0.55f, 0.28f),
        new Vector2(0.35f, 0.50f),
        new Vector2(0.22f, 0.70f),
    };

    [Header("¶Ñ²± ¼³Á¤")]
    public float thickness = 0.04f;
    public float knobWidth = 0.15f;
    public float knobHeight = 0.12f;

    const int SIDES = 4;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();

        mf.mesh = CreateLid();
    }

    Mesh CreateLid()
    {
        int n = roofProfile.Length;
        Vector2 roofTop = roofProfile[n - 1];
        List<Vector2> path = new List<Vector2>();

        path.Add(new Vector2(0f, roofTop.y));
        for (int k = n - 1; k >= 0; k--)
            path.Add(roofProfile[k]);

        for (int k = 0; k < n; k++)
            path.Add(roofProfile[k] + new Vector2(0f, thickness));

        float knobBottom = roofTop.y + thickness;
        path.Add(new Vector2(knobWidth, knobBottom));
        path.Add(new Vector2(knobWidth, knobBottom + knobHeight));
        path.Add(new Vector2(0f, knobBottom + knobHeight));

        float[] dist = new float[path.Count];
        for (int p = 1; p < path.Count; p++)
            dist[p] = dist[p - 1] + Vector2.Distance(path[p - 1], path[p]);
        float total = dist[path.Count - 1];

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int p = 0; p < path.Count - 1; p++)
        {
            for (int s = 0; s < SIDES; s++)
            {
                Vector3 a = Corner(path[p], s);
                Vector3 b = Corner(path[p + 1], s);
                Vector3 c = Corner(path[p + 1], s + 1);
                Vector3 d = Corner(path[p], s + 1);

                float v0 = dist[p] / total;
                float v1 = dist[p + 1] / total;
                Vector2 uvA = new Vector2(0.5f - path[p].x * 0.5f, v0);
                Vector2 uvB = new Vector2(0.5f - path[p + 1].x * 0.5f, v1);
                Vector2 uvC = new Vector2(0.5f + path[p + 1].x * 0.5f, v1);
                Vector2 uvD = new Vector2(0.5f + path[p].x * 0.5f, v0);

                AddTriangle(vertices, uvs, triangles, a, b, c, uvA, uvB, uvC);
                AddTriangle(vertices, uvs, triangles, a, c, d, uvA, uvC, uvD);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "LanternLid";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    Vector3 Corner(Vector2 point, int s)
    {
        float rad = (45f + 360f / SIDES * s) * Mathf.Deg2Rad;
        float r = point.x * Mathf.Sqrt(2f);
        return new Vector3(r * Mathf.Cos(rad), point.y, r * Mathf.Sin(rad));
    }

    void AddTriangle(List<Vector3> v, List<Vector2> uv, List<int> t,
                     Vector3 a, Vector3 b, Vector3 c,
                     Vector2 uvA, Vector2 uvB, Vector2 uvC)
    {
        if (a == b || b == c || a == c) return;

        int start = v.Count;
        v.Add(a); uv.Add(uvA);
        v.Add(b); uv.Add(uvB);
        v.Add(c); uv.Add(uvC);
        t.Add(start);
        t.Add(start + 1);
        t.Add(start + 2);
    }
}