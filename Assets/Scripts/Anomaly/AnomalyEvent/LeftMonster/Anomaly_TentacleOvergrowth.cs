    using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/LeftRoom/Anomaly_TentacleOvergrowth")]
public class Anomaly_TentacleOvergrowth : BasicEventAnomaly
{
    private GameObject food;
    public bool isAnomalyStart = false;

    private AudioSource saveSound;

    public override EventType Execute()
    {
        Transform root = GameManager.Instance.anomalySystem.specialObjects[0].transform;

        saveSound = SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalTentacleAggressiveWindowHit,
           GameManager.Instance.anomalySystem.monsters[1].transform.position, 20, false);

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Target"))
            {
                food = child.gameObject;
                break;
            }
        }

        isAnomalyStart = true;
        food.SetActive(false);

        return eventType;
    }

    public override void Clear()
    {
        isAnomalyStart = false;
        food.SetActive(true);

        SoundManager.Instance.StopSFX(saveSound);
    }

    public override void Fail()
    {
        isAnomalyStart = false;
        food.SetActive(true);

        SoundManager.Instance.StopSFX(saveSound);
    }
}
