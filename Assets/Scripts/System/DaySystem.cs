using TMPro;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    private int maxDay;
    private int nowDay;

    private int hour;
    private int minute;
    private float minuteUpCountingTime;

    [Header("임시 시간 관련 텍스트 UI")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI clockText;

    //몇 초 지나면 분이 오름
    private float MINUTEUP_TIME = 0.1f;

    public void Update()
    {
        TimeUpdate();

        //임시 시간 텍스트 업데이트
        dayText.text = GetDay();
        clockText.text = GetClock();
    }

    private void TimeUpdate()
    {
        if(!GameManager.Instance.AllStopCheck())
        {
            if (minuteUpCountingTime < MINUTEUP_TIME) { minuteUpCountingTime += Time.deltaTime; }
            else
            {
                minute += 1;
                minuteUpCountingTime = 0;
            }
        }

        if(minute >= 60)
        {
            hour += minute / 60;
            minute = minute % 60;
        }
    }

    private void NextDay()
    {
        if (nowDay >= maxDay) /*마지막 날 버팀 이벤트 발생*/ return;
        else nowDay += 1; return; //마지막 날 아님 Day +1
    }

    public string GetClock() { return hour + ":" + minute; }
    public string GetDay() { return "Day " + nowDay; }

}
