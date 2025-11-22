using UnityEngine;

public class ProtocolSystem : MonoBehaviour
{
    [SerializeField, Header("게임오버 카운터 다운 시간(초)")]
    private float protocol_CountTime = 10;
    public bool protocol_Activated = false; // 프로토콜을 작동 시킨적이 있나요?

    //한번 실행했는지 검사하는 거.
    private bool isWarringStartEventCheck = false;
    private bool isDeadEventCheck = false;

    [SerializeField, Header("인식 박스")]
    private BoxCollider checkCollider;

    [SerializeField, Header("키패드")]
    private KeyPad protocolKeyPad; 

    // Update is called once per frame
    void Update()
    {
        //여기서 게임오버 처리해야할 듯
        if (!protocol_Activated) return;

        // 위험 상태 확인 1번만 작동하는거 다 넣어
        if (!isWarringStartEventCheck)
        {
            // 1, 2. 문열기, 여기에 워링 이펙트 활성화 (불빛 난다거나 하는거)
            GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Open");
            GameManager.Instance.anomalySystem.specialObjects[1].GetComponent<Animator>().Play("On");
            GameManager.Instance.isGameStop = true;
            GameManager.Instance.isDeadWarring = true;
            isWarringStartEventCheck = true;
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
    /// 플레이어가 다시 자기 방으로 돌아왔을 때 실행시키는 코드
    /// </summary>
    public void Protocol_ComeBack()
    {
        protocol_Activated = true;
        GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Close");
        GameManager.Instance.isGameStop = false;
    }

    /// <summary>
    /// 타임오버해서 게임 종료일 경우
    /// </summary>
    private void Protocal_GameOver()
    {
        Debug.Log("게임 오버");
        isDeadEventCheck = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!protocolKeyPad.GetSucess()) return;

        if (other.gameObject.tag != "Player") return;

        Protocol_ComeBack();
    }
}
