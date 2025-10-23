using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    [SerializeField]
    private CCTVManager cctvManager;
    [SerializeField]
    private TestManager testManager;

    private void Update()
    {
        if (cctvManager.isOnCCTV || testManager.isOnTablet)
        {
            // CCTV나 태블릿이 켜졌을 때
            Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            Cursor.visible = true;                  // 커서 보이기
        }
        else
        {
            // 평상시 (플레이 중)
            Cursor.lockState = CursorLockMode.Locked; // 커서 중앙에 잠금
            Cursor.visible = false;                 // 커서 숨기기
        }
    }
}
