using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/CenterRoom/Anomaly_HumanImitation")]
public class Anomaly_HumanImitation : BasicEventAnomaly
{
    [Header("변신 대상 여성")]
    public GameObject transformationPrefab;

    private GameObject spawnObj;

    [Header("서있는 몬스터 모습 생성 위치 및 회전값")]
    public Vector3 monster_Transform;
    public Quaternion monster_Quaternion;
    public Vector3 monster_Scale;

    private GameObject OrginMonster;
    private AudioSource saveSound;

    public override EventType Execute()
    {
        saveSound = SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalMaskHumanTransform,
            GameManager.Instance.anomalySystem.monsters[1].transform.position, 20, false);

        // 기존 몬스터의 보이는 부분을 비활성화
        Transform root = GameManager.Instance.anomalySystem.monsters[1].transform;

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Target"))
            {
                OrginMonster = child.gameObject;
                break;
            }
        }
        OrginMonster.SetActive(false);

        // --- 몬스터 생성 (로컬 좌표 적용) ---
        spawnObj = Instantiate(transformationPrefab, GameManager.Instance.anomalySystem.monsters[1].transform);
        spawnObj.transform.localPosition = monster_Transform;
        spawnObj.transform.localRotation = monster_Quaternion;
        spawnObj.transform.localScale = monster_Scale;

        return eventType;
    }

    private void CleanUp()
    {
        if (spawnObj != null)
        {
            Destroy(spawnObj);
        }
        OrginMonster.SetActive(true);

        SoundManager.Instance.StopSFX(saveSound);
    }

    public override void Clear()
    {
        CleanUp();
        Debug.Log("끝, 성공");
    }

    public override void Fail()
    {
        CleanUp();
        Debug.Log("끝, 실패");
    }
}
