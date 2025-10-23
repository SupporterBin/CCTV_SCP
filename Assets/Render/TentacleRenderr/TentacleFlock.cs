// TentacleFlock.cs
using UnityEngine;

public class TentacleFlock : MonoBehaviour
{
    public TentacleVerletChain tentaclePrefab;
    public TentacleSplineTube tubePrefab;
    public Transform blobCenter;
    public int count = 12;
    public Vector3 spawnRadius = new Vector3(0.05f, 0.05f, 0.05f);

    [Header("Random Ranges")]
    public Vector2 outwardBias = new Vector2(0f, 1.2f);
    public Vector2 targetLenScale = new Vector2(0.9f, 1.3f);
    public Vector2 noiseAmp = new Vector2(0.6f, 1.1f);
    public Vector2 noiseFreq = new Vector2(0.6f, 1.0f);

    public Material tubeMaterial;
    [Header("TubeRad A-B")]
    public float rootRadius = 0;
    public float tipRadius = 0;
    [Header("ChainDamp")]
    public float damp = 0;

    public bool applyFlow = false;
    public Vector3 flow = Vector3.zero;
    public bool applySegment = false;
    public int segment = 12;
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
            t.blobCenter = blobCenter != null ? blobCenter : transform;
            t.outwardBias = Random.Range(outwardBias.x, outwardBias.y);
            t.targetLenScale = Random.Range(targetLenScale.x, targetLenScale.y);
            t.noiseAmp = Random.Range(noiseAmp.x, noiseAmp.y);
            t.noiseFreq = Random.Range(noiseFreq.x, noiseFreq.y);
            if(applySegment)
                t.segments = segment;
            if(applyFlow)
                t.flow = flow;
            
            var tube = Instantiate(tubePrefab, t.transform);
            tube.Init(rootRadius, tipRadius);
            tube.transform.localPosition = Vector3.zero;
            tube.source = t;
            if (tubeMaterial) tube.GetComponent<MeshRenderer>().sharedMaterial = tubeMaterial;
        }
    }
}
