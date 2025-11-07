using UnityEngine;

public class LivingOrbBreathScale : MonoBehaviour
{
    public float amplitude = 0.02f;
    public float speed = 1.6f;
    [Range(0f, 6.28318f)] public float phaseOffset = 0f;
    Vector3 origin;

    void Awake()
    {
        origin = transform.localScale;
        phaseOffset += Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float s = 1f + amplitude * Mathf.Sin(Time.time * speed + phaseOffset);
        transform.localScale = origin * s;
    }
}
