using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/Anomaly_Restraint")]
public class Anomaly_Restraint : BasicEventAnomaly
{
    public GameObject chairPrefab;
    public GameObject monsterPrefab;

    private GameObject[] spawnObj;

    [Header("서있는 몬스터 모습 생성 위치 및 회전값")]
    public Vector3 monster_Transform;
    public Quaternion monster_Quaternion;

    public override EventType Execute()
    {
        spawnObj = new GameObject[2];

        GameObject OrginMonster = GameManager.Instance.anomalySystem.monsters[2];

        // 기존 몬스터의 보이는 부분을 비활성화
        OrginMonster.transform.GetChild(0).gameObject.SetActive(false);

        // --- 몬스터 생성 (로컬 좌표 적용) ---
        spawnObj[0] = Instantiate(monsterPrefab, OrginMonster.transform);
        spawnObj[0].transform.localPosition = monster_Transform;
        spawnObj[0].transform.localRotation = monster_Quaternion;

        spawnObj[1] = Instantiate(chairPrefab, OrginMonster.transform);

        return eventType;
    }

    private void Cleanup()
    {
        if (spawnObj != null)
        {
            Destroy(spawnObj[0]);
            Destroy(spawnObj[1]);
        }
        GameManager.Instance.anomalySystem.monsters[2].transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void Clear()
    {
        Cleanup();
        Debug.Log("끝, 성공");
    }

    public override void Fail()
    {
        Cleanup();
        Debug.Log("끝, 실패");
    }
}
