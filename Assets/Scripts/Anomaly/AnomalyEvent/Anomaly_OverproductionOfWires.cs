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
        //소환
        if (wiresPrefab == null)
            Debug.Log("이상현상_전선_SO 배치 안됨.");
        else
            spawnObject = Instantiate(wiresPrefab, spawnObjVector, SpawnObjQuaternion);

        return eventType;
    }

    public override void Clear()
    {
        Debug.Log("이상현 클리어 처리 완료");

        if (spawnObject == null)
            Debug.Log("이상현상_전선_SO_오브제 못 가져옴.");
        else
            Destroy(spawnObject);

        spawnObject = null;
    }

    public override void Fail()
    {
        Debug.Log("이상현 실패 처리 완료");

        if (spawnObject == null)
            Debug.Log("이상현상_전선_SO_오브제 못 가져옴.");
        else
            Destroy(spawnObject);

        spawnObject = null;
    }
}
