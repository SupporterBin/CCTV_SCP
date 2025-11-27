using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class CorruptedVramPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("CorruptedVram");
    Material m_Material;
    RTHandle m_CameraColorTarget;

    public CorruptedVramPass(Material material, RenderPassEvent passEvent)
    {
        m_Material = material;
        renderPassEvent = passEvent;
        renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    }

// RendererFeature 쪽에서 카메라 컬러 버퍼를 넘겨줄 때 호출됨
public void SetTarget(RTHandle colorHandle)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        var camera = cameraData.camera;

        // 게임 뷰가 아닌 카메라는 무시
        if (cameraData.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        // 이 카메라에 효과 설정 컴포넌트가 없으면 패스
        var effect = camera.GetComponent<ShaderEffect_CorruptedVram>();
        if (effect == null || !effect.isActiveAndEnabled)
            return;

        // 머티리얼 파라미터 세팅
        m_Material.SetFloat("_ValueX", effect.shift);
        if (effect.patternTexture != null)
            m_Material.SetTexture("_Texture", effect.patternTexture);

        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            // 카메라 컬러 텍스처를 자기 자신으로 Blit (후처리)
            Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
