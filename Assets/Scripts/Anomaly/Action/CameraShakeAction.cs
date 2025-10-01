using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Actions (Second)/Camera Shake")]

public class CameraShakeAction : BaseAnomalyActionSO
{
    //진폭의 정도
    [Range(0f, 5f)] public float amplitude = 0.8f;
    //진폭의 시간
    [Range(0f, 5f)] public float duration = 0.3f;

    public override bool Validate(out string error)
    {
        if(duration <= 0.5f) { error = "duration must be > 0"; return false; }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        //설정값 호출
        Log($"Shake A = {amplitude}, T = {duration}");
        //ICameraShaker.Instance.Shake(amplitude, duration);
        yield return new WaitForSeconds(duration);
    }
}
