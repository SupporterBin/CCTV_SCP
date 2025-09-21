using UnityEngine;

public class StabilityGage : MonoBehaviour
{
    //안정화 최대 수치
    public float maxStability = 100;
    //안정화 수치
    public float currentStability;
    
    //이상현상이 일어났나요?
    public bool isAbnormal;


    //왜 따로 +/- 분리함?
    //-> 특수 효과 혹은 추가적인 코드가 필요할까봐
    public void StabilizationDown(float value)
    {
        currentStability = Mathf.Clamp(currentStability -= value, 0f, maxStability);
    }

    public void StabilizationUp(float value)
    {
        currentStability = Mathf.Clamp(currentStability += value, 0f, maxStability);
    }
}