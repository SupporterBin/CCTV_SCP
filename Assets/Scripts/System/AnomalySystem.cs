using UnityEngine;

public class AnomalySystem : MonoBehaviour
{
    public float globalCheckTime;

    //클리어 타임 세기
    public float clearTime;

    [Header("이상현상 카운트(해당 시간 넘기면 작동)")]
    public float anomalyStartTime;

    [Header("이상현상이 발생했나요?")]
    public bool isAnomaly;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.AllStopCheck())
        {
            if (!isAnomaly) { globalCheckTime += Time.deltaTime; }

            if (globalCheckTime > anomalyStartTime)
            {
                isAnomaly = true;
                globalCheckTime = 0;
                anomalyStartTime = AnomalyTimeSetting(80, 100);
            }

            if (isAnomaly && clearTime <= 60)
            {
                //이상현상 클리어 실패 코드
                //안정수치 감소
            }
        }
    }

    float AnomalyTimeSetting(float min, float max)
    {
        return Random.Range(min, max);
    }
}
