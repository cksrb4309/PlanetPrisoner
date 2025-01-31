using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ButtonTextSetting : MonoBehaviour, IConfigurable
{
    [SerializeField] UISettings settings;

    float fadeDuration = 0.2f;

    Coroutine currCoroutine = null;

    [SerializeField] TMP_Text text = null;

    Color color;

    float speed = 0;

    private void Awake()
    {
        speed = 1f / fadeDuration;
    }

    public void OnColorChange(int order)
    {
        if (currCoroutine != null) StopCoroutine(currCoroutine);

        Color changeColor = Color.white;

        if (order == 0) changeColor = settings.textExitColor;
        else if (order == 1) changeColor = settings.textEnterColor;
        else if (order == 2) changeColor = settings.textDownColor;

        currCoroutine = StartCoroutine(ColorChangeCoroutine(changeColor));
    }
    IEnumerator ColorChangeCoroutine(Color changeColor)
    {
        Color currColor = text.color;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            color = Color.Lerp(currColor, changeColor, t);

            text.color = color;

            yield return null;
        }
        currCoroutine = null;
    }
    public void Configure(UISettings settings)
    {
        this.settings = settings;
        text = GetComponent<TMP_Text>();
        text.color = settings.textExitColor;
        text.fontSize = settings.buttonTextSize;

        fadeDuration = settings.textColorFadeDuration;
        speed = 1f / fadeDuration;

        UnityEditor.EditorUtility.SetDirty(this);
    }
}
