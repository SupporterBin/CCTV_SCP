using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Anomalies/Actions/Special/Play Timeline")]
public class PlayTimelineAction : BaseAnomalyActionSO
{
    public PlayableAsset timeline;   // 재생할 타임라인
    public float playSpeed = 1f;

    public override bool Validate(out string error)
    {
        if (!timeline) { error = "timeline is null"; return false; }
        error = null; return true;
    }

    public override IEnumerator Execute(AnomalyContext ctx)
    {
        // 데모: 임시 Director를 만들어 재생(실전은 풀/사전 바인딩 권장)
        var go = new GameObject("__TempTimeline");
        var director = go.AddComponent<PlayableDirector>();
        director.playableAsset = timeline;
        director.playableGraph.GetRootPlayable(0).SetSpeed(playSpeed);
        director.Play();

        while (director.state == PlayState.Playing)
            yield return null;

        Object.Destroy(go);
    }
}
