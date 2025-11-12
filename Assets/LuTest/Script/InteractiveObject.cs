using System.Threading;
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

    [SerializeField]
    private TabletManager testManager;

    [SerializeField]
    private LayerMask outlineLayerMask;

    private MeshRenderer curOutlineScale = null;

    private MaterialPropertyBlock outlineBlock;

    [SerializeField]
    private string outlineProperty = "_Scale";

    private float offOutlineScale = 0f;

    private float onOutlineScale = 1.05f;

    private bool isOnRay = true;

    private void Start()
    {
        outlineBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        InteractionObject();

        if (cctvManager.isOnCCTV || testManager.isOnTablet || !isOnRay)
        {
            ShowOutLine(null);
        }

        if (Input.GetKeyDown(KeyCode.F) && rayCollisionObject != null && !cctvManager.IsMoving() && !testManager.IsMoving())
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
        Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit hit; // Hit 당한 오브젝트를 hit에 저장

        GameObject outLineObject = null;

        if (Physics.Raycast(ray, out hit, InteractionDistance, outlineLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == LeftMonitor || hitObject == CenterMonitor ||
                hitObject == RightMonitor || hitObject == Tablet)
            {
                rayCollisionObject = hitObject;
                outLineObject = hitObject;
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

        ShowOutLine(outLineObject);
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
        testManager.MovingTabletView();
    }

    private void ShowOutLine(GameObject outlineable)
    {
        MeshRenderer newOutLineObject = null;

        if (outlineable != null)
        {
            newOutLineObject = outlineable.GetComponent<MeshRenderer>();

        }

        if (newOutLineObject != curOutlineScale)
        {
            if (curOutlineScale != null)
            {
                outlineBlock.SetFloat(outlineProperty, offOutlineScale);
                curOutlineScale.SetPropertyBlock(outlineBlock, 1);

                Debug.Log("끄기");
            }

            if (newOutLineObject != null)
            {
                outlineBlock.SetFloat(outlineProperty, onOutlineScale);
                newOutLineObject.SetPropertyBlock(outlineBlock, 1);

                Debug.Log("켜기");
            }

            curOutlineScale = newOutLineObject;
        }

    }
    public void SetRay(bool ray)
    {
        isOnRay = ray;

        if (!isOnRay)
        {
            ShowOutLine(null);
        }
    }
}
