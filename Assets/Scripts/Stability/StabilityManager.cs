using System;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.Rendering.DebugUI;

public class StabilityManager : MonoBehaviour
{
    private static StabilityManager instance;
    public static StabilityManager Instance => instance;

    //안정 수치 최대치
    static public float maxStability = 100;

    //현재 안정수치들
    private float[] currentStability;
    public float[] CurrentStability => currentStability;

    [SerializeField, Header("0 = 1일, 4 = 5일, Day 별 이상현상 발생시 소모량 작성.")]
    private float[] dayDownStabilityValue;

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
        if (currentStability[2] <= 0 && dam)
        {
            dam = false;
            ExecutionTimeLineManager.instance.PlayExecutionTimeline(0);
        }
        else if (currentStability[1] <= 1 && dam)
        {
            dam = false;
            ExecutionTimeLineManager.instance.PlayExecutionTimeline(1);
        }
    }

    //����ȭ ��ġ ����
    //-> �ٿ�, ��� �̺�Ʈ ������ ������.
    public void StabilizationDown(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] -= value, 0f, maxStability);
    }

    public void StabilizationUp(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] += value, 0f, maxStability);
    }
    
    public void Stabilization_AnomalyTime_Update(int roomNum, int dayNum)
    {
        currentStability[roomNum] = Mathf.Clamp(currentStability[roomNum] -= dayDownStabilityValue[dayNum] * Time.deltaTime, 0f, maxStability);
    }
}
