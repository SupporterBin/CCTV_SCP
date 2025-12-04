using UnityEngine;


[CreateAssetMenu(menuName = "Anomalies/Event/RightRoom/Anomaly_ShortCircuit")]
public class Anomaly_ShortCircuit : BasicEventAnomaly
{
    [Header("스파클 프리팹 넣기")]
    public GameObject sparklePrefab;
    [Header("스파클 위치, 회전 값 넣기")]
    public Vector3 spawnObjVector;
    public Quaternion SpawnObjQuaternion;
    //크기 혹은 여러개가 된다면 그에 맞게 표현 ㄱ

    private AudioSource saveSound;

    private GameObject spawnObject; 

    public override EventType Execute()
    {
        //소환
        if (sparklePrefab == null)
            Debug.Log("이상현상_스파클_SO 배치 안됨.");
        else
        {
            spawnObject = Instantiate(sparklePrefab, spawnObjVector, SpawnObjQuaternion);
            saveSound = SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalWireShortCircuitSpark,
            GameManager.Instance.anomalySystem.monsters[2].transform.position, 20, false);
        }
        return eventType;
    }

    public override void Clear()
    {
        Debug.Log("이상현 클리어 처리 완료");

        if (spawnObject == null)
            Debug.Log("이상현상_스파클_SO_오브제 못 가져옴.");
        else
            Destroy(spawnObject);
        
        spawnObject = null;
        SoundManager.Instance.StopSFX(saveSound);
    }

    public override void Fail()
    {
        Debug.Log("이상현 실패 처리 완료");

        if(spawnObject == null)
            Debug.Log("이상현상_스파클_SO_오브제 못 가져옴.");
        else
            Destroy(spawnObject);

        spawnObject = null;
        SoundManager.Instance.StopSFX(saveSound);
    }
}
