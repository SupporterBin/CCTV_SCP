// TentacleSplineTube.cs
using UnityEngine;

/// Verlet ����Ʈ�� ���ö������� ��Ȱȭ�ϰ�, Ʃ�� �޽÷� �׷���
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TentacleSplineTube : MonoBehaviour
{
    public IChainPos source;
    [Header("Tube Shape")]
    [Range(8, 64)] public int radialSegments = 16; // �ܸ� �� ���׸�Ʈ
    [Range(8, 256)] public int samples = 64;       // ���� ���� ����
    public float radiusRoot = 0.035f;
    public float radiusTip = 0.015f;
    public float twist = 0f; // UV ���� ���� �� ���

    Mesh mesh;
    Vector3[] verts;
    Vector3[] norms;
    Vector2[] uvs;
    int[] tris;

    void Awake()
    {
        mesh = new Mesh { name = "TentacleTube" };
        GetComponent<MeshFilter>().sharedMesh = mesh;
        Allocate();
    }
    public void Init(float rootRad, float tipRad)
    {
        if(rootRad > 0)
            radiusRoot = rootRad;
        if (tipRad > 0)
            radiusTip = tipRad;
    }
    void Allocate()
    {
        int ring = radialSegments;
        int rings = samples;
        verts = new Vector3[ring * rings];
        norms = new Vector3[ring * rings];
        uvs = new Vector2[ring * rings];
        tris = new int[(rings - 1) * ring * 6];
    }

    void LateUpdate()
    {
        if (source == null || source.Positions == null || source.Positions.Length < 4) return;
        var pts = source.Positions;
        int n = pts.Length;

        // 1) ���ö��� ����
        Vector3 prevPoint = Vector3.zero, prevTangent = Vector3.forward;
        for (int i = 0; i < samples; i++)
        {
            float u = (float)i / (samples - 1);
            float f = Mathf.Lerp(1, n - 2, u); // [1, n-2] ����
            int i1 = Mathf.FloorToInt(f);
            int i0 = Mathf.Clamp(i1 - 1, 0, n - 1);
            int i2 = Mathf.Clamp(i1 + 1, 0, n - 1);
            int i3 = Mathf.Clamp(i1 + 2, 0, n - 1);
            float t = f - i1;

            Vector3 p = CatmullRom.Sample(pts[i0], pts[i1], pts[i2], pts[i3], t);
            Vector3 pNext = CatmullRom.Sample(pts[i0], pts[i1], pts[i2], pts[i3], Mathf.Min(t + 0.01f, 1f));
            Vector3 tangent = (pNext - p).normalized;
            if (i == 0) { prevPoint = p; prevTangent = tangent; }

            // 2) ������ ������ �ٻ�(������ �� ���Ϳ� ����)
            Vector3 up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(up, tangent)) > 0.95f) up = Vector3.right;
            Vector3 side = Vector3.Cross(tangent, up).normalized;
            up = Vector3.Cross(side, tangent).normalized;

            // ������ ����
            float r = Mathf.Lerp(radiusRoot, radiusTip, u);

            // 3) �� ����
            int ring = radialSegments;
            for (int k = 0; k < ring; k++)
            {
                float ang = (k / (float)ring) * Mathf.PI * 2f + twist * u;
                Vector3 normal = (Mathf.Cos(ang) * side + Mathf.Sin(ang) * up).normalized;
                Vector3 v = p + normal * r;

                int idx = i * ring + k;
                verts[idx] = v;
                norms[idx] = normal;
                uvs[idx] = new Vector2(k / (float)ring, u);
            }

            prevPoint = p; prevTangent = tangent;
        }

        // 4) �ε���(��Ʈ��)
        int tri = 0;
        int ringCount = radialSegments;
        for (int i = 0; i < samples - 1; i++)
        {
            for (int k = 0; k < ringCount; k++)
            {
                int k1 = (k + 1) % ringCount;
                int a = i * ringCount + k;
                int b = (i + 1) * ringCount + k;
                int c = (i + 1) * ringCount + k1;
                int d = i * ringCount + k1;
                tris[tri++] = a; tris[tri++] = b; tris[tri++] = c;
                tris[tri++] = a; tris[tri++] = c; tris[tri++] = d;
            }
        }

        mesh.Clear(keepVertexLayout: true);
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
    }
}
