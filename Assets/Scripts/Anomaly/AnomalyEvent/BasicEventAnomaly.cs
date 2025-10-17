using System.Collections;
using System.Linq;
using UnityEngine;

public enum EventPlace
{
    None,
    All,
    LeftRoom,
    CenterRoom,
    RightRoom
}

public enum EventType
{
    None,
    CCTV1,
    CCTV2,
    CCTV3,
    CCTV4,
    CCTV5,
    Mission
}

public abstract class BasicEventAnomaly : ScriptableObject
{
    //그 이벤트의 이름이 뭐임? [임시, 삭제될 수 있는 변수]
    public string eventName;
    // 현재 어떤 공간에서 일어나는 거임?
    public EventPlace eventPlace;
    // 어떤 방식으로 문제를 해결할 거임?
    public EventType eventType;
    // 실행시킬 이벤트 모음
    public BaseAnomalyActionSO[] baseAnomalyActionSO;


    //어떻게 실행시킬건지, 시간으로 특정 이벤트를 몇초 뒤 실행시키거나 할 때 사용.
    //그러니까 이벤트 모음집으로 원하는 이벤트를 생성 및 타이밍 조절.
    public abstract EventType Execute();

    public abstract void Clear();
}
