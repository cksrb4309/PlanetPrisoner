using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenEffectController : MonoBehaviour
{
    static ScreenEffectController instance = null;

    [SerializeField] Volume volume;

    [SerializeField] Image noiseImage;

    Coroutine noiseCoroutine = null;
    Coroutine grayscaleCoroutine = null;

    ColorAdjustments colorAdjustments;

    float currentSaturation = 0;
    private void Awake()
    {
        instance = this;

        if (volume.profile.TryGet(out colorAdjustments))
        {
            currentSaturation = 0;

            colorAdjustments.saturation.Override(currentSaturation);
        }
        else
        {
            Debug.LogWarning("크아아아아악 이게 오류나 !!!!!");
        }
    }
    #region 화면 노이즈 효과
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
    #endregion

    #region 화면 흑백 효과
    public static void TriggerScreenGrayscaleEffect(float saturation)
    {
        instance.ScreenGrayscaleEffect(saturation);
    }
    void ScreenGrayscaleEffect(float saturation)
    {
        if (currentSaturation == saturation) return;

        if (grayscaleCoroutine != null) StopCoroutine(grayscaleCoroutine);

        grayscaleCoroutine = StartCoroutine(GrayscaleCoroutine(saturation));
    }
    IEnumerator GrayscaleCoroutine(float saturation)
    {
        float startSaturation = currentSaturation;
        float endSaturation = saturation;

        saturation = 0f;

        while (saturation < 1f)
        {
            saturation += Time.unscaledDeltaTime * 2f;

            if (saturation > 1f) saturation = 1f;

            currentSaturation = Mathf.Lerp(startSaturation, endSaturation, saturation);

            colorAdjustments.saturation.Override(currentSaturation);

            yield return null;
        }
    }
    #endregion
}
