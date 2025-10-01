using UnityEngine;

public class StabilityManager : MonoBehaviour
{
    public StabilityGage[] stabilityGages;

    public void StabilityUpdate()
    {
        for (int i = 0; i < stabilityGages.Length; i++)
        {
            stabilityGages[i].StabilizationDown(0.1f);
        }
    }
}
