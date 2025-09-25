using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AJae : MonoBehaviour
{
    // 패널 온오프
    [SerializeField]
    private GameObject aJaePanel;

    // 게임 시작시 글자 띄어주려고
    [SerializeField]
    private TextMeshProUGUI[] texts;

    // 타이머 보여주려고
    [SerializeField]
    private TextMeshProUGUI timerText;

    // 입력에 사용하는 키 및 리스트에 넣을 키
    private string[] usingKey = { "q", "w", "e", "a", "s", "d" };

    // 현 스테이지[일차]
    private int curStage = 1;
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


    TabletSceneManager tabletSceneManager;
    private void Start()
    {
        aJaePanel.SetActive(false);
    }

    private void Update()
    {
        // 시간 관리 및 텍스트로 내보내기
        if(isAjaePlaying)
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
        // 패널 온, 리스트 정리, 함수 시작
        aJaePanel.SetActive(true);
        aJaeSequence.Clear();
        inputSequence.Clear();
        StartCoroutine(StartStage());
    }

    private IEnumerator StartStage()
    {
        // 플레이 중, 리스트 정리, 1초 기다리기
        aJaeSequence.Clear();
        inputSequence.Clear();

        // 스테이지 마다 갯수 증가 및 중복가능한 랜덤 뽑아 리스트에 넣기

        for(int i = 0; i < stageLen + (2 * (curStage - 1)); i++)
        {
            int index = Random.Range(0, usingKey.Length);
            aJaeSequence.Add(usingKey[index]);
        }

        // 보여줄 리스트 크기 만큼 실행하며, 스테이지에 쓰이는 갯수보다 적어지면 캇 나머지는 안보임
        for (int i = 0; i < texts.Length; i++)
        {
            if(i < aJaeSequence.Count)
            {
                texts[i].text = aJaeSequence[i];
            }
            else
            {
                texts[i].text = "";
            }
        }

        // n초 기다렸다, 타이머 설정,  플레이 중
        yield return new WaitForSeconds(1f);

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

        // 나온거랑 이상하게 치면
        if (inputSequence[inputSequence.Count - 1] != aJaeSequence[inputSequence.Count - 1])
        {
            // 여기서 끝내는 함수미완
            isAjaePlaying = false;
            EndGame();
        }

        if (inputSequence.Count == aJaeSequence.Count)
        {
            // 성공 함수여기 부분 고쳐야됨 테스트용으로 계속 나오게 만듬.
            curStage++;
            StartCoroutine(StartStage());
        }
    }

    // 아직 구체적인 기능 미완
    private void EndGame()
    {
        isAjaePlaying = false;
        aJaePanel.SetActive(false);
        tabletSceneManager.tabletPanels[2].SetActive(true);
    }
}
