using UnityEngine;

[CreateAssetMenu(
    fileName = "SoundData",
    menuName = "Game/Sound Data",
    order = 0)]
public class SoundData : ScriptableObject
{
    // =========================
    // 시스템 UI
    // =========================

    [Header("시스템 UI - 버튼 관련")]

    [Tooltip("타이틀 씬에서 시작버튼 클릭 시 출력")]
    public AudioClip systemUiStartButtonClick;              // 스타트 버튼 클릭

    [Tooltip("게임오버 씬에서 타이틀 버튼 클릭 시 출력")]
    public AudioClip systemUiTitleButtonClick;              // 타이틀 버튼 클릭

    [Tooltip("일차 종료 씬에서 다음날 버튼 클릭 시 출력")]
    public AudioClip systemUiNextDayButtonClick;            // 다음날 버튼 클릭

    [Tooltip("게임오버 씬에서 리트라이 버튼 클릭 시 출력")]
    public AudioClip systemUiRetryButtonClick;              // 리트라이 버튼 클릭


    [Header("시스템 UI - 시스템 연출 / BGM")]

    [Tooltip("시간이 06:00에 도달해 일차 종료 씬에 진입 시 출력되는 알람시계 효과음")]
    public AudioClip systemUiDayEndEnterAlarmClock;         // 일차 종료 씬 진입

    [Tooltip("타이틀 씬 진입 시 출력되는 배경음악")]
    public AudioClip systemUiBgmTitle;                      // 타이틀씬 진입 BGM

    [Tooltip("게임오버 씬 진입 시 출력되는 배경음악")]
    public AudioClip systemUiBgmGameOver;                   // 게임오버씬 진입 BGM

    [Tooltip("엔딩 씬 진입 시 출력되는 배경음악")]
    public AudioClip systemUiBgmEnding;                     // 엔딩씬 진입 BGM


    // =========================
    // 인게임 플레이
    // =========================

    [Header("인게임 플레이 - 플레이어 조작")]

    [Tooltip("플레이어 이동 시 출력되는 콘크리트 걷는 소리 (필요 시 배속)")]
    public AudioClip ingamePlayerWalkConcrete;              // 플레이어 걷기

    [Tooltip("F를 눌러 CCTV를 확대할 때 출력")]
    public AudioClip ingameCctvZoomIn;                      // CCTV 확대

    [Tooltip("F를 눌러 태블릿을 확대할 때 출력")]
    public AudioClip ingameTabletZoomIn;                    // 태블릿 확대


    [Header("인게임 플레이 - CCTV 대처 버튼 조작")]

    [Tooltip("CCTV UI의 소각 버튼 클릭 시 출력")]
    public AudioClip ingameCctvBurnButtonClick;             // 소각 버튼 클릭

    [Tooltip("CCTV UI의 전격 버튼 클릭 시 출력")]
    public AudioClip ingameCctvShockButtonClick;            // 전격 버튼 클릭

    [Tooltip("CCTV UI의 공진 버튼 클릭 시 출력")]
    public AudioClip ingameCctvResonanceButtonClick;        // 공진 버튼 클릭

    [Tooltip("CCTV UI의 시스템점검 버튼 클릭 시 출력")]
    public AudioClip ingameCctvSystemCheckButtonClick;      // 시스템점검 버튼 클릭

    [Tooltip("CCTV UI의 배식 버튼 클릭 시 출력")]
    public AudioClip ingameCctvFeedButtonClick;             // 배식 버튼 클릭


    [Header("인게임 플레이 - 메뉴얼 조작 / 연출")]

    [Tooltip("매뉴얼 페이지 변경 버튼 클릭 시 출력")]
    public AudioClip ingameManualPageChangeButtonClick;     // 페이지변경 버튼 클릭

    [Tooltip("게임 시작 및 프로토콜 개방 과정에서 문이 열릴 때 출력되는 유압 소리")]
    public AudioClip ingameDoorOpenHydraulic;               // 문 열리는 소리

    [Tooltip("게임 시작 및 프로토콜 개방 과정에서 문이 닫힐 때 출력되는 유압 소리")]
    public AudioClip ingameDoorCloseHydraulic;              // 문 닫히는 소리

    [Tooltip("게임 시작 후 메인 씬 진입 시 페이드인과 동시에 출력되는 엘리베이터 도착 소리")]
    public AudioClip ingameElevatorArriveDing;              // 엘리베이터 도착 소리


    // =========================
    // 이상현상
    // =========================

    [Header("이상현상 - 전선 미상개체")]

    [Tooltip("전선 과생성 이상현상 발동 시 창문쪽에서 출력 (스테레오)")]
    public AudioClip abnormalWireOvergrowthRubber;          // 과생성 이상현상

    [Tooltip("합선 이상현상 발동 시 창문쪽에서 반복 출력 (스테레오)")]
    public AudioClip abnormalWireShortCircuitSpark;         // 합선 이상현상


    [Header("이상현상 - 가면 미상개체")]

    [Tooltip("인간 변신 이상현상 발동 시 창문쪽에서 출력 (스테레오)")]
    public AudioClip abnormalMaskHumanTransform;            // 인간 변신 이상현상

    [Tooltip("우는 가면 이상현상 발동 시 생성된 가면쪽에서 출력 (스테레오)")]
    public AudioClip abnormalMaskCreationWeepingMan;        // 가면 생성 이상현상 (우는 소리)

