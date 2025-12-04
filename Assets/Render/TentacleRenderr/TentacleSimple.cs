using UnityEngine.AI;
using UnityEngine;

public class TentacleSimple : MonoBehaviour
{
    public SimpleTentacleSine tentaclePrefab;
    SimpleTentacleSine instance;
    public TentacleSplineTube tubePrefab;
    public int count = 12;
    public Vector3 spawnRadius = new Vector3(0.05f, 0.05f, 0.05f);


    [SerializeField] private Transform tipTransform;

    public Material tubeMaterial;
    [Header("TubeRad A-B")]
    public float rootRadius = 0;
    public float tipRadius = 0;

    [Header("Nav Path (공통 설정)")]
    public bool useNavPathMode = true;
    public NavMeshAgent sharedAgent;    // 하나만 두고 경로 계산용으로 써도 되고,
    public Transform sharedTarget;      // 플레이어 같은 공통 타겟
    public float headMoveSpeed = 3f;
    public float waitAtTargetTime = 0.5f;

    // ... 기존 Sine 옵션들 생략 ...

    void Start()
    {
        SimpleTentacleSine t = null;
        for (int i = 0; i < count; i++)
        {
            t = Instantiate(tentaclePrefab, transform);
            instance = t;
            t.transform.localPosition = new Vector3(
                Random.Range(-spawnRadius.x, spawnRadius.x),
                Random.Range(-spawnRadius.y, spawnRadius.y),
                Random.Range(-spawnRadius.z, spawnRadius.z)
            );
            var tube = Instantiate(tubePrefab, t.transform);
            tube.Init(rootRadius, tipRadius);
            tube.transform.localPosition = Vector3.zero;
            tube.source = t;
            if (tubeMaterial) tube.GetComponent<MeshRenderer>().sharedMaterial = tubeMaterial;
        }
        // NavMesh 모드 세팅
        if (useNavPathMode)
        {
            t.useNavPathMode = true;
            t.navAgent = sharedAgent;
            t.navTarget = sharedTarget;
            t.headMoveSpeed = headMoveSpeed;
            t.waitAtTargetTime = waitAtTargetTime;
            t.autoReturnAfterWait = false; // 필요하면 true
            //t.StartPathToTarget();
        }

        //}
    }

    private void Update()
    {
        if(tipTransform == null) return;
        tipTransform.position = instance.TipWorldPosition;
    }

    public void StartGrab()
    {
        instance.StartPathToTarget();
    }
    public void StartReturn()
    {
        instance.StartReturn();
    }
}
