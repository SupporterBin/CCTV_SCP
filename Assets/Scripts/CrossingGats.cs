using System.Collections;
using UnityEngine;

public class CrossingGats : MonoBehaviour
{
    [SerializeField, Header("텍스트 가이드")]
    private GameObject guide;
    private bool isClick = false;
    public bool isCrossingGateShutDown => gameObject.GetComponent<Animator>().GetBool("isShotDown");
    [SerializeField] LightOutAnomaly lightOutAnomaly;

    private void Awake()
    {
        lightOutAnomaly.SetCrossingGats(gameObject);
    }

   public void TryActionEvent()
    {
        if (GameManager.Instance.isGameStop == true)
            return;
        if (isClick == true) return;
        StartCoroutine(ActionEvent());
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
        gameObject.GetComponent<Animator>().SetBool("isShotDown", false);

        yield return new WaitForSecondsRealtime(1.0f);

        isClick = false;
    }
}
