using UnityEngine;

public class DaySystem : MonoBehaviour
{
    private int maxDay;
    private int nowday;

    private int hour;
    private int minute;

    public void Update()
    {
        TimeUpdate();
    }

    private void TimeUpdate()
    {
        if(minute <= 60)
        {
            hour += minute / 60;
            minute = minute % 60;
        }
    }

    private void NextDay()
    {
        if (nowday >= maxDay) return;
    }
}
