using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_Stick_code : MonoBehaviour
{
    [Header("막대 모양")]
    public float stickRadius = 0.04f;
    public float stickLength = 1.6f;
    [Range(3, 32)] public int segments = 12;

    [Header("젓는 움직임")]
    public Transform cauldron; 
    public float bottomHeight = 0.5f;
    public float stirRadius = 0.3f;
    public float tiltAngle = 20f;
    public float stirSpeed = 90f;

    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] baseNormals;
    float orbitAngle;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();

        mesh = CreateStick();
        mesh.MarkDynamic();
        mf.mesh = mesh;

        baseVertices = mesh.vertices;
        baseNormals = mesh.normals;
    }

    void Update()
    {
        orbitAngle += stirSpeed * Time.deltaTime;

        Matrix4x4 M = transform.worldToLocalMatrix;
        foreach (Matrix4x4 step in BuildSteps())
            M = M * step;
        Vector3[] verts = new Vector3[baseVertices.Length];
        Vector3[] norms = new Vector3[baseNormals.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            verts[i] = M.MultiplyPoint(baseVertices[i]);
            norms[i] = M.MultiplyVector(baseNormals[i]).normalized;
        }

        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.RecalculateBounds();
    }

    Mesh CreateStick()
    {
        List<Vector2> path = new List<Vector2>
        {
            new Vector2(0f, 0f),
            new Vector2(stickRadius, 0f),
            new Vector2(stickRadius, 0f),
            new Vector2(stickRadius, stickLength),
            new Vector2(stickRadius, stickLength),
            new Vector2(0f, stickLength),
        };

        int ring = segments + 1;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int p = 0; p < path.Count; p++)
        {
            for (int i = 0; i <= segments; i++)
            {
                float rad = 360f / segments * i * Mathf.Deg2Rad;
                float r = path[p].x;
                vertices.Add(new Vector3(r * Mathf.Cos(rad), path[p].y, r * Mathf.Sin(rad)));
                uvs.Add(new Vector2((float)i / segments, path[p].y / stickLength));
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

        Mesh m = new Mesh();
        m.name = "Stick";
        m.vertices = vertices.ToArray();
        m.uv = uvs.ToArray();
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();

        Vector3[] normals = m.normals;
        for (int p = 0; p < path.Count; p++)
        {
            int first = p * ring;
            int last = p * ring + segments;
            Vector3 avg = (normals[first] + normals[last]).normalized;
            normals[first] = avg;
            normals[last] = avg;
        }
        m.normals = normals;
        m.RecalculateBounds();
        return m;
    }

    Matrix4x4[] BuildSteps()
    {
        Matrix4x4 cauldronMatrix = (cauldron != null) ? cauldron.localToWorldMatrix : Matrix4x4.identity;

        return new Matrix4x4[]
        {
            cauldronMatrix,
            Matrix4x4.Translate(new Vector3(0f, bottomHeight, 0f)),
            Matrix4x4.Rotate(Quaternion.Euler(0f, orbitAngle, 0f)),
            Matrix4x4.Translate(new Vector3(stirRadius, 0f, 0f)),
            Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -tiltAngle)), 
            Matrix4x4.Scale(transform.localScale),
        };
    }

    void OnDrawGizmos()
    {
        Matrix4x4[] steps = BuildSteps();

        Matrix4x4 M = Matrix4x4.identity;
        Vector3 prev = Vector3.zero;
        for (int k = 0; k < steps.Length; k++)
        {
            M = M * steps[k];
            Vector3 origin = M.GetColumn(3);
            float len = 0.2f;

            Gizmos.color = Color.red; Gizmos.DrawLine(origin, origin + ((Vector3)M.GetColumn(0)).normalized * len);
            Gizmos.color = Color.green; Gizmos.DrawLine(origin, origin + ((Vector3)M.GetColumn(1)).normalized * len);
            Gizmos.color = Color.blue; Gizmos.DrawLine(origin, origin + ((Vector3)M.GetColumn(2)).normalized * len);

            Gizmos.color = Color.yellow;
            if (k > 0) Gizmos.DrawLine(prev, origin);
            Gizmos.DrawWireSphere(origin, 0.03f);
            prev = origin;
        }
    }
}