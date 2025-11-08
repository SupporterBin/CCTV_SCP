using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    [HideInInspector]
    public static DaySystem Instance;

    //날짜
    private int maxDay;
    private int nowDay = 1;

    //시간
    private int totalMinute;
    private float minuteUpCountingTime;

    //해당 날 클리어?
    private bool isDayClear;

    [Header("임시 시간 관련 텍스트 UI")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI clockText;

    //몇 초 지나면 분이 오름
    [Header("몇 초 지나면 분이 오름? (초)"), SerializeField]
    private float MINUTEUP_TIME = 0.1f;
    [Header("몇 시부터 시작할거임? (분으로 계산)"), SerializeField]
    private int TIME_START = 6;

    private void Awake()
    {
        // 3. instance가 비어있는지 (최초 실행인지) 확인합니다.
        if (Instance == null)
        {
            // 4. instance가 비어있다면, 자기 자신을 할당합니다.
            Instance = this;
            
            // 5. 씬이 전환되어도 이 게임 오브젝트를 파괴하지 않도록 설정합니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 6. 만약 instance에 이미 다른 DaySystem이 할당되어 있다면 (씬 재시작 등)
            //    새로 생긴 이 오브젝트는 파괴합니다. (중복 방지)
            Destroy(gameObject);
        }

        //시간 초기화 (씬 초기화할 때 마다)
        TimeReset();
    }

    public void Update()
    {
        //만약 일정 시간(24시간)을 지나면. (Day 클리어)
        if (totalMinute >= 1440 && !isDayClear) {
            isDayClear = true;
            Debug.Log("게임 클리어");
        }
        else
            TimeUpdate();

        //임시 시간 텍스트 업데이트
        dayText.text = GetDayText();
        clockText.text = GetClockText();
    }

    private void TimeUpdate()
    {
        if(!GameManager.Instance.AllStopCheck())
        {//
            if (minuteUpCountingTime < MINUTEUP_TIME) { minuteUpCountingTime += Time.deltaTime; }
            else
            {
                totalMinute += 4;
                minuteUpCountingTime = 0;
            }
        }
    }

    private void NextDay()
    {
        if (nowDay >= maxDay) /*마지막 날 버팀 이벤트 발생*/ return;
        else nowDay += 1; return; //마지막 날 아님 Day +1
    }

    public string GetClockText()
    {
        int hour = totalMinute / 60;
        int minute = totalMinute % 60;

        if (minute / 10 == 0) { return hour + ":0" + minute; }
        else return hour + ":" + minute;
    }

    public string GetDayText() { return "Day " + nowDay; }

    public int GetNowDay() { return nowDay; }

    /// <summary>
    /// int 시간(분) 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetClock()
    {
        return totalMinute;
    }

    /// <summary>
    /// 시간 리셋
    /// </summary>
    public void TimeReset()
    {
        totalMinute = TIME_START;
    }
}
