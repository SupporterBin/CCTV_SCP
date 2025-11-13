using System.Threading;
using UnityEngine;

public class TentacleWallhug : MonoBehaviour
{
    public SimpleTentacleSine tentaclePrefab;
    SimpleTentacleSine[] createdTentaclePrefab ;
    public TentacleSplineTube tubePrefab;
    public Transform blobCenter;
    public int count = 12;
    public Vector3 spawnRadius = new Vector3(0.05f, 0.05f, 0.05f);


    public Material tubeMaterial;
    [Header("TubeRad A-B")]
    public float rootRadius = 0;
    public float tipRadius = 0;
    [Header("ChainDamp")]
    public float damp = 0;

    public bool EnableDirOffset = false;
    public Axis3D alongAxis = Axis3D.Z;         // ���� ���� (������Ʈ ���� ����)
    public bool reverseAlong = false;           // ���� �������� ������

    void Start()
    {
        createdTentaclePrefab = new SimpleTentacleSine[count];
        // y 0~4�� ����
        for (int i = 0; i < count; i++)
        {
            var t = Instantiate(tentaclePrefab, transform);
            t.transform.localPosition = new Vector3(
                Random.Range(-spawnRadius.x, spawnRadius.x),
                spawnRadius.y * i / (float)count,
                Random.Range(-spawnRadius.z, spawnRadius.z)
            );
            createdTentaclePrefab[i] = t;
            t.phaseOffset = Random.Range(0, 2);
            t.speed = Random.Range(0.7f, 1.2f);
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
        foreach(var g in createdTentaclePrefab)
        {
            g.gameObject.SetActive(false);
            g.gameObject.SetActive(true);
        }
    }
    private void OnEnable()
    {
        grow = true;
        len = 0;
        if (createdTentaclePrefab == null) return;
        foreach (var t in createdTentaclePrefab)
        {
            t.totalLength = len;
        }
    }
    float len = 0;
    float maxlen = 13;
    bool grow = true;
    public void FadeAwayTentacle()
    {
        grow = false;
    }
    private void Update()
    {
        if (len > maxlen && grow == true)
            return;
        else if (grow == false && len <= 0.01f)
        {
            gameObject.SetActive(false);
            return;
        }

        if(grow )
            len += Time.deltaTime;    
        else 
            len -= Time.deltaTime;

        foreach( var t in createdTentaclePrefab )
        {
            t.totalLength = len;
        }
    }
}
