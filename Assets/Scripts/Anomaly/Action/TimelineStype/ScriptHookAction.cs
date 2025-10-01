using System.Collections;
using UnityEngine;

// ✦ 중간에 맞았냐/조건 충족이냐 등 분기/계산이 필요한 로직을 호출
[CreateAssetMenu(menuName = "Anomalies/Actions/Special/Script Hook")]
public class ScriptHookAction : BaseAnomalyActionSO
{
    public string hookName; // 예: "ApplyDamage", "OpenDoor", "BroadcastWarning"

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        // 실행자(서비스)에서 훅 호출
        // 예: AnomalyServices.Invoke(hookName, ctx);
        Log($"Invoke Hook: {hookName}");
        yield break;
    }
}
