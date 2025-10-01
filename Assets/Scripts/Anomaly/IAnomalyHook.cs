using System.Collections;

public interface IAnomalyHook
{
    // 이 훅이 처리하는 이름(키). 예: "ApplyDamage", "OpenDoor"
    string Name { get; }

    // 실제 동작. AnomalyContext와 임의의 payload(옵션)를 받아 코루틴으로 수행
    IEnumerator Handle(AnomalyContext ctx, object payload);
}