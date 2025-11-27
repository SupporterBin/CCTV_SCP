using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CorruptedVramRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader shader;                            // 사용할 쉐이더
        public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();

    CorruptedVramPass m_RenderPass;
    Material m_Material;

    public override void Create()
    {
        // 쉐이더가 인스펙터에서 안 채워져 있으면 자동으로 찾기
        if (settings.shader == null)
            settings.shader = Shader.Find("Hidden/Distortion");

        if (settings.shader != null)
        {
            m_Material = CoreUtils.CreateEngineMaterial(settings.shader);
            m_RenderPass = new CorruptedVramPass(m_Material, settings.passEvent);
        }
        else
        {
            Debug.LogError("[CorruptedVram] Shader 'Hidden/Distortion' not found.");
        }
    }

    // 카메라마다 매 프레임 호출 – 여기서 타깃 RTHandle 세팅
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (m_RenderPass == null)
            return;

        var camera = renderingData.cameraData.camera;
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        // 이 카메라에 효과 컴포넌트가 없으면 패스
        if (!camera.TryGetComponent<ShaderEffect_CorruptedVram>(out var effect) || !effect.isActiveAndEnabled)
            return;

        // 컬러 버퍼 읽기 권한 + 타깃 설정
        m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_RenderPass.SetTarget(renderer.cameraColorTargetHandle);
    }

    // 실제로 패스를 렌더 큐에 집어넣는 곳
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_RenderPass == null)
            return;

        var camera = renderingData.cameraData.camera;
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        if (!camera.TryGetComponent<ShaderEffect_CorruptedVram>(out var effect) || !effect.isActiveAndEnabled)
            return;

        renderer.EnqueuePass(m_RenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}
