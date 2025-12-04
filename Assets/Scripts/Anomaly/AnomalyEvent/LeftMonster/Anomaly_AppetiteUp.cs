using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/LeftRoom/Anomaly_AppetiteUp")]
public class Anomaly_AppetiteUp : BasicEventAnomaly
{
    private Animator anim;
    private AudioSource saveSound;

    public override EventType Execute()
    {
        anim = GameManager.Instance.anomalySystem.monsters[0].GetComponent<Animator>();

        saveSound = SoundManager.Instance.Play3DSFX(SoundManager.Instance.Data.abnormalTentacleAggressiveWindowHit,
            GameManager.Instance.anomalySystem.monsters[1].transform.position, 20, false);

        anim.Play("Skill1");
        return eventType;
    }

    public override void Clear()
    {
        anim.Play("Idle");
        SoundManager.Instance.StopSFX(saveSound);
    }

    public override void Fail()
    {
        anim.Play("Idle");
        SoundManager.Instance.StopSFX(saveSound);
    }
}
