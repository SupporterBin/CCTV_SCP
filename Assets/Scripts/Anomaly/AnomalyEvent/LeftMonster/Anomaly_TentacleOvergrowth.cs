    using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/LeftRoom/Anomaly_TentacleOvergrowth")]
public class Anomaly_TentacleOvergrowth : BasicEventAnomaly
{
    private GameObject food;
    public bool isAnomalyStart = false;
    public override EventType Execute()
    {
        Transform root = GameManager.Instance.anomalySystem.specialObjects[0].transform;

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
    }

    public override void Fail()
    {
        isAnomalyStart = false;
        food.SetActive(true);
    }
}
