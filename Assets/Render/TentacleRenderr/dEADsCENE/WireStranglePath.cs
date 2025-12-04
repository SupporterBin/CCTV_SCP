using UnityEngine;

/// 전선이 플레이어 목까지 뻗고, 이후 카메라 주변을 나선형으로 감아 올라가는 경로를 만드는 스크립트
/// TentacleSplineTube 의 source 로 연결해서 메쉬를 렌더링한다.
public class WireStranglePath : MonoBehaviour, IChainPos
{
    [Header("Common")]
    [Min(4)] public int segments = 32;           // 뼈 개수
    public bool outputWorldSpace = true;

    [Tooltip("전선 뼈대 포지션들 (TentacleSplineTube 가 읽어감)")]
    public Vector3[] positions;
    public Vector3[] Positions => positions;

    [Header("참조")]
    public Transform wireRoot;          // 전선이 시작되는 지점(천장, 벽 등)
    public Transform neckTarget;        // 플레이어 목 부위 트랜스폼
    public Camera playerCamera;         // 플레이어 카메라

    [Header("Phase 1 - 목까지 뻗기")]
    public float approachTime = 0.8f;   // 목까지 도달하는 데 걸리는 시간
    public AnimationCurve approachCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Phase 2 - 카메라 나선 감기")]
    public float spiralTime = 1.2f;     // 카메라 주변을 다 감는 데 걸리는 시간
    public float faceDistance = 0.3f;   // 카메라 앞 어느 정도 거리에서 감길지
    public float spiralRadiusStart = 0.4f; // 처음 반경
    public float spiralRadiusEnd = 0.1f; // 마지막 반경 (목 조이는 느낌)
    public float spiralTurns = 2.5f; // 총 몇 바퀴를 도는지
    public float spiralHeight = 0.4f; // 위로 얼마나 올라갈지

    [Header("Auto Play")]
    public bool playOnStart = false;

    enum State { Idle, ApproachNeck, SpiralAroundCamera, Done }
    State state = State.Idle;

    float stateTimer = 0f;

    // 외부에서 상태 확인하고 싶을 때용
    public bool IsFinished => state == State.Done;

    void Awake()
    {
        EnsureArray();
        ResetToRoot();
        GetComponent<TentacleSplineTube>().source = this;
    }

    void Start()
    {
        if (playOnStart)
            Play();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            EnsureArray();
            if (state == State.Idle)
                ResetToRoot();
            else
                UpdateState(Time.deltaTime); // 에디터에서 미리보기용
            return;
        }

        EnsureArray();
        UpdateState(Time.deltaTime);
    }

    // ===================== 외부 제어 =====================

    /// <summary>
    /// 전선 연출 시작 (Phase1 → Phase2 순서대로 진행)
    /// </summary>
    public void Play()
    {
        state = State.ApproachNeck;
        stateTimer = 0f;
    }

    public void ForceFinish()
    {
        state = State.Done;
        stateTimer = 0f;
    }

    // ===================== 내부 로직 =====================

    void UpdateState(float deltaTime)
    {
        switch (state)
        {
            case State.Idle:
                ResetToRoot();
                break;

            case State.ApproachNeck:
                stateTimer += deltaTime;
                float t1 = Mathf.Clamp01(stateTimer / Mathf.Max(approachTime, 0.01f));
                float eval1 = (approachCurve != null) ? approachCurve.Evaluate(t1) : t1;
                BuildApproachNeck(eval1);

                if (t1 >= 1f)
                {
                    state = State.SpiralAroundCamera;
                    stateTimer = 0f;
                }
                break;

            case State.SpiralAroundCamera:
                stateTimer += deltaTime;
                float t2 = Mathf.Clamp01(stateTimer / Mathf.Max(spiralTime, 0.01f));
                BuildSpiralAroundCamera(t2);

                if (t2 >= 1f)
                {
                    state = State.Done;
                    stateTimer = 0f;
                }
                break;

            case State.Done:
                // 최종 상태 유지 (원하면 여기서 조이는 연출 더 추가 가능)
                BuildSpiralAroundCamera(1f);
                break;
        }
    }

    /// <summary>
    /// Phase1: wireRoot → neckTarget 로 선형으로 쭉 뻗어나가는 연출
    /// </summary>
    void BuildApproachNeck(float t)
    {
        if (wireRoot == null || neckTarget == null)
        {
            ResetToRoot();
            return;
        }

        Vector3 rootPos = wireRoot.position;
        Vector3 neckPos = neckTarget.position;

        // 현재 프레임에서 전선의 "끝(Tip)" 위치
        Vector3 tipPos = Vector3.Lerp(rootPos, neckPos, t);

        for (int i = 0; i < segments; i++)
        {
            float s = (segments <= 1) ? 0f : (i / (float)(segments - 1));
            Vector3 pWorld = Vector3.Lerp(rootPos, tipPos, s);

            positions[i] = outputWorldSpace
                ? pWorld
                : transform.InverseTransformPoint(pWorld);
        }
    }

    /// <summary>
    /// Phase2: 목 → 카메라 앞에서 나선형(헬릭스)으로 감기는 연출
    /// </summary>
    void BuildSpiralAroundCamera(float t)
    {
        if (wireRoot == null || neckTarget == null || playerCamera == null)
        {
            ResetToRoot();
            return;
        }

        Transform cam = playerCamera.transform;
        Vector3 neckPos = neckTarget.position;

        // 카메라 앞, 전선이 감기는 기준 중심
        Vector3 camCenter = cam.position + cam.forward * faceDistance;

        Vector3 camRight = Vector3.right; 
        Vector3 camUp = Vector3.forward;

        for (int i = 0; i < segments; i++)
        {
            float s = (segments <= 1) ? 0f : (i / (float)(segments - 1));

            // s: 0(목쪽) → 1(끝 부분)
            float q = s * t;  // 전체 진행도에 따라 실제로 사용되는 길이

            // 나선 각도, 반경, 높이
            float angle = spiralTurns * Mathf.PI * 2f * q;
            float radius = Mathf.Lerp(spiralRadiusStart, spiralRadiusEnd, q);
            float height = spiralHeight * q;

            // 카메라 기준 헬릭스 상의 좌표
            Vector3 helixPos =
                camCenter
                + Vector3.up * height
                + (camRight * Mathf.Cos(angle) + camUp * Mathf.Sin(angle)) * radius;

            // 목에서 헬릭스 위치로 서서히 빼오는 느낌
            Vector3 worldPos = Vector3.Lerp(neckPos, helixPos, q);

            positions[i] = outputWorldSpace
                ? worldPos
                : transform.InverseTransformPoint(worldPos);
        }
    }

    void ResetToRoot()
    {
        Vector3 rootPos = (wireRoot != null) ? wireRoot.position : transform.position;

        for (int i = 0; i < segments; i++)
        {
            positions[i] = outputWorldSpace
                ? rootPos
                : transform.InverseTransformPoint(rootPos);
        }
    }

    void EnsureArray()
    {
        int size = Mathf.Max(2, segments);
        if (positions == null || positions.Length != size)
            positions = new Vector3[size];
    }
}
