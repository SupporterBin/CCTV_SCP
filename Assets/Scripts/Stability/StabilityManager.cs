using UnityEngine;
using UnityEngine.Playables;

public class StabilityManager : MonoBehaviour
{
    private static StabilityManager instance;
    public static StabilityManager Instance => instance;

    //���� �̻����� �ִ�ġ
    static public float maxStability = 100;
    //���� �̻����� ��ġ
    private float[] currentStability;
    public float[] CurrentStability => currentStability;

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
}
