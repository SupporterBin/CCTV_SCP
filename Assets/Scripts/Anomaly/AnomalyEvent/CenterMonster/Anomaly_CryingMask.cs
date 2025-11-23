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
    }

    public override EventType Execute()
    {
        spawnObj = new GameObject[10];
        int num = Random.Range(0, firstTransRange.Length);

        // [안전장치] 날짜 인덱스 계산
        int dayIndex = DaySystem.Instance.GetNowDay() - 1;

        // 날짜가 배열 범위를 벗어나는지 체크 (에러 방지)
        if (dayIndex < 0 || dayIndex >= StabilityManager.Instance.dayGetMaskValue.Length)
        {
            Debug.LogError("Day Index 오류! dayGetMaskValue 배열 범위를 확인하세요.");
            return eventType; // 생성 안 하고 종료
        }

        int spawnCount = (int)StabilityManager.Instance.dayGetMaskValue[dayIndex];

        for (int i = 0; i < spawnCount; i++)
        {
            // 배열 인덱스 초과 방지 (spawnObj는 10개인데 spawnCount가 11 이상이면 에러남)
            if (i >= spawnObj.Length) break;

            spawnObj[i] = Instantiate(sadMaskPreafab, new Vector3(
                Random.Range(firstTransRange[num].x, lastTransRange[num].x),
                Random.Range(firstTransRange[num].y, lastTransRange[num].y),
                Random.Range(firstTransRange[num].z, lastTransRange[num].z)),
                Quaternion.Euler(0, Random.Range(0, 360), 0)); // Quaternion.Euler 사용 권장
        }
        return eventType;
    }

    public override void Fail()
    {
        // ★ 안전장치 추가: spawnObj가 존재할 때만 실행
        if (spawnObj != null)
        {
            for (int i = 0; i < spawnObj.Length; i++)
            {
                if (spawnObj[i] != null)
                {
                    Destroy(spawnObj[i]);
                }
            }
        }

        // 배열 자체를 비워주는 것이 안전함
        spawnObj = null;

        Debug.Log("실패마스크미션");
    }
}
