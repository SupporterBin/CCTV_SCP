using System.Collections;
using UnityEngine;

public class StartSystem : MonoBehaviour
{
    [HideInInspector]
    public bool isGameStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.isGameStop = true;

        StartCoroutine(GameStartTimeline());
    }

    private IEnumerator GameStartTimeline()
    {
        yield return new WaitForSecondsRealtime(5.5f);

        GameManager.Instance.anomalySystem.specialObjects[3].GetComponent<Animator>().Play("Open");

        yield return new WaitForSecondsRealtime(3f);

        GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Open");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameStart) return;

        GameManager.Instance.anomalySystem.specialObjects[3].GetComponent<Animator>().Play("Close");
        GameManager.Instance.anomalySystem.specialObjects[2].GetComponent<Animator>().Play("Close");

        GameManager.Instance.isGameStop = false;
        isGameStart = true;
    }
}
