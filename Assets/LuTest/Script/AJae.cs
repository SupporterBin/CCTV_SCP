using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;

// 텍스트로 하니까 안 이뻐서 키 이미지 가져와서 하려고 만들었습니다.
// 키랑 쓸 이미지랑 매칭 클래스
[System.Serializable]
public class Link_Image_And_Key
{
    public string key;
    public Sprite keyImage;
}

public class AJae : MonoBehaviour
{
    // 패널 온오프
    [SerializeField]
    private GameObject aJaePanel;

    [SerializeField]
    private Link_Image_And_Key[] Link;

    // 게임 시작시 글자 띄어주려고
    [SerializeField]
    private Image[] ShowImages;

    // 타이머 보여주려고
    [SerializeField]
    private TextMeshProUGUI timerText;
    // 게임 시간
    private float limitTimer = 10;
    // 타이머에 쓸라고
    private float timer = 0;
    // 스테이지마다 갯수 늘어나는데 기초 갯수
    private int stageLen = 6;

    // 랜덤으로 뽑은 리스트와 입력에 필요한 리스트
    List<string> aJaeSequence = new List<string>();
    List<string> inputSequence = new List<string>();

    // 이건 좀 봐야댐 일단 프로토 타입으로 제작
    private bool isAjaePlaying = false;

    [SerializeField]
    TabletSceneManager tabletSceneManager;

    [SerializeField]
    private Button exitButton;

    private int difficultyLevel = 1;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => ClickExitButton());
    }

    private void Start()
    {
        aJaePanel.SetActive(false);
    }
    private int GetDay()
    {
        return DaySystem.Instance.GetNowDay();
    }

    private void Update()
    {
        if(aJaePanel.activeSelf)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                EndGame();
            }
        }

        // 시간 관리 및 텍스트로 내보내기
        if (isAjaePlaying)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F1");

            if(timer <= 0)
            {
                EndGame();
                return;
            }
        }

        // 아재 실행시에만
        if(isAjaePlaying)
        {
            // 입력 감지
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                InputKey("q");
            }
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                InputKey("w");
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                InputKey("e");
            }
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                InputKey("a");
            }
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                InputKey("s");
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                InputKey("d");
            }
        }
    }

    public void StartAJae()
    {
        if(tabletSceneManager.isPlaying)
        {
            // 패널 온, 리스트 정리, 함수 시작
            aJaePanel.SetActive(true);
            aJaeSequence.Clear();
            inputSequence.Clear();
            StartCoroutine(StartStage());
        }
    }

    private IEnumerator StartStage()
    {
        // 플레이 중, 리스트 정리, 1초 기다리기
        aJaeSequence.Clear();
        inputSequence.Clear();
        timerText.text = limitTimer.ToString("F1");

        int day = GetDay();
        if (day == 1) { difficultyLevel = 1; }
        else if (day == 2) { difficultyLevel = 2; }
        else if (day == 3 || day == 4 || day == 5) { difficultyLevel = 3; }

        foreach (var images in ShowImages)
        {
            images.gameObject.SetActive(false);
            images.color = Color.white;
        }

        // 스테이지 마다 갯수 증가 및 중복가능한 랜덤 뽑아 리스트에 넣기

        for(int i = 0; i < stageLen + (2 * (difficultyLevel - 1)); i++)
        {
            int index = Random.Range(0, Link.Length);
            aJaeSequence.Add(Link[index].key);
        }

        // 보여줄 리스트 크기 만큼 실행하며, 스테이지에 쓰이는 갯수보다 적어지면 캇 나머지는 안보임
        for (int i = 0; i < ShowImages.Length; i++)
        {
            if(i < aJaeSequence.Count)
            {
                string currentKey = aJaeSequence[i];
                // 배열 찾고있는 게 맞으면 이미지 가져오고, 아님 말고
                Sprite sprite = Link.FirstOrDefault(x => x.key == currentKey)?.keyImage;

                ShowImages[i].sprite = sprite;
                
            }
            else
            {
                ShowImages[i].gameObject.SetActive(false);
            }
        }

        // n초 기다렸다, 타이머 설정,  플레이 중
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < ShowImages.Length; i++)
        {
            if(i < aJaeSequence.Count)
            {
                ShowImages[i].gameObject.SetActive(true);
            }
        }
        timer = limitTimer;
        isAjaePlaying = true;
    }

    private void InputKey(string inputKey)
    {
        // 이즈플레이 관련으로는 다시 작업할듯
        if(!isAjaePlaying)
        {
            return;
        }
        // 누른 입력값 입력 리스트에 추가
        inputSequence.Add(inputKey);

        int currentKey = inputSequence.Count - 1;

        // 나온거랑 이상하게 치면
        if (inputSequence[inputSequence.Count - 1] != aJaeSequence[inputSequence.Count - 1])
        {
            // 여기서 끝내는 함수미완
            isAjaePlaying = false;
            EndGame();
        }
        else if (inputSequence[currentKey] == aJaeSequence[currentKey])
        {
            ShowImages[currentKey].color = Color.gray;
        }

        if (inputSequence.Count == aJaeSequence.Count)
        {
            StabilityManager.Instance.StabilizationUp(10, 2);
            // 성공 함수여기 부분 고쳐야됨 테스트용으로 계속 나오게 만듬.
            Debug.Log($"{DaySystem.Instance.GetNowDay()} 일차");
            EndGame();
        }
    }

    private void ClickExitButton()
    {
        if (aJaePanel.activeSelf)
        {
            EndGame();
        }
    }


    // 아직 구체적인 기능 미완
    private void EndGame()
    {
        isAjaePlaying = false;
        aJaePanel.SetActive(false);
        tabletSceneManager.tabletPanels[2].SetActive(true);
        tabletSceneManager.isPlaying = false;
    }
}
