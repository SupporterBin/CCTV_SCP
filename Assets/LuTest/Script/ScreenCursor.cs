using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    [SerializeField]
    private CCTVManager cctvManager;
    [SerializeField]
    private TabletManager testManager;

    private void Update()
    {
        if (cctvManager.isOnCCTV || testManager.isOnTablet || GameManager.Instance.IsOptionMode)
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
