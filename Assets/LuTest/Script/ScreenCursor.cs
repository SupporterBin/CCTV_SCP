using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    [SerializeField]
    private CCTVManager cctvManager;
    [SerializeField]
    private TabletManager testManager;
    [SerializeField]
    private ManualManager manualManager;
    private void Update()
    {
        if (cctvManager.isOnCCTV || testManager.isOnTablet || manualManager.isOnManual || GameManager.Instance.IsOptionMode)
        {
            // CCTV�� �º���� ������ ��
            Cursor.lockState = CursorLockMode.Confined; // Ŀ�� ��� ����
            Cursor.visible = true;                  // Ŀ�� ���̱�
        }
        else
        {
            // ���� (�÷��� ��)
            Cursor.lockState = CursorLockMode.Locked; // Ŀ�� �߾ӿ� ���
            Cursor.visible = false;                 // Ŀ�� �����
        }
    }
}
