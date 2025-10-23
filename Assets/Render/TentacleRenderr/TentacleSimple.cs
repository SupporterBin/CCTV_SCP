using UnityEngine;

public class TentacleSimple : MonoBehaviour
{
    public SimpleTentacleSine tentaclePrefab;
    public TentacleSplineTube tubePrefab;
    public Transform blobCenter;
    public int count = 12;
    public Vector3 spawnRadius = new Vector3(0.05f, 0.05f, 0.05f);

    public Material tubeMaterial;
    [Header("TubeRad A-B")]
    public float rootRadius = 0;
    public float tipRadius = 0;

    public bool EnableDirOffset = false;
    public Axis3D alongAxis = Axis3D.Z;         // 뻗는 주축 (오브젝트 로컬 기준)
    public bool reverseAlong = false;           // 음의 방향으로 뻗을지
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var t = Instantiate(tentaclePrefab, transform);
            t.transform.localPosition = new Vector3(
                Random.Range(-spawnRadius.x, spawnRadius.x),
                Random.Range(-spawnRadius.y, spawnRadius.y),
                Random.Range(-spawnRadius.z, spawnRadius.z)
            );
            if (EnableDirOffset)
            {
                t.alongAxis = alongAxis;
                t.reverseAlong = reverseAlong;
            }
            var tube = Instantiate(tubePrefab, t.transform);
            tube.Init(rootRadius, tipRadius);
            tube.transform.localPosition = Vector3.zero;
            tube.source = t;
            if (tubeMaterial) tube.GetComponent<MeshRenderer>().sharedMaterial = tubeMaterial;
        }
    }
}
