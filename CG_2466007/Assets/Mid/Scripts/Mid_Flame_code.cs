using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_Flame_code : MonoBehaviour
{
    [Header("ºÒ²É ´Ü¸é")]
    public Vector2[] profile = new Vector2[]
    {
        new Vector2(0.00f, 0.00f),
        new Vector2(0.22f, 0.08f),
        new Vector2(0.30f, 0.25f),
        new Vector2(0.22f, 0.50f),
        new Vector2(0.10f, 0.75f),
        new Vector2(0.00f, 1.00f), 
    };

    [Header("ºÒ²É ¼³Á¤")]
    [Range(3, 16)] public int sides = 6;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();

        mf.mesh = CreateFlame();
    }

    Mesh CreateFlame()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        float top = profile[profile.Length - 1].y;

        for (int p = 0; p < profile.Length - 1; p++)
        {
            for (int s = 0; s < sides; s++)
            {
                Vector3 a = Around(profile[p], s);
                Vector3 b = Around(profile[p + 1], s);
                Vector3 c = Around(profile[p + 1], s + 1);
                Vector3 d = Around(profile[p], s + 1);

                float u0 = (float)s / sides;
                float u1 = (float)(s + 1) / sides;
                float v0 = profile[p].y / top;
                float v1 = profile[p + 1].y / top;

                AddTriangle(vertices, uvs, triangles, a, b, c,
                    new Vector2(u0, v0), new Vector2(u0, v1), new Vector2(u1, v1));
                AddTriangle(vertices, uvs, triangles, a, c, d,
                    new Vector2(u0, v0), new Vector2(u1, v1), new Vector2(u1, v0));
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Flame";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    Vector3 Around(Vector2 point, int s)
    {
        float rad = 360f / sides * s * Mathf.Deg2Rad;
        return new Vector3(point.x * Mathf.Cos(rad), point.y, point.x * Mathf.Sin(rad));
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