using UnityEngine;

public class AnomalySystem : MonoBehaviour
{
    public float globalCheckTime;

    //클리어 타임 세기
    public float clearTime;

    [Header("이상현상 카운트(해당 시간 넘기면 작동)")]
    public float anomalyStartTime = 10;

    [Header("이상현상이 발생했나요?")]
    public bool isAnomaly = false;

    [Header("이벤트 모음")]
    public BasicEventAnomaly[] eventAnomalys;

    BasicEventAnomaly currentEventAnomaly;
    public EventPlace currentEventPlace;
    public EventType currentEventType;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.AllStopCheck())
        {
            if (!isAnomaly) { globalCheckTime += Time.deltaTime; }
            else clearTime += Time.deltaTime; 

            //이상현상 발생 시간 세기
            if (globalCheckTime > anomalyStartTime)
            {
                globalCheckTime = 0;
                anomalyStartTime = AnomalyTimeSetting(80, 100);
                EventAnomalyStart();
            }

            if(isAnomaly)
            {
                StabilityManager.Instance?.StabilizationDown(10*Time.deltaTime, 0);
            }

            if (isAnomaly && clearTime >= 60)
            {
                //이상현상 클리어 실패 코드
                //안정수치 감소

                //실패했을때 더떨어지게
                //StabilityManager.Instance?.StabilizationDown(10, 0);
                isAnomaly = false;
                clearTime = 0;
            }
        }
    }

    float AnomalyTimeSetting(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void EventAnomalyStart()
    {
        isAnomaly = true;
        currentEventAnomaly = eventAnomalys[Random.Range(0, eventAnomalys.Length)];

        currentEventPlace = currentEventAnomaly.eventPlace;
        currentEventType = currentEventAnomaly.Execute();
    }

    public void EventAnomalyClear(int Type)
    {
        EventType value = (EventType)Type;


        if (value == currentEventType)
        {
            //클리어
            isAnomaly = false;
            clearTime = 0;
            Debug.Log("야호 이상현을 충분히 막아냈어요");
            currentEventAnomaly.Clear();

            currentEventAnomaly = null;
            currentEventPlace = EventPlace.None;
            currentEventType = EventType.None;
        }
        else
        {
            Debug.Log("이런 이상현을 충분히 실패했어요");
            //노클리어 디버프
        }
    }
}