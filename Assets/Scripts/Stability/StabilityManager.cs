using System;
using UnityEngine;

public class StabilityManager : MonoBehaviour
{
    private static StabilityManager instance;
    public static StabilityManager Instance => instance;

    // 안정 수치 최대치
    static public float maxStability = 100;

    // 현재 안정수치들
    private float[] currentStability;
    public float[] CurrentStability => currentStability;

    [Header("평상 시에 떨어지는 안정수치 소모량 (초당)")]
    public float normalDownStabilityValue = 0.5f; // 값을 조금 키우고 Time.deltaTime을 곱하는 방식 추천

    [Header("Day별 이상현상 발생 시 '추가로' 빨라지는 소모량 (초당)")]
    // 예: 0번(1일차)엔 0.5, 4번(5일차)엔 2.0 등 점점 빨라지게 설정
    public float[] dayDownStabilityValue;

    [SerializeField, Header("가면 주워야하는 개수 Day별")]
    public float[] dayGetMaskValue;
    public float currentGetMask = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        currentStability = new float[3];
        currentStability[0] = maxStability;
        currentStability[1] = maxStability;
        currentStability[2] = maxStability;
    }

    bool dam = true;
    private void Update()
    {
        // (기존 프로토콜 실행 로직 동일)
        if (currentStability[2] <= 0 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(0);
        }
        else if (currentStability[1] <= 1 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(1);
        }
        else if (currentStability[0] <= 1 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(2);
        }
    }

    // 단발성 감소 (오답 패널티 등)
    public void StabilizationDown(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] -= value, 0f, maxStability);
    }

    public void StabilizationUp(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] += value, 0f, maxStability);
    }

    // [변경] 통합된 안정도 감소 로직
    // isAnomalyActive가 true면 '기본 속도 + Day별 추가 속도'로 깎입니다.
    public void UpdateStabilityDrain(int roomIndex, int dayIndex, bool isAnomalyActive)
    {
        // 1. 기본 감소 속도
        float drainRate = normalDownStabilityValue;

        // 2. 이상현상이 있으면 Day별 설정값만큼 속도 '추가' (가속)
        if (isAnomalyActive)
        {
            // 배열 범위 체크 안전장치
            if (dayDownStabilityValue != null && dayIndex < dayDownStabilityValue.Length)
            {
                drainRate += dayDownStabilityValue[dayIndex];
            }
        }

        // 3. 최종 감소 적용 (FixedUpdate에서 호출되므로 fixedDeltaTime 사용)
        currentStability[roomIndex] = Mathf.Clamp(currentStability[roomIndex] -= drainRate * Time.fixedDeltaTime, 0f, maxStability);
    }
}