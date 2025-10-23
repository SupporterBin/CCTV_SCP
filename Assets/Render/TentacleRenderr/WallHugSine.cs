// WallHugSine.cs
// 방(Box) 내부에서 루트→한 방향으로 뻗어 가장 가까운 벽에 닿은 뒤
// 그 벽면을 따라 사인 파동으로 움직이는 분절 위치들을 positions[]에 저장
using UnityEngine;

[ExecuteAlways]
public class WallHugSine : MonoBehaviour, IChainPos
{
    public enum BoundsBasis { ParentLocal, World } // 박스 좌표계 기준

    [Header("Segments / Length")]
    [Min(2)] public int segments = 32;
    [Min(0.01f)] public float totalLength = 3f;

    [Header("Initial Direction (in Bounds Basis)")]
    public Axis3D alongAxis = Axis3D.Z; // 루트에서 뻗는 기준 축
    public bool reverseAlong = false;   // 음의 방향 시작

    [Header("Wave on Wall (after first hit)")]
    [Min(0f)] public float amplitude = 0.15f;      // 벽 위 파동 진폭
    [Min(0.01f)] public float wavelength = 1.2f;   // 파장(길이 기준)
    public float speed = 1f;                        // 진행 속도(사이클/초)
    public float phaseOffset = 0f;                  // 전체 위상(라디안)
    [Range(0, 1)] public float tipTaper = 0.25f;     // 팁 쪽 진폭 감소(0=균일)

    [Header("Before-Hit (optional wobble)")]
    public float preHitAmplitude = 0f;              // 닿기 전 흔들림(0=꺼짐)
    public Axis3D preHitLateralAxis = Axis3D.X;     // 닿기 전 횡축

    [Header("Bounds Auto-Detection (Raycast)")]
    public BoundsBasis boundsBasis = BoundsBasis.ParentLocal; // 권장: ParentLocal
    public LayerMask wallMask = ~0;                 // 벽 레이어
    public float raycastMaxDistance = 100f;
    public bool autoDetectOnEnable = true;          // OnEnable 때 자동 감지
    public float reDetectInterval = 0f;             // >0이면 주기적 재감지(초)
    public bool drawDebug = false;

    [Header("Output")]
    public bool outputWorldSpace = true;            // true면 월드 좌표로 저장
    [Tooltip("루트→팁 순으로 저장되는 최종 분절 위치")]
    public Vector3[] positions;
    public Vector3[] Positions=>positions;

    // 감지 결과(읽기전용)
    [SerializeField] private bool hasBounds = false;
    [SerializeField] private Vector3 boxCenterBasis;   // 박스 중심(기준 공간)
    [SerializeField] private Vector3 boxExtentsBasis;  // 박스 반사이즈(기준 공간)

    float lastDetectTime = -999f;

    // ─────────────────────────────────────────────────────────────────────

    void OnEnable()
    {
        EnsureArray();
        if (autoDetectOnEnable) TryDetectBounds();
        Rebuild(0f);
    }

    void OnValidate()
    {
        EnsureArray();
        if (preHitAmplitude > 0f && preHitLateralAxis == alongAxis)
            preHitLateralAxis = (alongAxis == Axis3D.Z) ? Axis3D.X : Axis3D.Z; // 주축과 다르게

        if (autoDetectOnEnable && !Application.isPlaying) TryDetectBounds();
        Rebuild(0f);
    }

    void Update()
    {
        if (reDetectInterval > 0f && (Time.realtimeSinceStartup - lastDetectTime) >= reDetectInterval)
        {
            if (TryDetectBounds())
                lastDetectTime = Time.realtimeSinceStartup;
        }

        float t = Application.isPlaying ? Time.time : 0f;
        Rebuild(t);
    }

