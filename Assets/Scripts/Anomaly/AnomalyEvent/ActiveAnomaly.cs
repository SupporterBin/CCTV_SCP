using UnityEngine;

[System.Serializable]
public class ActiveAnomaly
{
    public BasicEventAnomaly eventScript; // 실행된 SO 원본
    public EventPlace place;              // 발생 장소
    public EventType type;                // 이벤트 타입 (정답 체크용)
    public float currentTimer;            // 현재 경과 시간
    public int mapValue;                  // 안정도 감소 계산용 값

    // 생성자
    public ActiveAnomaly(BasicEventAnomaly script, EventPlace p, EventType t)
    {
        eventScript = script;
        place = p;
        type = t;
        currentTimer = 0f;

        // mapValue 미리 계산 (매 프레임 switch문 돌리지 않도록 최적화)
        switch (place)
        {
            case EventPlace.LeftRoom: mapValue = 0; break;
            case EventPlace.CenterRoom: mapValue = 1; break;
            case EventPlace.RightRoom: mapValue = 2; break;
            case EventPlace.All: mapValue = 10; break;
            default: mapValue = 999; break;
        }
    }
}