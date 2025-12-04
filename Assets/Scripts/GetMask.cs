using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GetMask : MonoBehaviour
{
    public GameObject guideUI;
    private void Awake()
    {
        if(guideUI != null) guideUI.SetActive(false);
    }

    private void Start()
    {
        SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalMaskCreationWeepingMan, gameObject.transform.position, 10, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (guideUI != null)
            {
                guideUI.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            StabilityManager.Instance.currentGetMask += 1;

            if (StabilityManager.Instance.currentGetMask >= StabilityManager.Instance.dayGetMaskValue[DaySystem.Instance.GetNowDay() - 1])
            {
                GameManager.Instance.anomalySystem.ClearMission(1);
                StabilityManager.Instance.currentGetMask = 0;
            }

            SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalMaskBreakCeramic, gameObject.transform.position, 10, false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (guideUI != null)
            {
                guideUI.SetActive(false);
            }
        }
    }

}
