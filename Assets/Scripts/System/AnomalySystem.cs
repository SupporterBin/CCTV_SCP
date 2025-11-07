using UnityEngine;

public class AnomalySystem : MonoBehaviour
{
    [Header("현재 맵에 배치되어있는 몬스터 /1번 왼쪽 /2번 가운데 /3번 오른쪽")]
    public GameObject[] monsters;

    //[Header("실행했던 가지고 있는 이벤트")]
    private BasicEventAnomaly[] anomalyEvents;

    //이상현상 남은 클리어 시간
    [HideInInspector] public float currentAnomalyClearTime;

    //가장 최근에 작동한 이상 현상.
    [HideInInspector] public int startAnomalyTime;

    [Header("이상현상이 발생했나요?")]
    public bool isAnomaly;

    [Header("이벤트 모음")]
    public BasicEventAnomaly[] eventAnomalys;

    BasicEventAnomaly currentEventAnomaly;
    public EventPlace currentEventPlace;
    public EventType currentEventType;

    private void Start()
    {
        AnomalyTimeSetting(30, 40);
    }

    // Update is called once per frame
    void Update()
    {
        //만약 게임이 정지 안해있으면
        if (GameManager.Instance.AllStopCheck())
        {
            return;
        }

        //이상현상 발생 중!
        if (isAnomaly)
        {
            currentAnomalyClearTime += Time.deltaTime; //이상현상 발생하면 이상현상 몇초동안 발생중인지 초 세기.

            int mapValue = 0;
            switch(currentEventPlace)
            {
                case EventPlace.LeftRoom:
                    mapValue = 0;
                    break;
                case EventPlace.CenterRoom:
                    mapValue = 1;
                    break;
                case EventPlace.RightRoom:
                    mapValue = 2;
                    break;
                case EventPlace.All:
                    mapValue = 10;
                    break;
                case EventPlace.None:
                    mapValue = 999;
                    break;
            }

            if(mapValue != 999 && mapValue != 10)
            {
                //StabilityManager.Instance?.StabilizationDown(2 * Time.deltaTime, mapValue); //이상현상 발생중에 얼마나 안정성 더 떨어뜨리기.
                for (int roomValue = 0; roomValue < 3; roomValue++) { StabilityManager.Instance.Stabilization_AnomalyTime_Update(mapValue, GameManager.Instance.daySystem.GetNowDay()); }
            }

            //일정이상동안 이상현상 해결 못한 경우.
            if (currentAnomalyClearTime >= 30) 
            {
                StabilityManager.Instance?.StabilizationDown(15, 0); //안전성 수치 한번 크게 떨어뜨리기 (몇 번방, 얼마나 떨어뜨릴지는 추가 코드 및 기획 필요)

                //이상현상 실패 처리 및 이상현상 깔려있는거 제거 및 초기화.
                startAnomalyTime = GameManager.Instance.daySystem.GetClock(); //발생한 시각 체크.

                Debug.Log("힝 이상현을 충분히 못막았어용..");

                currentEventAnomaly.Fail();
                currentAnomalyClearTime = 0;
                CleanAnomaly(25, 45);
            }
        }


        //이상현상 발생 시간 세기
        if (startAnomalyTime < GameManager.Instance.daySystem.GetClock() && !isAnomaly)
        {
            EventAnomalyStart();
        }
        
    }

    /// <summary>
    /// 다음 이상현상 초 세팅
    /// </summary>
    private void AnomalyTimeSetting(int min, int max)
    {
        if(startAnomalyTime == 0)
            startAnomalyTime = Random.Range(GameManager.Instance.daySystem.GetClock() + min, GameManager.Instance.daySystem.GetClock() + max);
        else
            startAnomalyTime = Random.Range(startAnomalyTime + min, startAnomalyTime + max);
    }

    /// <summary>
    /// 이상현상 이벤트 시작하기
    /// </summary>
    public void EventAnomalyStart()
    {
        Debug.Log("EventAnomalyStart 발생, 이상현상 발생");
        isAnomaly = true;
        startAnomalyTime = GameManager.Instance.daySystem.GetClock(); //발생한 시각 체크.

        currentEventAnomaly = eventAnomalys[Random.Range(0, eventAnomalys.Length)];
        currentEventPlace = currentEventAnomaly.eventPlace;
        currentEventType = currentEventAnomaly.Execute();
    }

    private void ProcessEventClear(EventType Type, int index)
    {
        if (!isAnomaly)
        {
            StabilityManager.Instance?.StabilizationDown(15, index);
            Debug.Log("이상현상이 아닌데 눌렀어요 -15");
            return;
        }

        if (Type == currentEventType && index == (int)currentEventPlace - 2)
        {
            //클리어
            isAnomaly = false;
            Debug.Log("야호 이상현을 충분히 막아냈어요");

            currentEventAnomaly.Clear();
            CleanAnomaly(25, 40);
        }
        else
        {
            StabilityManager.Instance?.StabilizationDown(10, index);
            Debug.Log("이런 이상현을 충분히 실패했어요");
            currentEventAnomaly.Fail();
            CleanAnomaly(25, 40);
        }
    }

    // 유니티 버튼 OnClick()에 연결할 함수들
    public void ClearCCTVSystemCheck(int index) { ProcessEventClear(EventType.CCTV_SystemCheck, index); }
    public void ClearCCTVResonance(int index) { ProcessEventClear(EventType.CCTV_Resonance, index); }
    public void ClearCCTVIncinerate(int index) { ProcessEventClear(EventType.CCTV_Incinerate, index); }
    public void ClearCCTVElectricity(int index) { ProcessEventClear(EventType.CCTV_Electricity, index); }
    public void ClearCCTVFoodRefeel(int index) { ProcessEventClear(EventType.CCTV_FoodRefeel, index); }
    public void ClearMission(int index) { ProcessEventClear(EventType.Mission, index); }

    private void CleanAnomaly(int NextEventTime_Min, int NextEventTime_Max)
    {
        currentEventAnomaly = null;
        isAnomaly = false;
        currentEventType = EventType.None;
        currentEventPlace = EventPlace.None;
        AnomalyTimeSetting(NextEventTime_Min, NextEventTime_Max);
    }
}