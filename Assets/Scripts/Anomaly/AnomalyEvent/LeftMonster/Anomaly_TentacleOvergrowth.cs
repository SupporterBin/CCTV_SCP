    using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/LeftRoom/Anomaly_TentacleOvergrowth")]
public class Anomaly_TentacleOvergrowth : BasicEventAnomaly
{
    private GameObject food;

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

        food.SetActive(false);

        return eventType;
    }

    public override void Clear()
    {
        food.SetActive(true);
        throw new System.NotImplementedException();
    }

    public override void Fail()
    {
        food.SetActive(true);
        throw new System.NotImplementedException();
    }
}
