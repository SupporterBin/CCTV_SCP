using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/Special/Anomaly_Illusion")]
public class Anomaly_Illusion : BasicEventAnomaly
{
    public override void Clear()
    {
        if(ShaderEffect_CorruptedVram.Instance != null)
            ShaderEffect_CorruptedVram.Instance.shift = 0;
    }

    public override EventType Execute()
    {
        if(ShaderEffect_CorruptedVram.Instance != null)
            ShaderEffect_CorruptedVram.Instance.shift = 1;
        return eventType;
    }

    public override void Fail()
    {
        if(ShaderEffect_CorruptedVram.Instance != null)
            ShaderEffect_CorruptedVram.Instance.shift = 0;
    }
}
