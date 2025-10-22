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
        throw new System.NotImplementedException();
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    public override void Fail()
    {
        throw new System.NotImplementedException();
    }
}
