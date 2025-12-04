using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StabilityManager : MonoBehaviour
{
    private static StabilityManager instance;
    public static StabilityManager Instance => instance;

    // 안정 수치 최대치 (100)
    static public float maxStability = 100;

    // 현재 안정수치들
    private float[] currentStability;
    public float[] CurrentStability => currentStability;

    [Header("일차별 초당 기본 감소량 (1일차: 0.37, 2일차: 0.392 ...)")]
    public float[] dayBaseDecayRates;

    [Header("이상현상 활성화 중일 때 추가되는 초당 감소량")]
    public float activeAnomalyExtraDrain = 1.0f;

    [Header("이상현상 대처 실패 시 즉시 감소하는 양")]
    public float failureDropAmount = 8.0f;

    [Header("=== 가면 설정 ===")]
    [Header("Day 별 주워야하는 가면 개수")]
    public float[] dayGetMaskValue;
    [HideInInspector]
    public float currentGetMask = 0;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        currentStability = new float[3];
        // 시작 시 모든 방 100으로 초기화
        for (int i = 0; i < 3; i++) currentStability[i] = maxStability;
    }

    bool dam = true;
    bool uDead = false;
    private void Update()
    {
        // 프로토콜 시스템 발동 로직 (기존 유지)
        if (currentStability[2] <= 0 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(2);
        }
        else if (currentStability[1] <= 1 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(1);
        }
        else if (currentStability[0] <= 1 && dam)
        {
            dam = false;
            ProtocolSystem.instance.StartProtocol(0);
        }

        if (dam || uDead) return;
        for(int i = 0; i<3; i++)
        {
            if (currentStability[i] <= 1)
            {

                if(!uDead)
                {
                    GameManager.Instance.anomalySystem.specialObjects[1].GetComponent<Animator>().Play("On");
                    uDead = true;
                }
            }
        }
    }

    // 단발성 감소 (대처 실패, 오답 등)
    public void StabilizationDown(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] - value, 0f, maxStability);
    }

    public void StabilizationUp(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] + value, 0f, maxStability);
    }

    // =========================================================
    // [핵심 변경] 이미지 로직 적용된 안정도 감소 함수
    // =========================================================
    public void UpdateStabilityDrain(int roomIndex, int dayIndex, bool isAnomalyActive)
    {
        // 1. 현재 일차에 맞는 기본 감소량 가져오기 (예: 1일차면 0.37)
        float currentDrainRate = 0f;

        if (dayBaseDecayRates != null && dayIndex < dayBaseDecayRates.Length)
        {
            currentDrainRate = dayBaseDecayRates[dayIndex];
        }
        else
        {
            // 배열 인덱스 에러 방지용 기본값 (혹시 설정 안했을 경우)
            currentDrainRate = 0.37f;
        }

        // 2. 이상현상 활성화 시 +1 추가 (이미지 로직)
        if (isAnomalyActive)
        {
            currentDrainRate += activeAnomalyExtraDrain;
        }

        // 3. 최종 적용 (FixedUpdate에서 호출되므로 Time.fixedDeltaTime 곱함)
        currentStability[roomIndex] = Mathf.Clamp(currentStability[roomIndex] -= currentDrainRate * Time.fixedDeltaTime, 0f, maxStability);
    }

    public void ProtocolSuccess()
    {
        dam = true;
    }
}