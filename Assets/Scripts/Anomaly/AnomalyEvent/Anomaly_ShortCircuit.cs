using UnityEngine;


[CreateAssetMenu(menuName = "Anomalies/Event/Anomaly_ShortCircuit")]
public class Anomaly_ShortCircuit : BasicEventAnomaly
{
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
