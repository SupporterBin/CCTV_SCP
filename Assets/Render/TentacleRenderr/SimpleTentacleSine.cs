// SimpleTentacleSine.cs
// 단순 사인파 기반 위치 생성기: 오브젝트(로컬 기준) 한 축으로 뻗어 분절별 위치를 계산해 positions[]에 저장
using UnityEngine;

    public enum Axis3D { X, Y, Z }
[ExecuteAlways]
public class SimpleTentacleSine : MonoBehaviour, IChainPos
{

    [Header("Shape")]
    [Min(2)] public int segments = 32;          // 분절 수 (루트 포함)
    [Min(0.001f)] public float totalLength = 2f;// 전체 길이
    public Axis3D alongAxis = Axis3D.Z;         // 뻗는 주축 (오브젝트 로컬 기준)
    public bool reverseAlong = false;           // 음의 방향으로 뻗을지

    [Header("Wave (sin)")]
    public Axis3D lateralAxis = Axis3D.X;       // 파동으로 흔들릴 축(주축과 수직)
    [Min(0f)] public float amplitude = 0.1f;    // 진폭(최대 변위)
    [Min(0.001f)] public float wavelength = 1f; // 파장(길이 기준, m/사이클)
    public float speed = 1f;                    // 진행 속도(사이클/초)
    public float phaseOffset = 0f;              // 전체 위상 오프셋(라디안)
    [Range(0f, 1f)] public float tipTaper = 0.3f;// 팁 쪽으로 진폭을 줄이는 비율(0=균일)

    [Header("Output")]
    public bool outputWorldSpace = true;        // true면 월드 좌표로 저장, false면 로컬 좌표로 저장
    [Tooltip("계산된 분절 위치(루트→팁 순)")]
    public Vector3[] positions;                 // 최종 위치 결과

    // 편의 프로퍼티
    public Vector3[] Positions => positions;

    void OnEnable()
    {
        EnsureArray();
        RebuildPositions(0f); // 에디터에서도 초기 모습 보이게
    }

    void OnValidate()
    {
        // 주축과 횡축이 같으면 자동 보정
        if (lateralAxis == alongAxis)
            lateralAxis = (alongAxis == Axis3D.Z) ? Axis3D.X : Axis3D.Z;

        EnsureArray();
        RebuildPositions(0f);
    }

    void Update()
    {
        float t = Application.isPlaying ? Time.time : 0f;
        RebuildPositions(t);
    }

    // === Core ===
    void RebuildPositions(float timeSeconds)
    {
        if (segments < 2) segments = 2;
        EnsureArray();

        float segLen = (segments > 1) ? (totalLength / (segments - 1)) : 0f;

        // 로컬 기준 단위 축 벡터
        Vector3 a = AxisToVector(alongAxis) * (reverseAlong ? -1f : 1f);
        Vector3 l = AxisToVector(lateralAxis); // 주축과 수직인 축으로 설정되어야 자연스러움

        // 진행 파동: 2π * (s/λ - f*t) + φ
        float k = Mathf.PI * 2f / Mathf.Max(0.0001f, wavelength); // 파수
        float w = Mathf.PI * 2f * speed;                           // 각주파수
        float phi = phaseOffset;

        // 루트(로컬 원점)에서 시작
        Vector3 rootLocal = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float s = segLen * i; // 루트부터의 거리
            // 기본(뻗는) 위치: 루트 + 주축 * s
            Vector3 baseLocal = rootLocal + a * s;

            // 진폭 테이퍼(팁으로 갈수록 줄어드는 옵션)
            float tNorm = (segments <= 1) ? 0f : (i / (float)(segments - 1)); // 0~1
            float amp = Mathf.Lerp(amplitude, amplitude * (1f - tipTaper), tNorm);

            // 사인 변위(횡축 방향)
            float theta = k * s - w * timeSeconds + phi;
            Vector3 offsetLocal = l * (amp * Mathf.Sin(theta));

            Vector3 pLocal = baseLocal + offsetLocal;

            positions[i] = outputWorldSpace
                ? transform.TransformPoint(pLocal)  // 월드로 변환 저장
                : pLocal;                           // 로컬로 그대로 저장
        }
    }

    void EnsureArray()
    {
        if (positions == null || positions.Length != Mathf.Max(2, segments))
            positions = new Vector3[Mathf.Max(2, segments)];
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
