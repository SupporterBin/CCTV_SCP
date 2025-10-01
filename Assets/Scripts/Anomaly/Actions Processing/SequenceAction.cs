using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// 시퀀스(Sequence: A 끝나고 B, 그 후 C…)

[CreateAssetMenu(menuName = "Anomalies/Actions Processing (First)/Sequence")]
public class SequenceAction : BaseAnomalyActionSO
{
    public List<BaseAnomalyActionSO> steps = new();

    public override bool Validate(out string error)
    {
        foreach (var s in steps)
        {
            if (s == null) { error = "Null step in Sequence"; return false; }
            if (!s.Validate(out error)) return false;
        }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        foreach (var s in steps)
        {
            yield return s.Execute(ctx);
        }
    }
}
