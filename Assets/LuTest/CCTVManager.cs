using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum CCTVLocation
{
    Left,
    Center,
    Right
}

public class CCTVManager : MonoBehaviour
{
    // 플레이어 카메라와 태블릿 카메라 위치 및 회전 값
    [SerializeField]
    private Transform playerCamera;

    // cctv을 켰을 때 이동 관련
    [SerializeField]
    private MonoBehaviour playerMove;

    [SerializeField]
    private GameObject[] panels;

    // // 태블릿 온 오프시 시점 이동 및 시간 관련 변수 [부드러운 시점 변환?]
    // 시작했던 그 원래 위치
    private Vector3 originalPlayerPos;
    private Quaternion originalPlayerRot;

    // 현재 값
    private Vector3 curCCTVPos;
    private Quaternion curCCTVRot;

    // 갱신 값
    private Vector3 renewalPos;
    private Quaternion renewalRot;

    private float renewalFov;
    private float renewalFar;

    private float moveDuration = 0.5f;
    private float timer = 0f;

    // 태블릿 온 오프 여부
    public bool isOnCCTV = false;
    // 플레이어 시작 FOV, Far 값
    private float startFOV;
    private float startFar;

    // 세팅한 Test의 위치, 회전, FOV, Far값
    // 왼쪽 가운데 오른쪽 
    private Vector3 LeftCCTVPosition = new Vector3(-0.364f, 1.571f, -1.39f);
    private Quaternion LeftCCTVRotation = Quaternion.Euler(0f, -24.904f, 0f);
    private Vector3 CenterCCTVPosition = new Vector3(0, 1.571f, -1.32f);
    private Quaternion CenterCCTVRotation = Quaternion.Euler(0f, 0f, 0f);
    private Vector3 RightCCTVPosition = new Vector3(0.364f, 1.571f, -1.39f);
    private Quaternion RightCCTVRotation = Quaternion.Euler(0f, 24.904f, 0f);
    private float CCTVFOV = 21f;
    private float CCTVFar = 3f;

    // 카메라 세팅 관련
    private Camera playerCamSetting;

    private void Awake()
    {
        panels[0].transform.Find("RightBt").GetComponent<Button>().onClick.AddListener(() => CCTV_Pos_Rot(CCTVLocation.Center));
        panels[1].transform.Find("LeftBt").GetComponent<Button>().onClick.AddListener(() => CCTV_Pos_Rot(CCTVLocation.Left));
        panels[1].transform.Find("RightBt").GetComponent<Button>().onClick.AddListener(() => CCTV_Pos_Rot(CCTVLocation.Right));
        panels[2].transform.Find("LeftBt").GetComponent<Button>().onClick.AddListener(() => CCTV_Pos_Rot(CCTVLocation.Center));
    }

    void Start()
    {
        // 그냥 넣음 이건
        if (playerMove != null)
        {
            playerMove.enabled = true;
        }

        playerCamSetting = playerCamera.GetComponent<Camera>();
        if (playerCamSetting != null)
        {
            startFOV = playerCamSetting.fieldOfView;
            startFar = playerCamSetting.farClipPlane;
        }
        else
        {
            Debug.Log("플레이어 카메라 세팅 null");
        }

        timer = moveDuration + 5f;
    }

    void Update()
    {
        if (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            if (playerCamSetting != null)
            {
                if (isOnCCTV)
                {
                    playerCamera.transform.position = Vector3.Lerp(renewalPos, curCCTVPos, t);
                    playerCamera.transform.rotation = Quaternion.Slerp(renewalRot, curCCTVRot, t);

                    playerCamSetting.fieldOfView = Mathf.Lerp(renewalFov, CCTVFOV, t);
                    playerCamSetting.farClipPlane = Mathf.Lerp(renewalFar, CCTVFar, t);
                }
                else
                {
                    playerCamera.transform.position = Vector3.Lerp(renewalPos, originalPlayerPos, t);
                    playerCamera.transform.rotation = Quaternion.Slerp(renewalRot, originalPlayerRot, t);

                    playerCamSetting.fieldOfView = Mathf.Lerp(renewalFov, startFOV, t);
                    playerCamSetting.farClipPlane = Mathf.Lerp(renewalFar, startFar, t);
                }
            }
        }

    }

    public void CCTV_Pos_Rot(CCTVLocation Location)
    {
        renewalPos = playerCamera.transform.position;
        renewalRot = playerCamera.transform.rotation;

        renewalFov = playerCamSetting.fieldOfView;
        renewalFar = playerCamSetting.farClipPlane;

        Vector3 targetPos;
        Quaternion targetRot;

        if (Location == CCTVLocation.Left)
        {
            targetPos = LeftCCTVPosition;
            targetRot = LeftCCTVRotation;

        }
        else if (Location == CCTVLocation.Center)
        {
            targetPos = CenterCCTVPosition;
            targetRot = CenterCCTVRotation;
        }
        else
        {
            targetPos = RightCCTVPosition;
            targetRot = RightCCTVRotation;
        }

        if(isOnCCTV && curCCTVPos == targetPos)
        {
            isOnCCTV = false;
            if(playerMove != null) 
            {
                playerMove.enabled = true;
            }
        }
        else
        {
            if(!isOnCCTV)
            {
                originalPlayerPos = renewalPos;
                originalPlayerRot = renewalRot;
                if(playerMove != null)
                {
                    playerMove.enabled = false;
                }
            }

            curCCTVPos = targetPos;
            curCCTVRot = targetRot;
            isOnCCTV = true;
        }

        timer = 0;
    }
}

