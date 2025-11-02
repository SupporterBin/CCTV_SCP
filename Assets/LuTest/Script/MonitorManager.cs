using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonitorManager : MonoBehaviour
{
    [Header("Left Monitor")]
    public GameObject leftCameraObject;
    public GameObject leftMonitorScreen;
    public Button[] leftButtons;

    [Header("Center Monitor")]
    public GameObject centerCameraObject;
    public GameObject centerMonitorScreen;
    public Button[] centerButtons;

    [Header("Right Monitor")]
    public GameObject rightCameraObject;
    public GameObject rightMonitorScreen;
    public Button[] rightButtons;

    void Start()
    {
        foreach (var button in leftButtons)
        {
            button.onClick.AddListener(() => Anomaly_Solution_Click(leftCameraObject, leftMonitorScreen, leftButtons));
        }

        foreach (var button in centerButtons)
        {
            button.onClick.AddListener(() => Anomaly_Solution_Click(centerCameraObject, centerMonitorScreen, centerButtons));
        }

        foreach (var button in rightButtons)
        {
            button.onClick.AddListener(() => Anomaly_Solution_Click(rightCameraObject, rightMonitorScreen, rightButtons));
        }
    }

    private void Anomaly_Solution_Click(GameObject camera, GameObject screen, Button[] button)
    {
        StartCoroutine(StopMonitorAndButtons(camera, screen, button));
    }

    private IEnumerator StopMonitorAndButtons(GameObject camera, GameObject screen, Button[] button)
    {
        camera.SetActive(false);
        screen.SetActive(false);
        foreach (Button bts in button)
        {
            bts.interactable = false;
        }


        yield return new WaitForSeconds(2f);

        camera.SetActive(true);
        screen.SetActive(true);
        foreach (Button bts in button)
        {
            bts.interactable = true;
        }
    }
}
