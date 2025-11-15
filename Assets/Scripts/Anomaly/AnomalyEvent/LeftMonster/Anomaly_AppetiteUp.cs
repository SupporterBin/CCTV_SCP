using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/LeftRoom/Anomaly_AppetiteUp")]
public class Anomaly_AppetiteUp : BasicEventAnomaly
{
    private Animator anim;

    public override EventType Execute()
    {
        anim = GameManager.Instance.anomalySystem.monsters[0].GetComponent<Animator>();

        anim.Play("Skill1");
        return eventType;
    }

    public override void Clear()
    {
        anim.Play("Idle");
    }

    public override void Fail()
    {

        anim.Play("Idle");
    }
}
