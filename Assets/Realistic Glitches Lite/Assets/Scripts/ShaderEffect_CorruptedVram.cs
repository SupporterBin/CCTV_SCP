using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class ShaderEffect_CorruptedVram : MonoBehaviour
{
    private static ShaderEffect_CorruptedVram instance;
    public static ShaderEffect_CorruptedVram Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    [Header("Distortion Settings")]
    public float shift = 10f;              // 왜곡 강도
    public Texture patternTexture;         // Checkerboard-big 같은 패턴 텍스처

    // 여기서는 OnRenderImage 안 씀! (URP에서는 안 불림)
}
