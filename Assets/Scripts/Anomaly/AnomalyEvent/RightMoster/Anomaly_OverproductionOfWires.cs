using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/RightRoom/Anomaly_OverproductionOfWires")]
public class Anomaly_OverproductionOfWires : BasicEventAnomaly
{
    private AudioSource saveSound;

    public override EventType Execute()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleGrow();
        saveSound = SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalWireOvergrowthRubber,
            GameManager.Instance.anomalySystem.monsters[2].transform.position, 20, false);
        return eventType;
    }

    public override void Clear()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleBurn();

        SoundManager.Instance.StopSFX(saveSound);
    }

    public override void Fail()
    {
        TentacleAnomalyController inst = TentacleAnomalyController.Instance;
        if (inst == null) throw new NullReferenceException();
        inst.TentacleFadeAway();

        SoundManager.Instance.StopSFX(saveSound);
    }
}