    [Tooltip("생성된 우는 가면과 상호작용 시 출력되는 도자기 깨지는 소리")]
    public AudioClip abnormalMaskBreakCeramic;              // 가면 상호작용 (도자기 깨짐)


    [Header("이상현상 - 촉수 미상개체")]

    [Tooltip("공격성 증가 이상현상 발동 시 창문쪽에서 출력 (스테레오)")]
    public AudioClip abnormalTentacleAggressiveWindowHit;   // 공격성 이상현상

    [Tooltip("과성장 이상현상 발동 시 창문쪽에서 출력 (스테레오)")]
    public AudioClip abnormalTentacleOvergrowthSquirm;      // 과성장 이상현상


    // =========================
    // 미니게임
    // =========================

    [Header("미니게임 - 시스템 및 UI")]

    [Tooltip("미니게임 변경 버튼 클릭 시 출력")]
    public AudioClip minigameChangeGameButtonClick;         // 게임 전환 버튼 클릭

    [Tooltip("미니게임 시작 버튼 클릭 시 출력")]
    public AudioClip minigameStartButtonClick;              // 게임 시작 버튼 클릭

    [Tooltip("미니게임 성공 시 출력")]
    public AudioClip minigameSuccess;                       // 미니게임 성공

    [Tooltip("미니게임 실패 시 출력")]
    public AudioClip minigameFail;                          // 미니게임 실패


    [Header("미니게임 - 키패드 / 낚시 / 키 따라치기")]

    [Tooltip("키패드 선행 신호 1번당 1번 출력")]
    public AudioClip minigameKeypadLeadSignal;              // 선행신호 발생

    [Tooltip("키패드 버튼 클릭 시 출력")]
    public AudioClip minigameKeypadButtonPress;             // 키패드 버튼 누름

    [Tooltip("낚시게임에서 성공 존을 유지중인 동안 출력 (루프)")]
    public AudioClip minigameFishingSuccessZoneLoop;        // 성공 존 유지중

    [Tooltip("키 따라치기 미니게임에서 키 입력 시 출력")]
    public AudioClip minigameRhythmKeyInput;                // 키 입력


    // =========================
    // 데스신
    // =========================

    [Header("데스신 - 공통사항")]

    [Tooltip("안정수치 0에 진입 시 불꺼짐과 함께 출력되는 발전기 폭발음")]
    public AudioClip deathCommonStabilityZeroGeneratorExplode;  // 안정수치 0 진입

    [Tooltip("창문 깨지는 소리 출력 이후, 데스신 출력 때까지 반복되는 사이렌 소리")]
    public AudioClip deathCommonSirenLoopBeforeDeath;           // 안정수치 0~데스신 사이렌

    [Tooltip("데스신 출력 시작과 동시에 출력되는 창문 깨지는 소리")]
    public AudioClip deathCommonWindowBreak;                    // 데스신 출력 시작


    [Header("데스신 - 전선 미상개체")]

    [Tooltip("전선이 날아오는 타이밍에 출력되는 밧줄/채찍 휘두르는 소리")]
    public AudioClip deathWireWhipSwing;                        // 전선 날아올 때

    [Tooltip("날아온 전선이 조여질 때 출력되는 소리")]
    public AudioClip deathWireTighten;                          // 밧줄이 조여지는 소리

    [Tooltip("페이드아웃 시작과 함께 출력되는 사람이 목졸리는 소리")]
    public AudioClip deathWireStrangle;                         // 목졸리는 소리


    [Header("데스신 - 가면 미상개체")]

    [Tooltip("주위에 생성된 가면을 볼 때부터 페이드아웃 때까지 출력되는 여러 성별의 웃음 소리")]
    public AudioClip deathMaskCrowdLaugh;                       // 가면 둘러볼 때 ~ 써질 때

    [Tooltip("페이드아웃 시작과 함께 출력되는 남자의 낮은 웃음 소리")]
    public AudioClip deathMaskManLowLaugh;                      // 페이드아웃 시 낮은 웃음


    [Header("데스신 - 촉수 미상개체 (플레이스홀더)")]

    [Tooltip("TODO: 촉수 미상개체 데스신용 사운드 1")]
    public AudioClip deathTentaclePatternA;

    [Tooltip("TODO: 촉수 미상개체 데스신용 사운드 2")]
    public AudioClip deathTentaclePatternB;

    [Tooltip("TODO: 촉수 미상개체 데스신용 사운드 3")]
    public AudioClip deathTentaclePatternC;


    // =========================
    // 프로토콜
    // =========================

    [Header("프로토콜 - 조작")]

    [Tooltip("프로토콜 코드를 입력하는 키패드 버튼 클릭 시 출력되는 키보드 소리")]
    public AudioClip protocolKeypadButtonClick;                 // 키패드 버튼 클릭

    [Tooltip("프로토콜 코드 입력 후 로딩 상태일 때 출력 (루프 예상)")]
    public AudioClip protocolLoadingLoop;                       // 로딩중 출력

    [Tooltip("맞는 프로토콜 코드를 넣어 성공 시 출력되는 에너지 채워지는 효과음")]
    public AudioClip protocolSuccess;                           // 성공 사운드

    [Tooltip("틀린 프로토콜 코드를 넣어 실패 시 출력되는 효과음")]
    public AudioClip protocolFail;                              // 실패 사운드
}
