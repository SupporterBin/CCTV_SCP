using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Actions/Spawn VFX")]
public class SpawnVFXAction : BaseAnomalyActionSO
{
    public GameObject vfxPrefab;
    public Vector3 offset;
    public float lifeTime = 2f;

    public override bool Validate(out string error)
    {
        if (!vfxPrefab) { error = "vfxPrefab is null"; return false; }
        if (lifeTime < 0f) { error = "lifeTime must be >= 0"; return false; }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        var pos = ctx.Origin + offset;
        var go = Object.Instantiate(vfxPrefab, pos, Quaternion.identity);
        Log($"Spawn VFX at {pos}");
        if (lifeTime > 0f)
        {
            yield return new WaitForSeconds(lifeTime);
            if (go) Object.Destroy(go);
        }
    }
}
