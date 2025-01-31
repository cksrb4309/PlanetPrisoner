using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectController : MonoBehaviour
{
    static ScreenEffectController instance = null;

    [SerializeField] Image noiseImage;

    Coroutine noiseCoroutine = null;
    private void Awake()
    {
        instance = this;
    }
    public static void TriggerScreenNoiseEffect(float noiseSpeed = 1f, float noiseAlpha = 1f, float duration = 1f)
    {
        instance.ScreenNoiseEffect(noiseSpeed, noiseAlpha, duration);
    }
    void ScreenNoiseEffect(float noiseSpeed, float noiseAlpha, float duration)
    {
        if (noiseCoroutine != null) StopCoroutine(noiseCoroutine);

        noiseCoroutine = StartCoroutine(NoiseCoroutine(noiseSpeed, noiseAlpha, duration));
    }
    IEnumerator NoiseCoroutine(float noiseSpeed, float noiseAlpha, float duration)
    {
        float alpha = noiseImage.color.a;

        Color color = Color.white;

        color.a = alpha;

        noiseImage.color = color;

        while (alpha < noiseAlpha)
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * noiseSpeed);

            color.a = alpha;

            noiseImage.color = color;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(duration);

        while (alpha > 0f)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * noiseSpeed);

            color.a = alpha;

            noiseImage.color = color;

            yield return null;
        }
    }
}
