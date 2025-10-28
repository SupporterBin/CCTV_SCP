using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.TimeZoneInfo;
// 해당 테스트 스크립트입니다. 
public class TabletManager : MonoBehaviour
{
    // 플레이어 카메라와 태블릿 카메라 위치 및 회전 값
    [SerializeField]
    private Transform playerCamera;

    // 태블릿을 켰을 때 이동 관련
    [SerializeField]
    private PlayerMove move;

    // // 태블릿 온 오프시 시점 이동 및 시간 관련 변수 [부드러운 시점 변환?]
    private Vector3 startCameraPosition;
    private Quaternion startCameraRotation;

    // 플레이어 시작 FOV, Far 값
    private float startFov;
    private float startFar;

    private float moveDuration = 0.5f;
    private float timer = 0f;

    // 태블릿 온 오프 여부
    public bool isOnTablet = false;
    public bool isMovingTabletCamera = false;

    // 세팅한 Test의 위치, 회전, FOV, Far값
    private Vector3 tabletPosition = new Vector3(-2.045f, 2.5f, -0.464f);
    private Quaternion tabletRotation = Quaternion.Euler(90f, 0f, 0f);
    private float tabletFov = 13f;
    private float tabletFar = 1000f;

    // 카메라 세팅 관련
    //private Camera playerCamSetting;
    private CinemachineCamera playerCamSetting;

    [SerializeField]
    private Button[] interactionTabletButtons;

    [SerializeField]
    private Canvas crossHairCanvas;

    void Start()
    {
        // 그냥 넣음 이건
        if (move != null)
        {
            move.enabled = true;
        }

        //playerCamSetting = playerCamera.GetComponent<Camera>();
        playerCamSetting = playerCamera.GetComponent<CinemachineCamera>();
        if (playerCamSetting != null)
        {
            //startFov = playerCamSetting.fieldOfView;
            //startFar = playerCamSetting.farClipPlane;
            startFov = playerCamSetting.Lens.FieldOfView;
            startFar = playerCamSetting.Lens.FarClipPlane;
        }
        else
        {
            Debug.Log("플레이어 카메라 세팅 null");
        }

        timer = moveDuration + 5f;
    }

    private void Update()
    {
        if (!isOnTablet)
        {
            crossHairCanvas.enabled = true;
            for (int i = 0; i < interactionTabletButtons.Length; i++)
            {
                interactionTabletButtons[i].interactable = false;
            }
        }
        else
        {
            crossHairCanvas.enabled = false;
            for (int i = 0; i < interactionTabletButtons.Length; i++)
            {
                interactionTabletButtons[i].interactable = true;
            }
        }

        if (isOnTablet && !isMovingTabletCamera && Input.GetKeyDown(KeyCode.F))
        {
            isOnTablet = false;
            StartCoroutine(MovingTabletCamera(false));
        }
    }

    public bool IsMoving()
    {
        return isMovingTabletCamera;
    }

    public void MovingTabletView()
    {
        if (isMovingTabletCamera || isOnTablet)
        {
            return;
        }
        isOnTablet = true;

        StartCoroutine(MovingTabletCamera(true));
    }

    private IEnumerator MovingTabletCamera(bool ismoving)
    {
        isMovingTabletCamera = true;
        timer = 0f;

        if (ismoving)
        {
            if (move != null && move.enabled)
            {
                move.enabled = false;
            }
        }

        Vector3 startPos;
        Quaternion startRot;
        float startFov;
        float startFar;

        if (ismoving) // 켜질 때
        {
            // 현재 플레이어 위치를 start.. 으로 저장
            startPos = playerCamera.transform.position;
            startRot = playerCamera.transform.rotation;
            //startFov = playerCamSetting.fieldOfView;
            //startFar = playerCamSetting.farClipPlane;
            startFov = playerCamSetting.Lens.FieldOfView;
            startFar = playerCamSetting.Lens.FarClipPlane;

            // 나중에 꺼질 때를 위해 각각 위치 회전 값 저장
            startCameraPosition = startPos;
            startCameraRotation = startRot;
        }
        else // 꺼질 때
        {
            // 태블릿에서 시작하기에 각각 시작 점을 태블릿의 값으로 저장
            startPos = tabletPosition;
            startRot = tabletRotation;
            startFov = tabletFov;
            startFar = tabletFar;
        }
        // ismoving 즉 켜질 때면 도착 지점의 값을 tablet 아니면 꺼질 때니까 플레이어 값
        Vector3 endPos = ismoving ? tabletPosition : startCameraPosition;
        Quaternion endRot = ismoving ? tabletRotation : startCameraRotation;
        float endFov = ismoving ? tabletFov : this.startFov;
        float endFar = ismoving ? tabletFar : this.startFar;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            //playerCamSetting.fieldOfView = Mathf.Lerp(startFov, endFov, t);
            //playerCamSetting.farClipPlane = Mathf.Lerp(startFar, endFar, t);
            playerCamSetting.Lens.FieldOfView = Mathf.Lerp(startFov, endFov, t);
            playerCamSetting.Lens.FarClipPlane = Mathf.Lerp(startFar, endFar, t);

            yield return null;
        }

        if (!ismoving) /// 움직일 때가 아닐 때
        {
            yield return new WaitForSeconds(0.2f);

            if (move != null && !move.enabled)
            {
                move.enabled = true;
            }
        }

        isMovingTabletCamera = false;
    }
}
