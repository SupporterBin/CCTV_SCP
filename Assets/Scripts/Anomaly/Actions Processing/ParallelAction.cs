using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 병렬(Parallel: A / B / C 동시에 시작, 모두 끝나면 완료)

[CreateAssetMenu(menuName = "Anomalies/Actions Processing (First)/Parallel")]
public class ParallelAction : BaseAnomalyActionSO
{
    public List<BaseAnomalyActionSO> branches = new();

    public override bool Validate(out string error)
    {
        foreach (var b in branches)
        {
            if (b == null) { error = "Null branch in Parallel"; return false; }
            if (!b.Validate(out error)) return false;
        }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        var coroutines = new List<IEnumerator>(branches.Count);
        foreach (var b in branches) coroutines.Add(b.Execute(ctx));

        // 간단 병렬 실행 (러너를 안 쓰고 여기서 직접 관리하는 최소형)
        var runners = new List<Coroutine>(branches.Count);
        var mb = GetOrCreateRunnerHost(); // ScriptableObject라 코루틴 호스트가 필요(유틸)
        foreach (var co in coroutines) runners.Add(mb.StartCoroutine(co));

        // 모두 끝났는지 체크(간소화: lifeTime/Wait로 대체 가능)
        // 실제로는 러너에서 관리하는 게 더 깔끔
        // 여기서는 데모 용도로 WaitForEndOfFrame N프레임 정도 대기
        yield return new WaitForEndOfFrame();
    }

    private MonoBehaviour GetOrCreateRunnerHost()
    {
        // 데모용. 실제 프로젝트는 전용 Runner(MB)가 액션들을 실행함.
        var go = GameObject.Find("__AnomalyRunnerHost") ?? new GameObject("__AnomalyRunnerHost");
        var host = go.GetComponent<MonoBehaviourHost>();
        if (!host) host = go.AddComponent<MonoBehaviourHost>();
        return host;
    }

    private class MonoBehaviourHost : MonoBehaviour { }
}
