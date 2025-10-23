using System;
using System.Collections;
using UnityEngine;

public class TentacleAnomalyController : MonoBehaviour
{
    [SerializeField]
    private TentacleWallhug tentacle1;
    [SerializeField]
    private TentacleWallhug tentacle2;
    [SerializeField]
    Material tentacleMat;
    // -0.3Âë µÇ¸é ´Ù »ç¶óÁü 1ÀÌ¸é ¸ÖÂÄÇÑ »óÅÂ
    private float burnValue = 1f;
    private float MinBurnValue = -0.5f;
    public void TentacleGrow()
    {
        tentacle1.gameObject.SetActive(true);
        tentacle2.gameObject.SetActive(true);
    }
    public void TentacleFadeAway()
    {
        tentacle1.FadeAwayTentacle();
        tentacle2.FadeAwayTentacle();
    }
    public void TentacleBurn()
    {
        if (tentacleMat == null) throw new NullReferenceException();
        StartCoroutine("ITentacleBurn");
    }
    private IEnumerator ITentacleBurn()
    {
        burnValue = 1f;
        while(burnValue > MinBurnValue)
        {
            burnValue -= Time.deltaTime;
            tentacleMat.SetFloat("_BurnValue", burnValue);
            yield return null;
        }
        tentacle1.gameObject.SetActive(false);
        tentacle2.gameObject.SetActive(false);
        burnValue = 1f;
        tentacleMat.SetFloat("_BurnValue", burnValue);
    }
}
