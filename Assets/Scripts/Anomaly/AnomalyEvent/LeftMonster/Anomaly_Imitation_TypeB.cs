using UnityEngine;

public class Anomaly_Imitation_TypeB : BasicEventAnomaly
{
    public GameObject transformationPrefab;

    private GameObject spawnObj;

    [Header("서있는 몬스터 모습 생성 위치 및 회전값")]
    public Vector3 monster_Transform;
    public Quaternion monster_Quaternion;

    public override EventType Execute()
    {
        //이거 지금 2번으로 되어있는데 나중에 그에 해당 하는 몬스터로 바꾸기
        GameObject OrginMonster = GameManager.Instance.anomalySystem.monsters[2];

        // 기존 몬스터의 보이는 부분을 비활성화
        OrginMonster.transform.GetChild(0).gameObject.SetActive(false);

        // --- 몬스터 생성 (로컬 좌표 적용) ---
        spawnObj = Instantiate(transformationPrefab, OrginMonster.transform);
        spawnObj.transform.localPosition = monster_Transform;
        spawnObj.transform.localRotation = monster_Quaternion;

        return eventType;
    }

    private void Cleanup()
    {
        if (spawnObj != null)
        {
            Destroy(spawnObj);
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
