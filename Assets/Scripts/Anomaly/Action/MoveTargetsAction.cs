using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Actions/Move Targets")]
public class MoveTargetsAction : BaseAnomalyActionSO
{
    public Vector3 localOffset = new Vector3(0, 0, 2);
    public float duration = 0.5f;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    public override bool Validate(out string error)
    {
        if (duration <= 0f) { error = "duration must be > 0"; return false; }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        if (ctx.Targets == null || ctx.Targets.Count == 0) yield break;

        // 모든 타깃을 동일한 로컬 오프셋만큼 부드럽게 이동 (병렬)
        float t = 0f;
        var start = new Vector3[ctx.Targets.Count];
        var end = new Vector3[ctx.Targets.Count];

        for (int i = 0; i < ctx.Targets.Count; i++)
        {
            var tr = ctx.Targets[i].transform;
            start[i] = tr.position;
            end[i] = tr.position + localOffset;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = curve.Evaluate(Mathf.Clamp01(t / duration));
            for (int i = 0; i < ctx.Targets.Count; i++)
            {
                var tr = ctx.Targets[i].transform;
                tr.position = Vector3.LerpUnclamped(start[i], end[i], k);
            }
            yield return null;
        }
    }
}
