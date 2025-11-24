using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    [SerializeField]
    private GameObject fishingPanel; // 미니 게임 패널

    [SerializeField]
    private RectTransform gaugeBarTransform; // 게이지 바 위치
    
    [SerializeField]
    private RectTransform successZoneTransform; // 성공 범위 위치

    [SerializeField]
    private RectTransform playerMakerTransform; // 플레이어 마커 위치

    [SerializeField]
    private TextMeshProUGUI Timer; // 타이머 텍스트

    [SerializeField]
    private TextMeshProUGUI Progress; // 성공 범위에 있는 시간 텍스트

    private Rect gaugeBarRect; // width값 계산 <- [오브젝트 가로 사이즈]
    private Rect successZoneRect; // width값 계산

    private float makerspeed = 0.5f; // 마커 스피드
    private float gravity = 0.8f; // 스페이스 바 안눌렀을 때 떨어지는 속도

    private float totalTime = 10f; // 총 시간 
    private float successTime = 3f; // 성공에 필요한 시간
    private float progressTime = 0f; // 성공 범위에 얼마나 있었는지 확인
    private float curTime = 0f; // 현재 시간 그냥 이건 타이머랑 연관됨
    private float playerVelocity = 0f; // 플레이어 속도 <- 마커랑 연관됨

    private bool isReeling = false; // 키 눌름과 연관됨
    private bool isFishingPlaying = false; // 미니게임 여부와 연관

    [SerializeField]
    TabletSceneManager tabletSceneManager;

    [SerializeField]
    private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => ClickExitButton());
    }

    public void StartFishing()
    {
        if(tabletSceneManager.isPlaying)
        {
            // 미니게임 시작시 패널 활성화 및 게임 여부
            fishingPanel.SetActive(true);
            isFishingPlaying = true;

            // 이건 알겠지 뭐 대충 초기화 내용
            curTime = totalTime;
            progressTime = 0f;
            playerVelocity = 0f;

            GetDay();
            SetDifficultyLevel();

            // 성공 범위 배치 함수
            SuccessZonePlacement();
        }
    }

    private int GetDay()
    {
        return DaySystem.Instance.GetNowDay();
    }
    private void SetDifficultyLevel()
    {
        int day = GetDay();

        float zoneRange = 30f;
        float makerVelocity = 5f;
        float gravityVelocity = 5f;

        if (day == 1) { zoneRange = 30f; makerVelocity = 5f; gravityVelocity = 5f; }
        else if (day == 2) { zoneRange = 25f; }//makerVelocity = 5f; //gravityVelocity = 5f; }
        else if (day == 3) { zoneRange = 20f; }//makerVelocity = 8f; //gravityVelocity = 7f; }
        else if (day == 4) { zoneRange = 15f; }//makerVelocity = 8f; //gravityVelocity = 7f; }
        else if (day >= 5) { zoneRange = 10f; }//makerVelocity = 9f; //gravityVelocity = 10f; }

            makerspeed = makerVelocity * 0.1f;
            gravity = gravityVelocity * 0.1f;

            float totalWidth = gaugeBarRect.width > 0 ? gaugeBarRect.width : 500f;

            float newWidth = totalWidth * (zoneRange / 100f);

            successZoneTransform.sizeDelta = new Vector2(newWidth, successZoneTransform.sizeDelta.y);

            successZoneRect = successZoneTransform.rect;

    }
    private void Start()
    {
        // 게임 시작 시 패널 비활성화 및 미니게임에 쓰인 오브젝트의 값 가져오기
        fishingPanel.SetActive(false);
        // 해당 변수들이 가지고있는 크기값 주기
        gaugeBarRect = gaugeBarTransform.rect;
        successZoneRect = successZoneTransform.rect;
    }

    private void Update()
    {
        if(fishingPanel.activeSelf)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                EndGame();
            }
        }

        if (!isFishingPlaying) return;

        // 매번 부르는 함수 - 시간 확인, 키 눌림, 성공 범위 시간 확인
        CheckTimer();
        PressSpaceBar();
        CheckSuccessTime();
    }

    // 랜덤된 곳에 성공 범위 배치 함수
    private void SuccessZonePlacement()
    {
        float zoneHalfWidth = successZoneRect.width / 2;
        // 아까 받아온 게이지 바 크기의 x의 최소값 최대값 선언 그다음 랜덤으로 값 부르기
        float minX = gaugeBarRect.xMin + zoneHalfWidth;
        float maxX = gaugeBarRect.xMax - zoneHalfWidth;
        float randomX = Random.Range(minX, maxX);

        if(minX > maxX)
        {
            minX = 0f;
            maxX = 0f;
        }

        // 성공 범위의 Pos X의 값을 랜덤x 로 배치
        successZoneTransform.anchoredPosition = new Vector2(randomX, successZoneTransform.anchoredPosition.y);
        // 얘는 중앙 시작
        playerMakerTransform.anchoredPosition = new Vector2(0f, playerMakerTransform.anchoredPosition.y);
    }

    private void CheckTimer()
    {
        // 시간 업데이트 함수고 대충 시간 지나면 끝
        curTime -= Time.deltaTime;
        Timer.text = curTime.ToString("F2") + "s";

        if (curTime <= 0)
        {
            EndGame();
        }
    }

    private void PressSpaceBar()
    {
        // 대충 키 잘 작동하냐 그리고 스페이스 바 눌렸냐
        isReeling = Mouse.current != null && Mouse.current.leftButton.isPressed;

        // 트루면 플레이어 마커 오른쪽 이동 아니면 왼쪽 이동
        if (isReeling)
        {
            playerVelocity = makerspeed;
        }
        else
        {
            playerVelocity -= gravity * Time.deltaTime;
        }

        // 트루일 때 마커의 pos x 값에 플레이어 속도(마커 속도) 그래서 오른쪽 이동함
        float makerX = playerMakerTransform.anchoredPosition.x + playerVelocity * Time.deltaTime;

        // 마커가 게이지 바 안 넘어가게 하려고 최소 최대 정해둠
        float bar = gaugeBarRect.width / 2f;
        makerX = Mathf.Clamp(makerX, (-bar + 0.1f), (bar - 0.08f));

        // 그래서 마커위치는 저 위에 makerX에서 계산된 값으로 간다
        playerMakerTransform.anchoredPosition = new Vector2(makerX, playerMakerTransform.anchoredPosition.y);
    }

    private void CheckSuccessTime()
    {
        // 성공 범위 확인 맞으면 
        if (IsMakerInSuccessZone())
        {
            // 성공 범위 내부에 있는 시간 표시 되면 성공 나가면 0으로 초기화
            progressTime += Time.deltaTime;
            Progress.text = ((progressTime / 0.3) * 10).ToString("F1") + "%";

            if (progressTime >= successTime)
            {
                StabilityManager.Instance.StabilizationUp(10, 1);
                EndGame();
            }
        }
        else
        {
            progressTime = 0f;
            Progress.text = progressTime.ToString("F1") + "%";
        }
    }

    private bool IsMakerInSuccessZone()
    {
        // 성공 범위 확인 함수
        if (playerMakerTransform == null && successZoneTransform == null) return false;

        // 마커 위치 확인용 그리고 걍 성공 범위 좌우해서 최소 최대 안에 있으면 맞음 아님 리턴
        float makerX = playerMakerTransform.anchoredPosition.x;
        float successMin = successZoneTransform.anchoredPosition.x - (successZoneRect.width / 2f);
        float successMax = successZoneTransform.anchoredPosition.x + (successZoneRect.width / 2f);

        return makerX >= successMin && makerX <= successMax;
    }

    private void ClickExitButton()
    {
        if(fishingPanel.activeSelf)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        // 이건 걍 미니 게임 끝났을 때 하는 거
        isFishingPlaying = false;
        fishingPanel.SetActive(false);
        tabletSceneManager.tabletPanels[1].SetActive(true);
        tabletSceneManager.isPlaying = false;
    }
}
