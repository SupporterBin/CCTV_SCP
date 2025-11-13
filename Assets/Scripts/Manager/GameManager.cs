using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("세팅 해야되는 시스템들")]
    public AnomalySystem anomalySystem;

    [HideInInspector] public bool isGameStop;
    [HideInInspector] public bool isTimeStop;
    [HideInInspector] public bool isDeadWarring;

    [HideInInspector] public int protocolNum;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        protocolNum = Random.Range(1000, 9999);
    }

    /// <summary>
    /// 게임이 멈췄거나 컷씬 애니메이션 재생과 같은 시간 일시정지 상태인가요?
    /// </summary>
    /// <returns></returns>
    public bool AllStopCheck()
    {
        if (isGameStop || isTimeStop) { return true; }
        else return false;
    }
}
