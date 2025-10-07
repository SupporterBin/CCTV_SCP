using UnityEngine;

public class DaySystem : MonoBehaviour
{
    private int maxDay;
    private int nowDay;

    private int hour;
    private int minute;
    private float minuteUpCountingTime;

    //몇 초 지나면 분이 오름
    private float MINUTEUP_TIME = 5;

    public void Update()
    {
        TimeUpdate();
    }

    private void TimeUpdate()
    {
        if(GameManager.Instance.AllStopCheck())
        {
            if (minuteUpCountingTime < MINUTEUP_TIME) { minuteUpCountingTime += Time.deltaTime; }
            else minute += 1;
        }

        if(minute <= 60)
        {
            hour += minute / 60;
            minute = minute % 60;
        }
    }

    private void NextDay()
    {
        if (nowDay >= maxDay) return;
    }

    public int GetMinute() { return minute; }
    public int GetHour() { return hour; }
    public int GetDay() { return nowDay; }

}
