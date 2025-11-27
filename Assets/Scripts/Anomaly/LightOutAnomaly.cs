using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Anomalies/Event/Special/LightOutAnomaly")]
public class LightOutAnomaly : BasicEventAnomaly
{
    [Header("끌 빛들, Light"), SerializeField]
    private Light[] lights;
    private float[] lights_Intensity;

    public override void Clear()
    {
        Clean();
    }

    public override EventType Execute()
    {
        lights_Intensity = new float[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            lights_Intensity[i] = lights[i].intensity;
            lights[i].intensity = 0;
        }

        return eventType;
    }

    public override void Fail()
    {
        Clean();
    }

    private void Clean()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = lights_Intensity[i];
        }
    }
}
