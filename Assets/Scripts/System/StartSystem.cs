using System.Collections;
using UnityEngine;

public class StartSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.isGameStop = true;

        StartCoroutine(GameStartTimeline());
    }

    private IEnumerator GameStartTimeline()
    {
        SoundManager.Instance.PlayGlobalSFX(SoundManager.Instance.Data.ingameElevatorArriveDing);

        yield return new WaitForSecondsRealtime(5.5f);

        SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.ingameDoorOpenHydraulic, GameManager.Instance.anomalySystem.specialObjects[3].transform.position, 20, false);
        GameManager.Instance.anomalySystem.specialObjects[3].GetComponent<Animator>().Play("Open");

        yield return new WaitForSecondsRealtime(3f);

        SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.ingameDoorOpenHydraulic, GameManager.Instance.anomalySystem.specialObjects[2].transform.position, 20, false);
        GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Open");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DaySystem.Instance.GetNowDay() == 1) return;
        if (GameManager.Instance.isGameStart) return;

        GameManager.Instance.anomalySystem.specialObjects[3].GetComponent<Animator>().Play("Close");
        GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Close");

        SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.ingameDoorCloseHydraulic, GameManager.Instance.anomalySystem.specialObjects[3].transform.position, 20, false);
        SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.ingameDoorCloseHydraulic, GameManager.Instance.anomalySystem.specialObjects[2].transform.position, 20, false);
        GameManager.Instance.isGameStop = false;
        GameManager.Instance.isGameStart = true;
    }
}
