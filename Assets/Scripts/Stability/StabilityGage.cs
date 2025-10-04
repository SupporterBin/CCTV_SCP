using UnityEngine;

public class StabilityGage : MonoBehaviour
{
    //현재 이상현상 최대치
    public float maxStability = 100;
    //현재 이상현상 수치
    public float currentStability;
    
    //이상현상 발생함?
    public bool isAbnormal;

    //안정화 수치 조절
    //-> 다운, 상승 이벤트 때문에 나눠둠.
    public void StabilizationDown(float value)
    {
        currentStability = Mathf.Clamp(currentStability -= value, 0f, maxStability);
    }

    public void StabilizationUp(float value)
    {
        currentStability = Mathf.Clamp(currentStability += value, 0f, maxStability);
    }
}