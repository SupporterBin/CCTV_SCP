using System.Collections;
using UnityEngine;

public class CrossingGats : MonoBehaviour
{
    [SerializeField, Header("텍스트 가이드")]
    private GameObject guide;
    private bool isClick = false;

    [SerializeField] LightOutAnomaly lightOutAnomaly;

    private void Awake()
    {
        lightOutAnomaly.SetCrossingGats(gameObject);
    }

    private void OnMouseOver()
    {
        guide.SetActive(true);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isClick) return;
            StartCoroutine(ActionEvent());
        }
    }

    private void OnMouseExit()
    {
        guide.SetActive(false);
    }


    private IEnumerator ActionEvent()
    {
        isClick = true;

        gameObject.GetComponent<Animator>().SetBool("isAction", true);

        yield return new WaitForSecondsRealtime(1.0f);

        gameObject.GetComponent<Animator>().SetBool("isShotDown", true);
        GameManager.Instance.anomalySystem.ClearMission(2);
        
        yield return new WaitForSecondsRealtime(1.0f);

        gameObject.GetComponent<Animator>().SetBool("isAction", false);

        yield return new WaitForSecondsRealtime(1.0f);

        isClick = false;
    }
}
