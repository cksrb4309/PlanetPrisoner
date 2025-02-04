using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    static PlayerStateUI instance = null;
    public static PlayerStateUI Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] Image oxygenFillImage;
    [SerializeField] Image spaceSuitFillImage;
    [SerializeField] Image spaceSuitFaceImage;

    [SerializeField] Sprite[] faceSprites;

    [SerializeField] CanvasGroup spaceSuitCanvasGroup;

    [SerializeField] Color spaceSuitStartColor;
    [SerializeField] Color spaceSuitEndColor;

    Coroutine spaceSuitFadeInOutCoroutine = null;

    private void Awake()
    {
        instance = this;

        spaceSuitCanvasGroup.alpha = 0.3f;

        spaceSuitFaceImage.sprite = faceSprites[0];

        spaceSuitFillImage.color = spaceSuitStartColor;
    }
    public void SetOxygenFillImage(float fillAmount)
    {
        oxygenFillImage.fillAmount = fillAmount;
    }
    public void SetSuitSpaceFillImage(float fillAmount)
    {
        spaceSuitFillImage.color = Color.Lerp(spaceSuitEndColor, spaceSuitStartColor, fillAmount);

        spaceSuitFaceImage.sprite = fillAmount > 0.66f ? faceSprites[0] : fillAmount > 0.33f ? faceSprites[1] : faceSprites[2];

        if (spaceSuitFadeInOutCoroutine != null) StopCoroutine(spaceSuitFadeInOutCoroutine);

        spaceSuitFadeInOutCoroutine = StartCoroutine(SpaceSuitFadeInOutCoroutine());
    }
    IEnumerator SpaceSuitFadeInOutCoroutine()
    {
        float alpha = spaceSuitCanvasGroup.alpha;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 3f;

            spaceSuitCanvasGroup.alpha = alpha;

            yield return null;
        }
        alpha = 1f;

        spaceSuitCanvasGroup.alpha = alpha;

        yield return new WaitForSeconds(1.5f);

        while (alpha > 0.5f)
        {
            alpha -= Time.deltaTime * 3f;

            spaceSuitCanvasGroup.alpha = alpha;

            yield return null;
        }

        spaceSuitCanvasGroup.alpha = 0.5f;
    }
}
