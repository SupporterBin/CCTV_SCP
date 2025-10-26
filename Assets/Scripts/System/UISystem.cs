using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [Header("버티기 게이지")]
    public Slider[] anomalyGageBar;

    [Header("버튼 열어")]
    public GameObject[] CCTV_SubButtonSet;

    private void Update()
    {
        //나머지는 나중에 추가 1,2는
        anomalyGageBar[0].value = StabilityManager.Instance.CurrentStability[0] / 100;
    }

    public void SubButtonOpen(int index)
    {
        if (CCTV_SubButtonSet[index].activeSelf)
            CCTV_SubButtonSet[index].SetActive(false);
        else
            CCTV_SubButtonSet[index].SetActive(true);
    }
}
