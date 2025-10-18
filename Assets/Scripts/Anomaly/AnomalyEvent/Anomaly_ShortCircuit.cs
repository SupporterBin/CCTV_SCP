using UnityEngine;


[CreateAssetMenu(menuName = "Anomalies/Event/Anomaly_ShortCircuit")]
public class Anomaly_ShortCircuit : BasicEventAnomaly
{
    [Header("스파클 프리팹 넣기")]
    public GameObject sparklePreafab;
    [Header("스파클 위치, 회전 값 넣기")]
    public Vector3 spawnObjVector;
    public Quaternion SpawnObjQuaternion;
    //크기 혹은 여러개가 된다면 그에 맞게 표현 ㄱ

    private GameObject spawnObject; 

    public override void Execute()
    {
        //소환
        spawnObject = Instantiate(spawnObject, spawnObjVector, SpawnObjQuaternion);

        throw new System.NotImplementedException();
    }

    public override void Clear()
    {
        Destroy(spawnObject);
        spawnObject = null;
        throw new System.NotImplementedException();
    }
}
