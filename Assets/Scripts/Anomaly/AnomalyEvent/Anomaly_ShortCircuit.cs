using UnityEngine;


[CreateAssetMenu(menuName = "Anomalies/Event/Anomaly_ShortCircuit")]
public class Anomaly_ShortCircuit : BasicEventAnomaly
{
    public override void Clear()
    {
        Debug.Log("나 작동하는데");
        //throw new System.NotImplementedException();
    }

    public override EventType Execute()
    {
        return eventType;
        throw new System.NotImplementedException();
    }
}
