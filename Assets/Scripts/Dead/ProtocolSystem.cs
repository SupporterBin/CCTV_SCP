using UnityEngine;

public class ProtocolSystem : MonoBehaviour
{
    [SerializeField, Header("게임오버 카운터 다운 시간(초)")]
    private float protocol_CountTime = 10;
    public bool protocol_Activated = false; // 프로토콜을 작동 시킨적이 있나요?
    private bool protocol_Click = false; // 프로토콜 실행 버튼 클릭!

    //버튼 실행 후 딜레이 타임.
    private float delayUpdateTime = 0;
    private float delatCountingTime = 0;

    //한번 실행했는지 검사하는 거.
    private bool isOpenEventCheck = false;
    private bool isDeadEventCheck = false;

    // Update is called once per frame
    void Update()
    {
        if (!protocol_Activated) return;

        // 위험 상태 확인 1번만 작동하는거 다 넣어
        if (!isOpenEventCheck)
        {
            // 1, 2. 문열기, 여기에 워링 이펙트 활성화 (불빛 난다거나 하는거)
            GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Open");
            GameManager.Instance.anomalySystem.specialObjects[1].GetComponent<Animator>().Play("On");
            GameManager.Instance.isDeadWarring = true;
            isOpenEventCheck = true;
        }

        //버튼 눌렀다면 실행될거 여기.
        if (protocol_Click)
        {
            if (delayUpdateTime > 0) return;
            else delatCountingTime += Time.deltaTime;

            for (int i = 0; i < 3; i++)
            {
                //질문, 만약 이미 상태가 0인 상태인 게이지는 어떻게 처리?

                StabilityManager.Instance.StabilizationUp(StabilityManager.Instance.CurrentStability[i] - StabilityManager.Instance.CurrentStability[i] + 15f, i);
            }

            protocol_Click = false;
        }

        //카운터 다운.
        if (GameManager.Instance.isDeadWarring) DeadWarringCount(1);
    }

    /// <summary>
    /// 프로토콜 누를 수 있는 시간제한 카운터 다운
    /// </summary>
    /// <param name="roomNum"></param>
    private void DeadWarringCount(int roomNum)
    {
        protocol_CountTime -= Time.deltaTime;
        if (protocol_CountTime > 0) return;

        if (isDeadEventCheck) return;
        Protocal_GameOver();
        //게임 오버 만들거면 여기에 만들어줘
        //혹시 쓸 수 있으니까 매개 변수 만들어두긴 함, 안쓰면 버려.
    }

    /// <summary>
    /// 버튼 누르는데 성공. (버튼)
    /// </summary>
    public void Protocal_StartButton()
    {
        protocol_Activated = true;

        for (int i = 0; i < 3; i++)
        {
            StabilityManager.Instance.StabilizationUp(1, i);
        }

        GameManager.Instance.isDeadWarring = false;
        protocol_Click = true;
        delayUpdateTime = Random.Range(1, 3);
    }

    private void Protocal_GameOver()
    {
        Debug.Log("게임 오버");
        isDeadEventCheck = true;
    }
}
