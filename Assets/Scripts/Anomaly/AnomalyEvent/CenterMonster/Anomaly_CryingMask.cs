using System.Security.Principal;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/CenterRoom/Anomaly_CryingMask")]
public class Anomaly_CryingMask : BasicEventAnomaly
{
    [Header("스크립트 들어가있는 가면 프리팹")]
    public GameObject sadMaskPreafab;
    [Header("소환 범위 지정으로 값으로")]
    [Header("[0]번째 first, last는 같은것으로 [1]번째 처럼 늘어날때 또다른 범위 지대가 생겨나는 방식.")]
    public Vector3[] firstTransRange;
    public Vector3[] lastTransRange;

    private GameObject[] spawnObj;

    public override void Clear()
    {
        //아직 순서 [0]몬스터 위치 안정해져서 정해지면 수정.
    }

    public override EventType Execute()
    {
        spawnObj = new GameObject[10];
        int num = Random.Range(0, firstTransRange.Length);

        for (int i = 0; i < StabilityManager.Instance.dayGetMaskValue[DaySystem.Instance.GetNowDay()-1]; i++) {
            spawnObj[i] = Instantiate(sadMaskPreafab, new Vector3(
                Random.Range(firstTransRange[num].x, lastTransRange[num].x),
                Random.Range(firstTransRange[num].y, lastTransRange[num].y),
                Random.Range(firstTransRange[num].z, lastTransRange[num].z)), new Quaternion(0, Random.Range(0, 360), 0, 0));
        }
        return eventType;
    }

    public override void Fail()
    {
        for (int i = 0; i < spawnObj.Length; i++)
        {
            if (spawnObj[i] != null)
            {
                Destroy(spawnObj[i]);
            }
        }

        Debug.Log("실패마스크미션");
    }
}
