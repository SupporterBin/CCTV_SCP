using UnityEngine;
using System.Collections;

//
public interface IAnomalyAction
{
    // 러너가 호출하는 공통 실행 진입점(코루틴 반환)
    // 실제 행동 액션.
    IEnumerator Execute(AnomalyContext ctx); 

    // 사전 검증(필수 참조/값 체크). 실패 시 false.
    // 세팅이 정상인지 확인하는 코드
    bool Validate(out string error);
}

// ✦ 중복되는 잡일(로그/기본 검증/타임아웃 헬퍼 등)을 담는 얇은 추상 부모 (선택적 상속)
public abstract class BaseAnomalyActionSO : ScriptableObject, IAnomalyAction
{
    //오류 검사 부분 (항상 true 이지만, 재정의 해서 추가 검사 가능)
    public virtual bool Validate(out string error) { error = null; return true; }

    // 행동 동작
    public abstract IEnumerator Execute(AnomalyContext ctx);

    // 어떤 동작했는지 확인이 필요할 때 사용.
    protected void Log(string msg) => Debug.Log($"[Action] {name}: {msg}");
}