// TentacleSplineTube.cs
using UnityEngine;

/// Verlet 포인트로 만들어진 뼈대(체인)를 활성화하고, 촉수 메쉬를 그리는 스크립트
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TentacleSplineTube : MonoBehaviour
{
    public IChainPos source;

    [Header("Tube Shape")]
    [Range(8, 64)] public int radialSegments = 16; // 단면(원)당 버텍스 개수
    [Range(8, 256)] public int samples = 64;       // 길이 방향 샘플 수
    public float radiusRoot = 0.035f;
    public float radiusTip = 0.015f;
    public float twist = 0f;                       // UV 기준으로 회전(비틀기)

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
        if (rootRad > 0)
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
        if (source == null || source.Positions == null || source.Positions.Length < 4)
            return;

        var pts = source.Positions;
        int n = pts.Length;
        if (n < 4) return;

        int ring = radialSegments;
        int rings = samples;

        // 1) 체인 포인트들을 Catmull-Rom으로 보간해서 중심선(스플라인) 샘플링
        //
        //   pts[0], pts[1], ... pts[n-1]
        //   세그먼트: [pts[1]~pts[2]], [2~3], ... [n-2~n-1]  => 총 (n-2)개
        //
        //   u(0~1) -> s(0~n-2) -> segIndex(0~n-3), localT(0~1)
        //
        float numSegments = n - 2; // [1~2] ~ [n-2~n-1] 총 개수

        for (int i = 0; i < rings; i++)
        {
            float u = (float)i / (rings - 1);   // 0 ~ 1
            float s = u * numSegments;          // 0 ~ (n-2)

            int seg = Mathf.Min(Mathf.FloorToInt(s), n - 3); // 0 ~ (n-3)
            float t = s - seg;                              // 0 ~ 1

            int i1 = seg + 1;                               // 1 ~ (n-2)
            int i0 = Mathf.Clamp(i1 - 1, 0, n - 1);
            int i2 = Mathf.Clamp(i1 + 1, 0, n - 1);
            int i3 = Mathf.Clamp(i1 + 2, 0, n - 1);

            Vector3 p = CatmullRom.Sample(pts[i0], pts[i1], pts[i2], pts[i3], t);
            float tNext = Mathf.Min(t + 0.01f, 1f);
            Vector3 pNext = CatmullRom.Sample(pts[i0], pts[i1], pts[i2], pts[i3], tNext);
            Vector3 tangent = (pNext - p).sqrMagnitude > 0f
                ? (pNext - p).normalized
                : Vector3.forward;

            // 2) 단면 좌표계(up/side) 만들기
            Vector3 up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(up, tangent)) > 0.95f)
                up = Vector3.right;

            Vector3 side = Vector3.Cross(tangent, up).normalized;
            up = Vector3.Cross(side, tangent).normalized;

            // 반지름 보간
            float r = Mathf.Lerp(radiusRoot, radiusTip, u);

            // 3) 링 버텍스 생성
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
        }

        // 4) 인덱스(트라이앵글) 생성
        int tri = 0;
        int ringCount = ring;
        for (int i = 0; i < rings - 1; i++)
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

        // 5) 메쉬 업데이트
        mesh.Clear(keepVertexLayout: true);
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
    }
}
