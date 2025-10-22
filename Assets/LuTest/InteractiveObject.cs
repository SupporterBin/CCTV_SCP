using UnityEditor;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    [Header("상호작용 거리")]
    [SerializeField]
    private float InteractionDistance = 5f;

    [Header("상호작용 오브젝트")]
    [SerializeField]
    private GameObject LeftMonitor;

    [SerializeField]
    private GameObject CenterMonitor;

    [SerializeField]
    private GameObject RightMonitor;

    [SerializeField]
    private GameObject Tablet;

    private GameObject rayCollisionObject;

    [SerializeField]
    private CCTVManager cctvManager;
    // 아 핫로드 왜 안돼

    // Update is called once per frame
    void Update()
    {
        InteractionObject();

        if(Input.GetKeyDown(KeyCode.F) && rayCollisionObject != null)
        {
            if (rayCollisionObject == LeftMonitor)
            {
                InteractLeftMonitor();
            }
            else if (rayCollisionObject == CenterMonitor)
            {
                InteractCenterMonitor();
            }
            else if (rayCollisionObject == RightMonitor)
            {
                InteractRightMonitor();
            }
            else if (rayCollisionObject == Tablet)
            {
                InteractTablet();
            }
        }
    }

    void InteractionObject()
    {
        // ray는 스크린 가로 세로의 중앙의 값 정중앙
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        Debug.DrawRay(ray.origin, ray.direction * InteractionDistance, Color.blue);

        RaycastHit hit; // Hit 당한 오브젝트를 hit에 저장

        if (Physics.Raycast(ray, out hit, InteractionDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            if(hitObject == LeftMonitor || hitObject == CenterMonitor || 
                hitObject == RightMonitor || hitObject == Tablet)
            {
                rayCollisionObject = hitObject;
            }
            else
            {
                rayCollisionObject = null;
            }
        }
        else
        {
            rayCollisionObject = null;
        }
    }

    void InteractLeftMonitor()
    {
        cctvManager.CCTV_Pos_Rot(CCTVLocation.Left);
        Debug.Log("왼");
    }
    void InteractCenterMonitor()
    {
        cctvManager.CCTV_Pos_Rot(CCTVLocation.Center);
        Debug.Log("가");
    }

    void InteractRightMonitor()
    {
        cctvManager.CCTV_Pos_Rot(CCTVLocation.Right);
        Debug.Log("오");
    }
    void InteractTablet()
    {
        
    }
}