    // ─────────────────────────────────────────────────────────────────────
    // 메인 빌드: positions[] 계산
    void Rebuild(float timeSec)
    {
        EnsureArray();

        Transform basisParent = (boundsBasis == BoundsBasis.ParentLocal) ? transform.parent : null;

        // 루트와 진행 방향(기준 공간)
        Vector3 rootBasis = ToBasisPoint(transform.position, basisParent);
        Vector3 dirBasis = AxisToVector(alongAxis) * (reverseAlong ? -1f : 1f);

        // 감지 실패 → 직선(옵션: pre-hit 사인)
        if (!hasBounds || boxExtentsBasis.x <= 0 || boxExtentsBasis.y <= 0 || boxExtentsBasis.z <= 0)
        {
            FillStraight(rootBasis, dirBasis, timeSec, basisParent);
            return;
        }

        // 루트→dir 레이와 AABB 교차
        if (!RayAABBHit(rootBasis, dirBasis, boxCenterBasis, boxExtentsBasis, out float tHit, out int face))
        {
            FillStraight(rootBasis, dirBasis, timeSec, basisParent);
            return;
        }
        tHit = Mathf.Max(0f, tHit);

        // 벽 면 정보
        Vector3 n = FaceNormal(face);
        GetFaceTangents(face, out Vector3 uAxis, out Vector3 vAxis);

        // 벽면 위 진행 방향 T: dirBasis에서 법선 성분 제거
        Vector3 T = dirBasis - Vector3.Dot(dirBasis, n) * n;
        if (T.sqrMagnitude < 1e-8f) T = uAxis; // 수직이면 임의 접선
        T.Normalize();

        // 파동 축: n ⨯ T (벽면에 접하고 T와 직교)
        Vector3 waveAxis = Vector3.Cross(n, T);
        if (waveAxis.sqrMagnitude < 1e-8f) waveAxis = uAxis; // 예외 시 보정
        waveAxis.Normalize();

        // 파동 파라미터
        float segLen = totalLength / (segments - 1);
        float k = Mathf.PI * 2f / Mathf.Max(0.0001f, wavelength);
        float w = Mathf.PI * 2f * speed;
        float phi = phaseOffset;

        // 히트 지점 및 위상 연속 보정
        Vector3 hitBasis = rootBasis + dirBasis * tHit;
        hitBasis = ProjectPointToFace(hitBasis, face, boxCenterBasis, boxExtentsBasis);
        float phiWall = k * tHit + phi; // s=tHit에서 연속되게

        for (int i = 0; i < segments; i++)
        {
            float s = segLen * i; // 루트부터의 거리
            Vector3 pBasis;

            if (s <= tHit)
            {
                // 벽 닿기 전: 직선 + (선택) 가벼운 흔들림
                pBasis = rootBasis + dirBasis * s;

                if (preHitAmplitude > 0f)
                {
                    Vector3 lat = AxisToVector(preHitLateralAxis);
                    // 주축과 거의 같으면 직교 성분만 남김
                    lat = (lat - Vector3.Dot(lat, dirBasis) * dirBasis);
                    if (lat.sqrMagnitude < 1e-8f) lat = Vector3.Cross(dirBasis, Vector3.up);
                    if (lat.sqrMagnitude < 1e-8f) lat = Vector3.Cross(dirBasis, Vector3.right);
                    lat.Normalize();

                    float preTheta = k * s - w * timeSec + phi;
                    pBasis += lat * (preHitAmplitude * Mathf.Sin(preTheta));
                }
            }
            else
            {
                // 벽 닿은 뒤: 면 위로 이동 + 새 파동 축으로 진동(위상 연속)
                float dWall = s - tHit; // 면 위에서 이동한 거리
                Vector3 baseOnWall = hitBasis + T * dWall;

                // 면 밖으로 나가지 않도록 법선 성분 제거(수치 안전)
                baseOnWall -= Vector3.Dot(baseOnWall - hitBasis, n) * n;

                // 진폭 테이퍼
                float tNorm = (segments <= 1) ? 0f : (i / (float)(segments - 1));
                float amp = Mathf.Lerp(amplitude, amplitude * (1f - tipTaper), tNorm);

                // θ = k*(dWall) - ωt + phiWall  (연속)
                float theta = k * dWall - w * timeSec + phiWall;
                Vector3 offset = waveAxis * (amp * Mathf.Sin(theta));

                // 법선 성분 제거(이론상 0이지만 안전)
                offset -= Vector3.Dot(offset, n) * n;

                pBasis = baseOnWall + offset;

                // 면 경계 안으로 클램프
                pBasis = ClampPointToFaceBounds(pBasis, face, boxCenterBasis, boxExtentsBasis);
            }

            positions[i] = FromBasisPoint(pBasis, basisParent, outputWorldSpace);
        }
    }

