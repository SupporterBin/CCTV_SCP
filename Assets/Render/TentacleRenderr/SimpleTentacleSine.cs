using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Axis3D { X, Y, Z }

[ExecuteAlways]
public class SimpleTentacleSine : MonoBehaviour, IChainPos
{
    [Header("Common Shape")]
    [Min(2)] public int segments = 32;          // 뼈 개수(샘플 수)
    [Tooltip("true면 Positions를 월드좌표로, false면 로컬좌표로 출력")]
    public bool outputWorldSpace = true;
    [Tooltip("뼈대 위치 출력 (TentacleSplineTube가 읽어감)")]
    public Vector3[] positions;                 // 체인 포인트들
    public Vector3[] Positions => positions;

    // ---------- Sine 모드 ----------
    [Header("Sine Mode")]
    public bool useNavPathMode = false;         // false면 기존 Sine, true면 NavMesh 경로 모드

    [Min(0.001f)] public float totalLength = 2f;// 전체 길이
    public Axis3D alongAxis = Axis3D.Z;         // 진행 방향
    public bool reverseAlong = false;           // 진행 방향 반전

    public Axis3D lateralAxis = Axis3D.X;       // 좌우 파동 방향
    [Min(0f)] public float amplitude = 0.1f;    // 파동 진폭
    [Min(0.001f)] public float wavelength = 1f; // 파동 파장
    public float speed = 1f;                    // 파동 이동 속도
    public float phaseOffset = 0f;              // 위상 오프셋
    [Range(0f, 1f)] public float tipTaper = 0.3f;// 끝 쪽 진폭 줄이기 비율

    // ---------- NavMesh 경로 모드 ----------
    [Header("NavMesh Path Mode")]
    public NavMeshAgent navAgent;              // 경로 계산용 Agent (updatePosition=false 권장)
    public Transform navTarget;                // 가고 싶은 목표

    [Tooltip("최소 이동 속도(거리/초). 너무 가까울 때도 이 값보다 느려지지 않음.")]
    public float headMoveSpeed = 3f;           // 최소 속도

    public float waitAtTargetTime = 0.5f;      // 도착 후 대기 시간
    public bool autoReturnAfterWait = false;   // true면 대기 후 자동 복귀

    [Tooltip("목표까지 이동하는데 걸리길 원하는 시간(초). 멀면 이 시간에 맞춰 더 빨라짐.")]
    public float desiredTravelTime = 1.0f;

    [Tooltip("복귀(루트로 돌아갈 때) 걸리길 원하는 시간(초). 멀면 이 시간에 맞춰 더 빨라짐.")]
    public float desiredReturnTime = 0.5f;

    enum PathState { None, MovingForward, Waiting, Returning }
    PathState pathState = PathState.None;

    // 경로 샘플
    List<Vector3> pathPoints = new List<Vector3>(); // 0 = 루트, 마지막 = 목표 근처
    float[] cumulativeLengths;                      // 각 포인트까지 누적 거리
    float pathTotalLength;                          // 전체 경로 길이
    float headDistance;                             // 루트에서 머리까지 현재 거리
    float targetDistance;                           // 현재 상태에서 목표로 하는 거리
    float waitTimer;

    // 길이에 따라 계산된 실제 속도
    float currentForwardSpeed;
    float currentReturnSpeed;

    // 촉수 끝 위치정보 (끝에서 한 칸 앞을 사용 중)
    public Vector3 TipWorldPosition
    {
        get
        {
            if (positions == null || positions.Length == 0)
                return transform.position;

            int last = positions.Length - 2;
            last = Mathf.Clamp(last, 0, positions.Length - 1);

            if (outputWorldSpace)
                return positions[last];
            else
                return transform.TransformPoint(positions[last]);
        }
    }

    // ======================================================
    // 기본 생명주기
    // ======================================================
    void OnEnable()
    {
        EnsureArray();
        if (!useNavPathMode)
        {
            RebuildPositionsSine(0f);
        }
        else
        {
            ResetPathState();
            RebuildPositionsPathMode(); // 초기화용
        }
    }

    void OnValidate()
    {
        if (lateralAxis == alongAxis)
            lateralAxis = (alongAxis == Axis3D.Z) ? Axis3D.X : Axis3D.Z;

        EnsureArray();

        if (!useNavPathMode)
            RebuildPositionsSine(0f);
        else
        {
            // 에디터에서 경로가 없으면 그냥 루트에 모아두기
            ResetPathState();
            RebuildPositionsPathMode();
        }
    }

