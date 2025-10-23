using System.Collections.Generic;
using System.Collections;
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
    [SerializeField]
    private PlayerMove move;

    // 플레이어 카메라와 태블릿 카메라 위치 및 회전 값
    [SerializeField]
    private Transform playerCamera;

    /*[SerializeField]
    private MonoBehaviour playerMove;*/

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
    public bool isMovingCamera = false;
    // 플레이어 시작 FOV, Far 값
    private float startFov;
    private float startFar;

    // 세팅한 Test의 위치, 회전, FOV, Far값
    // 왼쪽 가운데 오른쪽 
    private Vector3 LeftCCTVPosition = new Vector3(-0.364f, 1.571f, -1.39f);
    private Quaternion LeftCCTVRotation = Quaternion.Euler(0f, -24.904f, 0f);
    private Vector3 CenterCCTVPosition = new Vector3(0, 1.571f, -1.32f);
    private Quaternion CenterCCTVRotation = Quaternion.Euler(0f, 0f, 0f);
    private Vector3 RightCCTVPosition = new Vector3(0.364f, 1.571f, -1.39f);
    private Quaternion RightCCTVRotation = Quaternion.Euler(0f, 24.904f, 0f);
    private float CCTVFov = 21f;
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
        if (move != null)
        {
            move.enabled = true;
        }

        playerCamSetting = playerCamera.GetComponent<Camera>();
        if (playerCamSetting != null)
        {
            startFov = playerCamSetting.fieldOfView;
            startFar = playerCamSetting.farClipPlane;
        }
        else
        {
            Debug.Log("플레이어 카메라 세팅 null");
        }

        timer = moveDuration + 5f;

    }

    public bool IsMoving()
    {
        return isMovingCamera;
    }

    public void CCTV_Pos_Rot(CCTVLocation Location)
    {

        if (isMovingCamera)
        {
            return;
        }
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

        if (isOnCCTV && curCCTVPos == targetPos)
        {
            isOnCCTV = false;
            StartCoroutine(MovingCamera(false));
        }
        else
        {
            if (!isOnCCTV)
            {
                originalPlayerPos = renewalPos;
                originalPlayerRot = renewalRot;
            }

            isOnCCTV = true;
            curCCTVPos = targetPos;
            curCCTVRot = targetRot;
            StartCoroutine(MovingCamera(true));
        }
    }

    private IEnumerator MovingCamera(bool ismoving)
    {
        isMovingCamera = true;
        float timer = 0f;

        if (ismoving)
        {
            if (move != null && move.enabled)
            {
                move.enabled = false;
            }
        }

        Vector3 startPos = renewalPos;
        Quaternion startRot = renewalRot;
        float startFov = renewalFov;
        float startFar = renewalFar;

        Vector3 endPos = ismoving ? curCCTVPos : originalPlayerPos;
        Quaternion endRot = ismoving ? curCCTVRot : originalPlayerRot;
        float endFov = ismoving ? CCTVFov : this.startFov;
        float endFar = ismoving ? CCTVFar : this.startFar;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            playerCamSetting.fieldOfView = Mathf.Lerp(startFov, endFov, t);
            playerCamSetting.farClipPlane = Mathf.Lerp(startFar, endFar, t);

            yield return null;
        }

        if (!ismoving)
        {
            yield return new WaitForSeconds(0.1f);

            if (move != null && !move.enabled)
            {
                move.enabled = true;
            }
        }

        isMovingCamera = false;
    }
}

