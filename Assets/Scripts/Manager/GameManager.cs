using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("세팅 해야되는 시스템들")]
    public DaySystem daySystem;
    public AnomalySystem anomalySystem;

    [HideInInspector] public bool isGameStop;
    [HideInInspector] public bool isTimeStop;


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

    /// <summary>
    /// 게임이 멈췄거나 애니메이션과 같은 일시정지 상태인가요?
    /// </summary>
    /// <returns></returns>
    public bool AllStopCheck()
    {
        if (isGameStop || isTimeStop) { return true; }
        else return false;
    }
}
