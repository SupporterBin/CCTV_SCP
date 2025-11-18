using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Button9 : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI DisplayNum;

    // 버튼 9개 
    [SerializeField]
    private Button[] buttons;

    // 패널 관리
    [SerializeField]
    private GameObject button9Panel;

    // 보여줄 버튼 리스트, 누른 버튼 리스트
    private List<int> buttonSequence = new List<int>();
    private List<int> inputSequence = new List<int>();

    // 버튼에 색깔 부여하려고 만든 거
    private Color defaultColor = Color.white;
    private Color lightColor = Color.cyan;

    // 버튼 알려주는 시간
    private float lightDuration = 0.5f;

    // 버튼을 알려주고 있는가? 음 이건 순서대로 버튼 나올 때 입력 못 받게 하는? 그런느낌으로 씀
    private bool isStagePlaying = false;

    [SerializeField]
    TabletSceneManager tabletSceneManager;
    private void Awake()
    {
       // 각각의 버튼 배열에 OnClick 기능을 넣어줌
        for(int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            if (buttons[index] != null)
            {
                buttons[index].onClick.AddListener(() => OnClickButton(index));
            }
        }
    }
    // 시작시 패널 꺼
    private void Start()
    {
        button9Panel.SetActive(false);
    }

    // 태블릿에서 스타트 버튼을 눌렀을 때 실행되는 함수이며, 패널 켜기,
    // 스테이지 초기화, 버튼 순서 초기화, 스테이지 시작이 있음.
    public void StartButton9()
    {
        if (tabletSceneManager.isPlaying)
        {
            button9Panel.SetActive(true);
            buttonSequence.Clear();
            StartCoroutine(StageStart());
        }
    }
    
    // 스테이지 시작 함수
    private IEnumerator StageStart()
    {
        // 스테이지 시작 변수 활성화 [버튼 켜질 때 입력 막기위함]
        isStagePlaying = true;
        // 입력 및 순서 초기화
        buttonSequence.Clear();
        inputSequence.Clear();

        // 버튼 중복 켜짐을 방지하기위한 예비용 리스트 추가 및 리스트에 버튼 수만큼 배열에 넣기
        List<int> preparatoryList = new List<int>();
        for(int i = 0; i < buttons.Length; i++)
        {
            preparatoryList.Add(i);
        }

        // 스테이지 별 활성화 버튼 수 및 순서 정하기
        for(int i = 0; i < 4 + DaySystem.Instance.GetNowDay(); i++)
        {
            // 예비용 리스트에 2개 이상 있을 때 이걸 왜쓰냐 순서를 Random.Range로 뽑을건데
            // Random.Range는 (최소, 최대)이고 (0, 1) 이라했을때 최대의 -1 까지 뽑는거라 0밖에 안뽑임 확정
            if(preparatoryList.Count > 1)
            {
                // 랜덤으로 뽑은 숫자
                int random = Random.Range(0, preparatoryList.Count);
                // 뽑은 숫자 넣기위한 변수 1개 더
                int randomNumber = preparatoryList[random];
                // 버튼 순서 리스트에 추가
                buttonSequence.Add(randomNumber);
                // 뽑은 숫자 예비 리스트에서 제거
                preparatoryList.RemoveAt(random);
            }
        }
        DisplayInputNumber();
        yield return new WaitForSeconds(1f);

        // 랜덤으로 뽑은 리스트 다봐버리기
        foreach(int index in buttonSequence)
        {
            // 버튼의 색깔 바꾸기위한 image 컴포넌트 불러오기
            Image btColor = buttons[index].GetComponent<Image>();

            yield return new WaitForSeconds(lightDuration);
            btColor.color = lightColor;
            yield return new WaitForSeconds(lightDuration);
            btColor.color = defaultColor;
        }
        isStagePlaying = false;
        Debug.Log($"플레이 중 : {isStagePlaying}");

    }

    private void OnClickButton(int buttonindex)
    {
        // 스테이지 진행 중일 때 [불 반짝일 때] 안함
        if (isStagePlaying)
        {
            return;
        }

        // 위에 각 버튼에 OnClick 기능 부여했는데 눌렀을 때 누른 버튼의 번호를 리스트에 추가 
        inputSequence.Add(buttonindex);

        DisplayInputNumber();
        // 내가 누른 버튼의 수와 버튼의 리스트와 비교하여 틀릴 경우 바로 캇
        // 예를 들어 버튼의 리스트가 { 0 , 1 , 2 }라 했을 때 
        // 아직 inputSequence의 리스트에는 {} 비어있음 만약 1번째 칸을 누르면 0이 들어감
        // 그러면 inputSequence의 리스트는 { 0 }이 되고 이 if문 식으로 가면
        // count는 1이 되니까
        // inputSequence[0]의 값은 0 buttonSequence[0]의 값도 0 이기에 성공임 틀리면 나가리
        if (inputSequence[inputSequence.Count - 1] != buttonSequence[inputSequence.Count - 1])
        {
            Debug.Log("실패");
            EndGame();
            return;
        }
        // 그렇게 쭉 성공하고 두 배열의 갯수가 맞으면 똑같이 쳤다는 거기에 성공
        if (inputSequence.Count == buttonSequence.Count)
        {
            if (inputSequence.Count == 9)
            {
                Debug.Log("5일차");
                EndGame();
            }
            else
            {
                Debug.Log($"{DaySystem.Instance.GetNowDay()} 일차 클리어");
                StabilityManager.Instance.StabilizationUp(10, 0);
                EndGame();
            }
        }
        Debug.Log($"버튼 {buttonindex + 1} 눌림");
    }

    private void DisplayInputNumber()
    {
        if(DisplayNum == null)
        {
            return;
        }

        string resultText = "";

        foreach (int i in inputSequence)
        {
            resultText += (i + 1).ToString() + " ";
        }

        int remainText = buttonSequence.Count - inputSequence.Count;
        for (int i = 0; i < remainText; i++)
        {
            resultText += "_ ";
        }

        DisplayNum.text = resultText;
    }

    // 아직 대충 함
    private void EndGame()
    {
        button9Panel.SetActive(false);
        tabletSceneManager.tabletPanels[0].SetActive(true);
        tabletSceneManager.isPlaying = false;

        if(DisplayNum != null)
        {
            DisplayNum.text = " ";
        }
    }
}
