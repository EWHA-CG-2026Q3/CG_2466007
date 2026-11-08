using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_Cauldron_code : MonoBehaviour
{
    [Header("¼Ü ¹Ù±ù ´Ü¸é")]
    public Vector2[] outerProfile = new Vector2[]
    {
        new Vector2(0.50f, 0.00f),
        new Vector2(0.75f, 0.10f),
        new Vector2(0.92f, 0.30f),
        new Vector2(1.00f, 0.50f),
        new Vector2(0.96f, 0.70f),
        new Vector2(0.82f, 0.88f),
        new Vector2(0.72f, 1.00f),
        new Vector2(0.74f, 1.08f),
        new Vector2(0.80f, 1.14f),
    };

    [Header("¼Ü ¼³Á¤")]
    public float thickness = 0.08f;
    [Range(3, 128)] public int segments = 48;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();

        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

        mf.mesh = CreateCauldron();
    }

    Mesh CreateCauldron()
    {
        List<Vector2> path = new List<Vector2>();
        int n = outerProfile.Length;

        path.Add(new Vector2(0f, outerProfile[0].y));
        path.Add(outerProfile[0]);
        path.Add(outerProfile[0]);

        for (int k = 1; k < n; k++)
            path.Add(outerProfile[k]);
        path.Add(outerProfile[n - 1]);

        Vector2 innerTop = InnerPoint(n - 1);
        path.Add(innerTop);
        path.Add(innerTop);
        for (int k = n - 2; k >= 0; k--)
            path.Add(InnerPoint(k));
        path.Add(InnerPoint(0));

        path.Add(new Vector2(0f, InnerPoint(0).y));

        int ring = segments + 1;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        float[] dist = new float[path.Count];
        for (int p = 1; p < path.Count; p++)
            dist[p] = dist[p - 1] + Vector2.Distance(path[p - 1], path[p]);
        float total = dist[path.Count - 1];

        for (int p = 0; p < path.Count; p++)
        {
            for (int i = 0; i <= segments; i++)
            {
                float rad = 360f / segments * i * Mathf.Deg2Rad;
                float r = path[p].x;
                vertices.Add(new Vector3(r * Mathf.Cos(rad), path[p].y, r * Mathf.Sin(rad)));
                uvs.Add(new Vector2((float)i / segments, dist[p] / total));
            }
        }

        List<int> triangles = new List<int>();
        for (int p = 0; p < path.Count - 1; p++)
        {
            if (path[p] == path[p + 1]) continue;

            for (int i = 0; i < segments; i++)
            {
                int a = p * ring + i;
                int b = (p + 1) * ring + i;
                int c = (p + 1) * ring + i + 1;
                int d = p * ring + i + 1;

                if (path[p + 1].x > 0f) { triangles.Add(a); triangles.Add(b); triangles.Add(c); }
                if (path[p].x > 0f) { triangles.Add(a); triangles.Add(c); triangles.Add(d); }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Cauldron";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Vector3[] normals = mesh.normals;
        for (int p = 0; p < path.Count; p++)
        {
            int first = p * ring;
            int last = p * ring + segments;
            Vector3 avg = (normals[first] + normals[last]).normalized;
            normals[first] = avg;
            normals[last] = avg;
        }
        mesh.normals = normals;
        mesh.RecalculateBounds();
        return mesh;
    }

    Vector2 InnerPoint(int k)
    {
        float r = Mathf.Max(outerProfile[k].x - thickness, 0.01f);
        float y = (k == 0) ? outerProfile[0].y + thickness : outerProfile[k].y;
        return new Vector2(r, y);
    }

    public float GetInnerRadiusAt(float y)
    {
        for (int k = 0; k < outerProfile.Length - 1; k++)
        {
            Vector2 p0 = InnerPoint(k);
            Vector2 p1 = InnerPoint(k + 1);
            if (y >= p0.y && y <= p1.y)
                return Mathf.Lerp(p0.x, p1.x, (y - p0.y) / (p1.y - p0.y));
        }
        return InnerPoint(outerProfile.Length - 1).x;
    }
}