    // 감지 실패 시: 직선 + (선택) pre-hit 사인
    void FillStraight(Vector3 rootBasis, Vector3 dirBasis, float timeSec, Transform basisParent)
    {
        float segLen = totalLength / (segments - 1);
        float k = Mathf.PI * 2f / Mathf.Max(0.0001f, wavelength);
        float w = Mathf.PI * 2f * speed;
        float phi = phaseOffset;

        for (int i = 0; i < segments; i++)
        {
            float s = segLen * i;
            Vector3 pBasis = rootBasis + dirBasis.normalized * s;

            if (preHitAmplitude > 0f)
            {
                Vector3 lat = AxisToVector(preHitLateralAxis);
                lat = (lat - Vector3.Dot(lat, dirBasis) * dirBasis);
                if (lat.sqrMagnitude < 1e-8f) lat = Vector3.Cross(dirBasis, Vector3.up);
                if (lat.sqrMagnitude < 1e-8f) lat = Vector3.Cross(dirBasis, Vector3.right);
                lat.Normalize();

                float preTheta = k * s - w * timeSec + phi;
                pBasis += lat * (preHitAmplitude * Mathf.Sin(preTheta));
            }

            positions[i] = FromBasisPoint(pBasis, basisParent, outputWorldSpace);
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    // 박스 감지: 현재 위치 기준 ±X/±Y/±Z 레이캐스트
    public bool TryDetectBounds()
    {
        Transform basisParent = (boundsBasis == BoundsBasis.ParentLocal) ? transform.parent : null;

        // 기준축(월드 방향)
        Vector3 Xw = (basisParent) ? basisParent.TransformDirection(Vector3.right) : Vector3.right;
        Vector3 Yw = (basisParent) ? basisParent.TransformDirection(Vector3.up) : Vector3.up;
        Vector3 Zw = (basisParent) ? basisParent.TransformDirection(Vector3.forward) : Vector3.forward;

        Vector3 origin = transform.position;

        bool hxP = RayOne(origin, Xw, out RaycastHit hxPlus);
        bool hxM = RayOne(origin, -Xw, out RaycastHit hxMinus);
        bool hyP = RayOne(origin, Yw, out RaycastHit hyPlus);
        bool hyM = RayOne(origin, -Yw, out RaycastHit hyMinus);
        bool hzP = RayOne(origin, Zw, out RaycastHit hzPlus);
        bool hzM = RayOne(origin, -Zw, out RaycastHit hzMinus);

        if (!(hxP && hxM && hyP && hyM && hzP && hzM))
        {
            hasBounds = false;
            if (drawDebug)
                Debug.LogWarning("[WallHugSine_AutoDetect] Raycast로 방 경계를 충분히 찾지 못했습니다.");
            return false;
        }

        // 히트 지점을 기준공간으로 변환
        Transform basisParentT = (boundsBasis == BoundsBasis.ParentLocal) ? transform.parent : null;

        Vector3 xp = ToBasisPoint(hxPlus.point, basisParentT);
        Vector3 xm = ToBasisPoint(hxMinus.point, basisParentT);
        Vector3 yp = ToBasisPoint(hyPlus.point, basisParentT);
        Vector3 ym = ToBasisPoint(hyMinus.point, basisParentT);
        Vector3 zp = ToBasisPoint(hzPlus.point, basisParentT);
        Vector3 zm = ToBasisPoint(hzMinus.point, basisParentT);

        // 각 축별 중앙/반경
        float cx = 0.5f * (xp.x + xm.x);
        float cy = 0.5f * (yp.y + ym.y);
        float cz = 0.5f * (zp.z + zm.z);

        float ex = 0.5f * Mathf.Abs(xp.x - xm.x);
        float ey = 0.5f * Mathf.Abs(yp.y - ym.y);
        float ez = 0.5f * Mathf.Abs(zp.z - zm.z);

        boxCenterBasis = new Vector3(cx, cy, cz);
        boxExtentsBasis = new Vector3(ex, ey, ez);
        hasBounds = (ex > 1e-4f && ey > 1e-4f && ez > 1e-4f);

        if (drawDebug)
        {
            DrawRay(origin, Xw, hxPlus.distance, Color.red);
            DrawRay(origin, -Xw, hxMinus.distance, Color.red);
            DrawRay(origin, Yw, hyPlus.distance, Color.green);
            DrawRay(origin, -Yw, hyMinus.distance, Color.green);
            DrawRay(origin, Zw, hzPlus.distance, Color.blue);
            DrawRay(origin, -Zw, hzMinus.distance, Color.blue);
        }

        lastDetectTime = Time.realtimeSinceStartup;
        return hasBounds;
    }

    bool RayOne(Vector3 origin, Vector3 dir, out RaycastHit hit)
    {
        dir.Normalize();
        bool ok = Physics.Raycast(origin, dir, out hit, raycastMaxDistance, wallMask, QueryTriggerInteraction.Ignore);
        if (drawDebug && !ok) Debug.DrawRay(origin, dir * raycastMaxDistance, Color.yellow, 0.5f);
        return ok;
    }

    void DrawRay(Vector3 origin, Vector3 dir, float dist, Color c)
    {
        Debug.DrawRay(origin, dir.normalized * dist, c, 0.5f);
    }

    // ─────────────────────────────────────────────────────────────────────
    // AABB 헬퍼(모두 "기준 공간"에서 수행)
    static bool RayAABBHit(Vector3 o, Vector3 d, Vector3 c, Vector3 e, out float tHit, out int face)
    {
        tHit = 0f; face = -1;
        d = d.normalized;

        float tmin = -Mathf.Infinity, tmax = Mathf.Infinity;
        int hitFace = -1;

        // X
        if (Mathf.Abs(d.x) < 1e-8f)
        {
            if (o.x < c.x - e.x || o.x > c.x + e.x) return false;
        }
        else
        {
            float tx1 = (c.x - e.x - o.x) / d.x; // Left
            float tx2 = (c.x + e.x - o.x) / d.x; // Right
            float t1 = Mathf.Min(tx1, tx2);
            float t2 = Mathf.Max(tx1, tx2);
            if (t1 > tmin) { tmin = t1; hitFace = (tx1 > tx2) ? 0 : 1; }
            tmax = Mathf.Min(tmax, t2);
            if (tmin > tmax) return false;
        }

        // Y
        if (Mathf.Abs(d.y) < 1e-8f)
        {
            if (o.y < c.y - e.y || o.y > c.y + e.y) return false;
        }
        else
        {
            float ty1 = (c.y - e.y - o.y) / d.y; // Down
            float ty2 = (c.y + e.y - o.y) / d.y; // Up
            float t1 = Mathf.Min(ty1, ty2);
            float t2 = Mathf.Max(ty1, ty2);
            if (t1 > tmin) { tmin = t1; hitFace = (ty1 > ty2) ? 2 : 3; }
            tmax = Mathf.Min(tmax, t2);
            if (tmin > tmax) return false;
        }

        // Z
        if (Mathf.Abs(d.z) < 1e-8f)
        {
            if (o.z < c.z - e.z || o.z > c.z + e.z) return false;
        }
        else
        {
            float tz1 = (c.z - e.z - o.z) / d.z; // Back
            float tz2 = (c.z + e.z - o.z) / d.z; // Front
            float t1 = Mathf.Min(tz1, tz2);
            float t2 = Mathf.Max(tz1, tz2);
            if (t1 > tmin) { tmin = t1; hitFace = (tz1 > tz2) ? 4 : 5; }
            tmax = Mathf.Min(tmax, t2);
            if (tmin > tmax) return false;
        }

        if (tmax < 0f) return false; // 뒤쪽만 교차
        tHit = Mathf.Max(0f, tmin);
        face = hitFace;
        return true;
    }

    static Vector3 FaceNormal(int face)
    {
        switch (face)
        {
            case 0: return Vector3.left;
            case 1: return Vector3.right;
            case 2: return Vector3.down;
            case 3: return Vector3.up;
            case 4: return Vector3.back;
            default: return Vector3.forward; // 5
        }
    }

    static void GetFaceTangents(int face, out Vector3 u, out Vector3 v)
    {
        // 면의 두 접선 축(기준 공간)
        switch (face)
        {
            case 0: case 1: u = Vector3.forward; v = Vector3.up; return; // ±X면
            case 2: case 3: u = Vector3.right; v = Vector3.forward; return; // ±Y면
            default: u = Vector3.right; v = Vector3.up; return; // ±Z면
        }
    }

    static Vector3 ProjectPointToFace(Vector3 p, int face, Vector3 c, Vector3 e)
    {
        switch (face)
        {
            case 0: return new Vector3(c.x - e.x, p.y, p.z);
            case 1: return new Vector3(c.x + e.x, p.y, p.z);
            case 2: return new Vector3(p.x, c.y - e.y, p.z);
            case 3: return new Vector3(p.x, c.y + e.y, p.z);
            case 4: return new Vector3(p.x, p.y, c.z - e.z);
            default: return new Vector3(p.x, p.y, c.z + e.z);
        }
    }

    static Vector3 ClampPointToFaceBounds(Vector3 p, int face, Vector3 c, Vector3 e)
    {
        switch (face)
        {
            case 0:
            case 1: // X면 → Y,Z 클램프
                p.y = Mathf.Clamp(p.y, c.y - e.y, c.y + e.y);
                p.z = Mathf.Clamp(p.z, c.z - e.z, c.z + e.z);
                return p;
            case 2:
            case 3: // Y면 → X,Z
                p.x = Mathf.Clamp(p.x, c.x - e.x, c.x + e.x);
                p.z = Mathf.Clamp(p.z, c.z - e.z, c.z + e.z);
                return p;
            default:        // Z면 → X,Y
                p.x = Mathf.Clamp(p.x, c.x - e.x, c.x + e.x);
                p.y = Mathf.Clamp(p.y, c.y - e.y, c.y + e.y);
                return p;
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    // 기준공간 변환
    static Vector3 ToBasisPoint(Vector3 worldPoint, Transform basisParent)
    {
        return (basisParent) ? basisParent.InverseTransformPoint(worldPoint) : worldPoint;
    }
    static Vector3 FromBasisPoint(Vector3 basisPoint, Transform basisParent, bool toWorld)
    {
        if (!toWorld) return basisPoint;
        return (basisParent) ? basisParent.TransformPoint(basisPoint) : basisPoint;
    }

    static Vector3 AxisToVector(Axis3D axis)
    {
        switch (axis)
        {
            case Axis3D.X: return Vector3.right;
            case Axis3D.Y: return Vector3.up;
            default: return Vector3.forward;
        }
    }

    void EnsureArray()
    {
        if (positions == null || positions.Length != Mathf.Max(2, segments))
            positions = new Vector3[Mathf.Max(2, segments)];
    }

    // 디버그 박스 표시
    void OnDrawGizmosSelected()
    {
        if (!hasBounds) return;

        Transform basisParent = (boundsBasis == BoundsBasis.ParentLocal) ? transform.parent : null;
        Vector3 c = boxCenterBasis, e = boxExtentsBasis;

        Vector3[] P = new Vector3[8];
        P[0] = new Vector3(c.x - e.x, c.y - e.y, c.z - e.z);
        P[1] = new Vector3(c.x + e.x, c.y - e.y, c.z - e.z);
        P[2] = new Vector3(c.x + e.x, c.y + e.y, c.z - e.z);
        P[3] = new Vector3(c.x - e.x, c.y + e.y, c.z - e.z);
        P[4] = new Vector3(c.x - e.x, c.y - e.y, c.z + e.z);
        P[5] = new Vector3(c.x + e.x, c.y - e.y, c.z + e.z);
        P[6] = new Vector3(c.x + e.x, c.y + e.y, c.z + e.z);
        P[7] = new Vector3(c.x - e.x, c.y + e.y, c.z + e.z);

        for (int i = 0; i < 8; i++)
            P[i] = FromBasisPoint(P[i], basisParent, true);

        Gizmos.color = new Color(0, 1, 1, 0.6f);
        DrawEdge(0, 1); DrawEdge(1, 2); DrawEdge(2, 3); DrawEdge(3, 0);
        DrawEdge(4, 5); DrawEdge(5, 6); DrawEdge(6, 7); DrawEdge(7, 4);
        DrawEdge(0, 4); DrawEdge(1, 5); DrawEdge(2, 6); DrawEdge(3, 7);

        void DrawEdge(int a, int b) => Gizmos.DrawLine(P[a], P[b]);
    }
}
