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
            Debug.Log("발동");
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
