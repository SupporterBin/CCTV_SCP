using UnityEngine;
using UnityEngine.Rendering;



public class LightEnvironmentController : MonoBehaviour
{
    public float baeTime = 5f;
    public float targetDensity = 0.5f;
    public Color targetColor = Color.black;
    float awakeDensity = 0.5f;
    Color awakeColor = Color.black;
    public void FadeFogDensityAndColor(float seconds, float target, Color color)
    {
        StartCoroutine(FadeDensityCo(target, seconds));
        StartCoroutine(FadeColorCo(color, seconds));
    }
    public void FadeFogDensityAndColor()
    {
        StartCoroutine(FadeDensityCo(targetDensity, baeTime));
        StartCoroutine(FadeColorCo(targetColor, baeTime));
    }
    private void Awake()
    {
        awakeColor = targetColor;
        awakeDensity = targetDensity;
    }
    public void ResetOriginal()
    {
        RenderSettings.fogDensity = awakeDensity;
        RenderSettings.fogColor = awakeColor;
    }
    System.Collections.IEnumerator FadeDensityCo(float target, float t)
    {
        float start = RenderSettings.fogDensity;
        float time = 0f;
        RenderSettings.fog = true;
        //RenderSettings.fogMode = FogMode.Exponential;
        while (time < t)
        {
            time += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(start, target, time / t);
            yield return null;
        }
        RenderSettings.fogDensity = target;
    }
    System.Collections.IEnumerator FadeColorCo(Color target, float t)
    {
        Color start = RenderSettings.fogColor;
        float time = 0f;
        RenderSettings.fog = true;
        //RenderSettings.fogMode = FogMode.Exponential;
        while (time < t)
        {
            time += Time.deltaTime;
            RenderSettings.fogColor = Color.Lerp(start, target, time / t);
            yield return null;
        }
        RenderSettings.fogColor = target;
    }
}
