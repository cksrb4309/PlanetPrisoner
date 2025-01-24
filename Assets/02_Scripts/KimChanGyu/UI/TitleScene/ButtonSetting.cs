using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonSetting : MonoBehaviour
{
    [SerializeField] UISettings settings;

    float fadeDuration = 0.2f;

    Color color = Color.white;

    Coroutine currCoroutine = null;

    Image image;

    float speed;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = settings.imageExitColor;
        fadeDuration = settings.imageColorFadeDuration;
        speed = 1f / fadeDuration;
        image.sprite = settings.buttonSprite;
    }
    private void OnEnable()
    {
        image = GetComponent<Image>();
        image.color = settings.imageExitColor;
        fadeDuration = settings.imageColorFadeDuration;
        speed = 1f / fadeDuration;
    }
    public void OnColorChange(int order)
    {
        if (currCoroutine != null) StopCoroutine(currCoroutine);

        Color changeColor = Color.white;

        if (order == 0) changeColor = settings.imageExitColor;
        else if (order == 1) changeColor = settings.imageEnterColor;
        else if (order == 2) changeColor = settings.imageDownColor;

        currCoroutine = StartCoroutine(ColorChangeCoroutine(changeColor));
    }
    IEnumerator ColorChangeCoroutine(Color changeColor)
    {
        Color currColor = image.color;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            color = Color.Lerp(currColor, changeColor, t);

            image.color = color;

            yield return null;
        }
        currCoroutine = null;
    }
    private void OnValidate()
    {
        GetComponent<Image>().color = settings.imageExitColor;
        fadeDuration = settings.imageColorFadeDuration;
        GetComponent<Image>().sprite = settings.buttonSprite;
    }
}
