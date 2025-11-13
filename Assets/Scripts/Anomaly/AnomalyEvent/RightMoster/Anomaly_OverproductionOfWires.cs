using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/RightRoom/Anomaly_OverproductionOfWires")]
public class Anomaly_OverproductionOfWires : BasicEventAnomaly
{    
    public override EventType Execute()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleGrow();

        return eventType;
    }

    public override void Clear()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleBurn();
    }

    public override void Fail()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleFadeAway();
    }
}
