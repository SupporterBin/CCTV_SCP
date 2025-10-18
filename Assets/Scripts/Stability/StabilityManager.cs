using UnityEngine;

public class StabilityManager : MonoBehaviour
{
    private static StabilityManager instance;
    public static StabilityManager Instance => instance;

    //현재 이상현상 최대치
    static public float maxStability = 100;
    //현재 이상현상 수치
    private float[] currentStability;
    public float[] CurrentStability => currentStability;
    //이상현상 발생함?
    public bool isAbnormal;
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

    //안정화 수치 조절
    //-> 다운, 상승 이벤트 때문에 나눠둠.
    public void StabilizationDown(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] -= value, 0f, maxStability);
    }

    public void StabilizationUp(float value, int index)
    {
        currentStability[index] = Mathf.Clamp(currentStability[index] += value, 0f, maxStability);
    }
}