    void Update()
    {
        // 에디터 미재생 상태에서는 Sine만 미리보기
        if (!Application.isPlaying)
        {
            if (!useNavPathMode)
                RebuildPositionsSine(0f);
            else
                RebuildPositionsPathMode(); // 경로 없으면 전부 루트에
            return;
        }

        if (useNavPathMode)
        {
            UpdatePathMode();
        }
        else
        {
            float t = Time.time;
            RebuildPositionsSine(t);
        }
    }

    // ======================================================
    // Sine 기반 체인 생성 (원본 로직 유지)
    // ======================================================
    void RebuildPositionsSine(float timeSeconds)
    {
        if (segments < 2) segments = 2;
        EnsureArray();

        float segLen = (segments > 1) ? (totalLength / (segments - 1)) : 0f;

        Vector3 a = AxisToVector(alongAxis) * (reverseAlong ? -1f : 1f);
        Vector3 l = AxisToVector(lateralAxis);

        float k = Mathf.PI * 2f / Mathf.Max(0.0001f, wavelength);
        float w = Mathf.PI * 2f * speed;
        float phi = phaseOffset;

        Vector3 rootLocal = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float s = segLen * i;
            Vector3 baseLocal = rootLocal + a * s;

            float tNorm = (segments <= 1) ? 0f : (i / (float)(segments - 1));
            float amp = Mathf.Lerp(amplitude, amplitude * (1f - tipTaper), tNorm);

            float theta = k * s - w * timeSeconds + phi;
            Vector3 offsetLocal = l * (amp * Mathf.Sin(theta));

            Vector3 pLocal = baseLocal + offsetLocal;

            positions[i] = outputWorldSpace
                ? transform.TransformPoint(pLocal)
                : pLocal;
        }
    }

    // ======================================================
    // NavMesh 경로 모드
    // ======================================================

    /// <summary>
    /// 외부에서 호출: navTarget까지 경로 계산하고 전진 시작
    /// </summary>
    public void StartPathToTarget(Transform overrideTarget = null)
    {
        if (navAgent == null)
        {
            Debug.LogWarning($"{name}: NavMeshAgent가 설정되지 않았습니다.");
            return;
        }

        Transform tgt = overrideTarget != null ? overrideTarget : navTarget;
        if (tgt == null)
        {
            Debug.LogWarning($"{name}: navTarget이 설정되지 않았습니다.");
            return;
        }

        // 에이전트를 루트 위치로 워프해서 경로 계산 일치시키기
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;
        navAgent.Warp(transform.position);

        NavMeshPath navPath = new NavMeshPath();
        if (!navAgent.CalculatePath(tgt.position, navPath) || navPath.corners.Length == 0)
        {
            Debug.LogWarning($"{name}: NavMesh 경로 계산 실패.");
            return;
        }

        BuildPathFromNavMesh(navPath);
        headDistance = 0f;
        targetDistance = pathTotalLength;
        pathState = PathState.MovingForward;
        waitTimer = 0f;
    }

    /// <summary>
    /// 외부에서 호출: 기존 경로를 그대로 되돌아오기
    /// </summary>
    public void StartReturn()
    {
        if (pathPoints == null || pathPoints.Count < 2 || pathTotalLength <= 0f)
        {
            Debug.LogWarning($"{name}: 되돌아올 경로가 없습니다.");
            return;
        }

        targetDistance = 0f;
        pathState = PathState.Returning;
        waitTimer = 0f;
    }

    void ResetPathState()
    {
        pathPoints.Clear();
        pathTotalLength = 0f;
        headDistance = 0f;
        targetDistance = 0f;
        pathState = PathState.None;
        currentForwardSpeed = headMoveSpeed;
        currentReturnSpeed = headMoveSpeed;
    }

    void BuildPathFromNavMesh(NavMeshPath navPath)
    {
        pathPoints.Clear();

        // 0번은 항상 촉수 루트 위치
        Vector3 rootPos = transform.position;
        pathPoints.Add(rootPos);

        // NavMeshAgent에서 나온 corner들 추가
        var corners = navPath.corners;
        for (int i = 0; i < corners.Length; i++)
        {
            pathPoints.Add(corners[i]);
        }

        int count = pathPoints.Count;
        if (count < 2)
        {
            pathTotalLength = 0f;
            cumulativeLengths = null;
            currentForwardSpeed = headMoveSpeed;
            currentReturnSpeed = headMoveSpeed;
            return;
        }

        cumulativeLengths = new float[count];
        cumulativeLengths[0] = 0f;
        float total = 0f;
        for (int i = 1; i < count; i++)
        {
            float seg = Vector3.Distance(pathPoints[i - 1], pathPoints[i]);
            total += seg;
            cumulativeLengths[i] = total;
        }
        pathTotalLength = total;

        // === 길이에 따라 실제 속도 계산 ===
        float minSpeed = headMoveSpeed;

        float travelTime = Mathf.Max(desiredTravelTime, 0.01f);
        float speedByTimeForward = pathTotalLength / travelTime;
        currentForwardSpeed = Mathf.Max(minSpeed, speedByTimeForward);

        float returnTime = Mathf.Max(desiredReturnTime, 0.01f);
        float speedByTimeReturn = pathTotalLength / returnTime;
        currentReturnSpeed = Mathf.Max(minSpeed, speedByTimeReturn);
    }

    void UpdatePathMode()
    {
        EnsureArray();

        if (pathState == PathState.None || pathTotalLength <= 0f)
        {
            // 경로 없으면 루트에 모아두기
            RebuildPositionsPathMode();
            return;
        }

        if (pathState == PathState.MovingForward || pathState == PathState.Returning)
        {
            float speed = (pathState == PathState.Returning)
                ? currentReturnSpeed
                : currentForwardSpeed;

            headDistance = Mathf.MoveTowards(
                headDistance,
                targetDistance,
                speed * Time.deltaTime
            );

            // 도착 처리
            if (Mathf.Approximately(headDistance, targetDistance))
            {
                if (pathState == PathState.MovingForward)
                {
                    pathState = PathState.Waiting;
                    waitTimer = waitAtTargetTime;
                }
                else if (pathState == PathState.Returning)
                {
                    pathState = PathState.None;
                }
            }
        }
        else if (pathState == PathState.Waiting)
        {
            if (waitAtTargetTime > 0f)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    if (autoReturnAfterWait)
                    {
                        StartReturn();
                    }
                    else
                    {
                        // 그냥 도착 상태 유지
                        waitTimer = 0f;
                    }
                }
            }
        }

        RebuildPositionsPathMode();
    }

    // 현재 headDistance를 기준으로 positions[] 채우기
    void RebuildPositionsPathMode()
    {
        if (segments < 2) segments = 2;
        EnsureArray();

        // 경로가 없으면 전부 루트 위치에
        if (pathPoints == null || pathPoints.Count < 2 || pathTotalLength <= 0f)
        {
            Vector3 root = outputWorldSpace ? transform.position : Vector3.zero;
            for (int i = 0; i < segments; i++)
                positions[i] = root;
            return;
        }

        float maxDist = Mathf.Max(headDistance, 0.0001f);

        for (int i = 0; i < segments; i++)
        {
            float tNorm = (segments <= 1) ? 0f : (i / (float)(segments - 1));
            float d = maxDist * tNorm;
            Vector3 pWorld = GetPointOnPath(d);

            positions[i] = outputWorldSpace
                ? pWorld
                : transform.InverseTransformPoint(pWorld);
        }
    }

    // 루트에서 특정 거리 d 위치의 월드 좌표를 반환
    Vector3 GetPointOnPath(float distance)
    {
        if (pathPoints == null || pathPoints.Count == 0)
            return transform.position;

        if (pathPoints.Count == 1 || pathTotalLength <= 0f)
            return pathPoints[0];

        float d = Mathf.Clamp(distance, 0f, pathTotalLength);

        int count = pathPoints.Count;
        for (int i = 1; i < count; i++)
        {
            float prevLen = cumulativeLengths[i - 1];
            float curLen = cumulativeLengths[i];

            if (d <= curLen)
            {
                float segLen = curLen - prevLen;
                float t = segLen > 0f ? (d - prevLen) / segLen : 0f;
                return Vector3.Lerp(pathPoints[i - 1], pathPoints[i], t);
            }
        }

        return pathPoints[count - 1];
    }

    void EnsureArray()
    {
        int size = Mathf.Max(2, segments);
        if (positions == null || positions.Length != size)
            positions = new Vector3[size];
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
}
