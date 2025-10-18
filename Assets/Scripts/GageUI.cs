using UnityEngine;
using UnityEngine.UI;
public class GageUI : MonoBehaviour
{
    public Slider slider;
    
    // Update is called once per frame
    void Update()
    {
        slider.value = StabilityManager.Instance.CurrentStability[0] / StabilityManager.maxStability;
    }
}
