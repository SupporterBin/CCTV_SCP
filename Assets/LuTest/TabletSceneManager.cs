using UnityEngine;
using UnityEngine.UI;

public class TabletSceneManager : MonoBehaviour
{
    // 스타트 버튼 3개
    [SerializeField]
    private Button[] startBts;

    // 패널 3개
    [SerializeField]
    private GameObject[] tabletPanels;

    private bool isPlaying = false;
    private void Awake()
    {
        for (int i = 0; i < startBts.Length; i++)
        {
            int panelBtIndex = i;
            if (startBts[panelBtIndex] != null)
            {
                startBts[panelBtIndex].onClick.AddListener(() => OnStartButton(panelBtIndex));
            }
        }

        tabletPanels[0].transform.Find("RightButton").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(tabletPanels[1]));
        tabletPanels[1].transform.Find("LeftButton").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(tabletPanels[0]));
        tabletPanels[1].transform.Find("RightButton").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(tabletPanels[2]));
        tabletPanels[2].transform.Find("LeftButton").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(tabletPanels[1]));
    }
    private void Start()
    {
        tabletPanels[0].SetActive(false);
        tabletPanels[1].SetActive(true);
        tabletPanels[2].SetActive(false);
    }

    private void SwitchPanel(GameObject switchPanel)
    {
        // 3개 끄고 파라미터로 받는 패널 켜기
        foreach (GameObject panel in tabletPanels)
        {
            panel.SetActive(false);
        }
        switchPanel.SetActive(true);

    }

    private void OnStartButton(int panelBtIndex)
    {

        GameStart(panelBtIndex);
    }

    private void GameStart(int panelBtIndex)
    {
        isPlaying = true;
        Debug.Log($"isPlayer : {isPlaying} & PanelNumber : {panelBtIndex + 1}");
    }

    // 아직 구현안함
    private void EndStart()
    {
        isPlaying = false;
    }
}
