// TentacleVerletChain.cs
using UnityEngine;
public interface IChainPos
{
    public Vector3[] Positions { get; }
}
/// 한 가닥의 포즈를 매 프레임 계산(Verlet + 노이즈 + 중심 SDF 충돌)
public class TentacleVerletChain : MonoBehaviour, IChainPos
{
    [Header("Chain")]
    [Range(4, 128)] public int segments = 12;
    public float segmentLength = 0.06f;
    [Range(0.2f, 1f)] public float stiffness = 0.8f;
    [Range(1, 16)] public int constraintIterations = 6;

    [Header("Motion / Noise")]
    public float noiseAmp = 0.8f;
    public float noiseFreq = 0.7f;
    [Range(0.9f, 0.999f)] public float damping = 0.98f;
    public Vector3 flow = new Vector3(0.3f, 0.2f, 0.1f); // 전역 흐름
    [Range(0f, 2f)] public float outwardBias = 0.0f;     // 바깥으로 뻗는 편향
    [Range(0.5f, 2f)] public float targetLenScale = 1f;  // 가닥 길이 스케일(랜덤화 용)

    [Header("Clump / Blob SDF")]
    public Transform blobCenter;
    public float attractRadius = 0.22f;   // 중심부 반경
    public float attractStrength = 6f;    // 중심 끌림
    public float repelStrength = 8f;      // 내부 침투 시 반발
    [Range(0f, 2f)] public float baseCurvature = 0.4f; // 기본 만곡(더 유기적으로)

    public  Vector3[] pos, prev;
    public Vector3[] Positions => pos;

    void Awake()
    {
        pos = new Vector3[segments];
        prev = new Vector3[segments];

        var root = transform.localPosition;
        for (int i = 0; i < segments; i++)
        {
            pos[i] = root + Vector3.up * (i * segmentLength);
            prev[i] = pos[i];
        }
    }

    public void Init(float damp)
    {
        damp = Mathf.Clamp(damp, 0.9f, 0.999f);
        damping = damp;
    }
    void Update()
    {
        float dt = Mathf.Clamp(Time.deltaTime, 0f, 0.033f);
        float segLen = segmentLength * targetLenScale;

        // (선택) 월드 바람을 쓰고 싶다면 부모 로컬로 변환
        // Vector3 flowLocal = (transform.parent)
        //     ? transform.parent.InverseTransformDirection(flow)  // flow가 "월드"라고 가정할 때
        //     : flow;

        for (int i = 1; i < segments; i++)
        {
            Vector3 cur = pos[i];
            Vector3 vel = (pos[i] - prev[i]) * damping;

            float w = (float)i / (segments - 1);
            float t = Time.time * noiseFreq + i * 0.15f;
            Vector3 noiseF = new Vector3(
                Perlin(t + 13.1f, t + 1.7f),
                Perlin(t + 5.2f, t + 8.9f),
                Perlin(t + 2.3f, t + 3.3f)
            ) * (noiseAmp * Mathf.Lerp(0.4f, 1.0f, w));

            Vector3 dirPrev = (pos[i] - pos[i - 1]).normalized;
            Vector3 refAxis = (Mathf.Abs(Vector3.Dot(dirPrev, Vector3.up)) > 0.95f)
                              ? Vector3.right : Vector3.up;
            Vector3 tangent = Vector3.Cross(refAxis, dirPrev).normalized;
            Vector3 curveF = tangent * baseCurvature;

            // ★ blobCenter를 부모 로컬로 변환 (부모가 null이면 곧장 position 사용)
            Vector3 blobLocal = Vector3.zero;
            if (blobCenter)
                blobLocal = (transform.parent)
                    ? transform.parent.InverseTransformPoint(blobCenter.position)
                    : blobCenter.position;

            Vector3 toC = blobLocal - cur;
            float dist = toC.magnitude + 1e-6f;
            Vector3 attract = toC.normalized * attractStrength;
            Vector3 repel = (dist < attractRadius)
                ? -toC.normalized * (repelStrength * (attractRadius - dist))
                : Vector3.zero;

            // ★ outwardBias도 부모 로컬 축으로
            Vector3 acc = noiseF
                        + /*flowLocal*/ flow
                        + Vector3.up * outwardBias
                        + attract + repel + curveF;

            Vector3 next = cur + vel + acc * dt * dt;
            prev[i] = cur;
            pos[i] = next;
        }

        // 제약/루트 고정 (부모 로컬)
        for (int iter = 0; iter < constraintIterations; iter++)
        {
            pos[0] = transform.localPosition;
            for (int i = 0; i < segments - 1; i++)
            {
                Vector3 a = pos[i];
                Vector3 b = pos[i + 1];
                Vector3 ab = b - a;
                float L = ab.magnitude + 1e-6f;
                float diff = (L - segLen);
                Vector3 corr = (diff / L) * ab * 0.5f * stiffness;

                if (i == 0) pos[i + 1] -= corr * 2f;
                else { pos[i] += corr; pos[i + 1] -= corr; }
            }
        }
    }


    static float Perlin(float x, float y) => Mathf.PerlinNoise(x, y) * 2f - 1f;
}
