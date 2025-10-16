using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TentacleVerlet : MonoBehaviour
{
    [Header("Chain")]
    public int segments = 24;
    public float segmentLength = 0.06f;
    public float stiffness = 0.8f;          // 길이 보존 강도(제약 반복과 함께 조절)
    public int constraintIterations = 6;

    [Header("Motion")]
    public float noiseAmp = 0.8f;
    public float noiseFreq = 0.7f;
    public float damping = 0.98f;
    public Vector3 windDir = new Vector3(0.3f, 0.2f, 0.1f);

    [Header("Clump / Blob")]
    public Transform blobCenter;
    public float attractRadius = 0.2f;       // 뭉치 중심 반경(짧을수록 꽉 뭉침)
    public float attractStrength = 6f;       // 중심 끌어당김
    public float repelStrength = 8f;         // 블롭 내부 침투 시 반발
    public float outwardBias = 0f;           // 이 값을 크게 주면 ‘밖으로 뻗는’ 촉수 느낌

    Vector3[] pos, prev;
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
        pos = new Vector3[segments];
        prev = new Vector3[segments];

        // 초기화: 루트는 현재 위치에, 나머지는 뒤로 이어 붙이기
        var root = transform.position;
        for (int i = 0; i < segments; i++)
        {
            pos[i] = root - transform.up * (i * segmentLength);
            prev[i] = pos[i];
        }
    }

    void Update()
    {
        float dt = Mathf.Clamp(Time.deltaTime, 0, 0.033f);

        // 1) 적분(Verlet)
        for (int i = 1; i < segments; i++) // 루트(0)는 고정
        {
            Vector3 cur = pos[i];
            Vector3 vel = (pos[i] - prev[i]) * damping;

            // 컬/퍼린 노이즈 힘 (꿈틀)
            float t = Time.time * noiseFreq + i * 0.13f;
            Vector3 noiseF =
                new Vector3(
                    Perlin(t + 13.1f, t + 1.7f),
                    Perlin(t + 5.2f, t + 8.9f),
                    Perlin(t + 2.3f, t + 3.3f)
                ) * noiseAmp;

            // 약간의 바람/전역 흐름 + 바깥으로 뻗는 편향
            Vector3 flow = windDir * 0.4f + transform.up * outwardBias;

            // 블롭 끌어당김/반발
            Vector3 toCenter = (blobCenter ? blobCenter.position - cur : -cur);
            float dist = toCenter.magnitude + 1e-6f;
            Vector3 attract = toCenter.normalized * attractStrength;
            Vector3 repel = Vector3.zero;
            if (dist < attractRadius) // 중심 구 내부에 파고들면 밀어냄
                repel = -toCenter.normalized * (repelStrength * (attractRadius - dist));

            Vector3 acc = noiseF + flow + attract + repel;
            Vector3 next = cur + vel + acc * dt * dt;
            prev[i] = cur;
            pos[i] = next;
        }

        // 2) 제약(길이 유지 + 루트 고정)
        for (int iter = 0; iter < constraintIterations; iter++)
        {
            pos[0] = transform.position; // 루트 고정

            for (int i = 0; i < segments - 1; i++)
            {
                Vector3 a = pos[i];
                Vector3 b = pos[i + 1];
                Vector3 ab = b - a;
                float L = ab.magnitude + 1e-6f;
                float diff = (L - segmentLength);
                Vector3 corr = (diff / L) * ab * 0.5f * stiffness;

                if (i == 0)
                {
                    // 루트는 고정, 다음만 보정
                    pos[i + 1] -= corr * 2f;
                }
                else
                {
                    pos[i] += corr;
                    pos[i + 1] -= corr;
                }
            }
        }

        // 3) 라인 업데이트(프로토타입용). 실서비스는 스키닝/튜브 메시 추천
        lr.positionCount = segments;
        lr.SetPositions(pos);
        // 오브젝트의 up을 팁 방향으로 맞추고 싶다면:
        Vector3 dir = (pos[1] - pos[0]).normalized;
        transform.up = dir;
    }

    // 간단한 2D 퍼린 샘플(대체 가능)
    static float Perlin(float x, float y) => Mathf.PerlinNoise(x, y) * 2f - 1f;
}
