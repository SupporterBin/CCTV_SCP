using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/Anomaly_OverproductionOfWires")]
public class Anomaly_OverproductionOfWires : BasicEventAnomaly
{
    [Header("전선 프리팹 넣기")]
    public GameObject wiresPrefab;
    [Header("전선 위치, 회전 값 넣기")]
    public Vector3 spawnObjVector;
    public Quaternion SpawnObjQuaternion;
    //크기 혹은 여러개가 된다면 그에 맞게 표현 ㄱ

    private GameObject spawnObject;

    
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
