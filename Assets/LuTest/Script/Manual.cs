using UnityEngine;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    // manual number
    public int manualIndex;

    private Button manualButton;

    [SerializeField]
    private ManualManager manualManager;

    private void Awake()
    {
        manualButton = GetComponent<Button>();

        manualButton.onClick.AddListener(() => OnClickManualButton());
    }
    private void Start()
    {
        // 만약 인스펙터에서 깜빡하고 연결 안 했을 경우, 자동으로 찾기 (안전장치)
        if (manualManager == null)
        {
            manualManager = FindAnyObjectByType<ManualManager>();
        }
    }

    public void OnClickManualButton()
    {

        Debug.Log($" Number {manualIndex + 1} Click");

        if (manualManager != null)
        {
            manualManager.MoveToDetailPanel(manualIndex);
        }
        else
        {
            Debug.LogError("mia");
        }

    }
}
