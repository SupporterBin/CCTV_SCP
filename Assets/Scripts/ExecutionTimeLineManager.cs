using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ExecutionTimeLineManager : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;
    public static ExecutionTimeLineManager instance {get; private set;}
    public TimelineAsset[] executionTimelineAssets = new TimelineAsset[1];

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        playableDirector = GetComponent<PlayableDirector>();
    }

    public void PlayExecutionTimeline(int index)
    {
        if(executionTimelineAssets.Length > index)
            playableDirector.Play(executionTimelineAssets[index]);
    }
}
