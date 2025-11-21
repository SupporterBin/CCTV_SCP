using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/RightRoom/Anomaly_Restraint")]
public class Anomaly_Restraint : BasicEventAnomaly
{
    public GameObject chairPrefab;
    public GameObject monsterPrefab;

    private GameObject[] spawnObj;

    [Header("서있는 몬스터 모습 생성 위치 및 회전값")]
    public Vector3 monster_Transform;
    public Quaternion monster_Quaternion;
    public Vector3 monster_Scale;

    private GameObject orginMonster;

    public override EventType Execute()
    {
        spawnObj = new GameObject[2];

        // 기존 몬스터의 보이는 부분을 비활성화
        Transform root = GameManager.Instance.anomalySystem.monsters[2].transform;
        
        for(int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.CompareTag("Target"))
            {
                orginMonster = child.gameObject;
                break;
            }
        }
        //foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        //{
        //    Debug.Log("CHILD TEST" +child.name);
        //}

        orginMonster.SetActive(false);

        // --- 몬스터 생성 (로컬 좌표 적용) ---
        spawnObj[0] = Instantiate(monsterPrefab, GameManager.Instance.anomalySystem.monsters[2].transform);
        spawnObj[0].transform.localPosition = monster_Transform;
        spawnObj[0].transform.localRotation = monster_Quaternion;
        spawnObj[0].transform.localScale = monster_Scale;

        spawnObj[1] = Instantiate(chairPrefab, GameManager.Instance.anomalySystem.monsters[2].transform);
        spawnObj[1].transform.localScale = monster_Scale;

        return eventType;
    }

    private void Cleanup()
    {
        if (spawnObj != null)
        {
            Destroy(spawnObj[0]);
            Destroy(spawnObj[1]);
        }

        orginMonster.SetActive(true);
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
