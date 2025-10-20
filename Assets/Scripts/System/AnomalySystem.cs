using UnityEngine;

public class AnomalySystem : MonoBehaviour
{
    public float globalCheckTime;

    [Header("가지고 있는 이벤트")]
    public BasicEventAnomaly[] anomalyEvents;

    //이상현상 남은 클리어 시간
    [HideInInspector] public float currentClearTime;

    //[Header("이상현상 카운트(해당 시간 넘기면 작동)")]
    [HideInInspector] public int startAnomalyTime;
    private float setAnomalyTime;

    //"이상현상이 발생했나요?"
    public bool isAnomaly;

    [Header("이벤트 모음")]
    public BasicEventAnomaly[] eventAnomalys;

    BasicEventAnomaly currentEventAnomaly;
    public EventPlace currentEventPlace;
    public EventType currentEventType;

    private void Awake()
    {
        AnomalyTimeSetting(30, 40);
    }

    // Update is called once per frame
    void Update()
    {
        //만약 게임이 정지 안해있으면
        if (!GameManager.Instance.AllStopCheck())
        {
            return;
        }
        
        //이상현상 발생 중!
        if (isAnomaly)
        {
            currentClearTime = Time.deltaTime; //이상현상 발생하면 이상현상 몇초동안 발생중인지 초 세기.
            StabilityManager.Instance?.StabilizationDown(10 * Time.deltaTime, 0); //이상현상 발생중에 얼마나 안정성 더 떨어뜨리기.

            if(currentClearTime >= 60) //이상현상 감지 실패한 경우
            {
                StabilityManager.Instance?.StabilizationDown(10, 0); //안전성 수치 한번 크게 떨어뜨리기 (몇 번방, 얼마나 떨어뜨릴지는 추가 코드 및 기획 필요)
                isAnomaly = false;
                currentClearTime = 0;
            }
        }

        //이상현상 발생 시간 세기
        if (globalCheckTime > currentClearTime)
        {
            globalCheckTime = 0;
            AnomalyTimeSetting(80, 100);
            EventAnomalyStart();
        }
    }

    /// <summary>
    /// 다음 이상현상 초 세팅
    /// </summary>
    private void AnomalyTimeSetting(int min, int max)
    {
        startAnomalyTime = Random.Range(startAnomalyTime + min, startAnomalyTime + max);
    }

    /// <summary>
    /// 이상현상 이벤트 시작하기
    /// </summary>
    public void EventAnomalyStart()
    {
        isAnomaly = true;
        startAnomalyTime = GameManager.Instance.daySystem.GetClock(); //발생한 시각 체크.

        currentEventAnomaly = eventAnomalys[Random.Range(0, eventAnomalys.Length)];
        currentEventPlace = currentEventAnomaly.eventPlace;
        currentEventType = currentEventAnomaly.Execute();
    }

    private void ProcessEventClear(EventType Type)
    {
        if (Type == currentEventType)
        {
            //클리어
            isAnomaly = false;
            startAnomalyTime = 0;
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

    // 유니티 버튼 OnClick()에 연결할 함수들
    public void ClearCCTVSystemCheck() { ProcessEventClear(EventType.CCTV_SystemCheck); }
    public void ClearCCTVResonance() { ProcessEventClear(EventType.CCTV_Resonance); }
    public void ClearCCTVIncinerate() { ProcessEventClear(EventType.CCTV_Incinerate); }
    public void ClearCCTVElectricity() { ProcessEventClear(EventType.CCTV_Electricity); }
    public void ClearMission() { ProcessEventClear(EventType.Mission); }
}