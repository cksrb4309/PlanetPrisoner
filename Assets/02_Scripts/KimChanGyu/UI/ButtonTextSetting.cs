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

    [SerializeField] bool isNextDayControllerButton = false;

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

        if (order == (int)EventType.Enter) changeColor = isNextDayControllerButton? settings.redTextExitColor: settings.textExitColor;

        else if (order == (int)EventType.Exit) changeColor = isNextDayControllerButton ? settings.redTextEnterColor : settings.textEnterColor;

        else if (order == (int)EventType.Down) changeColor = isNextDayControllerButton ? settings.redTextDownColor : settings.textDownColor;

        currCoroutine = StartCoroutine(ColorChangeCoroutine(changeColor));
    }
    IEnumerator ColorChangeCoroutine(Color changeColor)
    {
        Color currColor = text.color;

        float t = 0;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * speed;

            color = Color.Lerp(currColor, changeColor, t);

            text.color = color;

            yield return null;
        }
        currCoroutine = null;
    }
    public void Configure(UISettings settings)
    {
        //this.settings = settings;

        //text = GetComponent<TMP_Text>();

        //text.color = isNextDayControllerButton ? settings.redTextExitColor : settings.textExitColor;

        //if (isNextDayControllerButton)
        //{
        //    text.color = settings.textExitColor;
        //    text.fontSize = settings.buttonTextSize;
        //}
        //else
        //{
        //    text.color = settings.textExitColor;
        //    text.fontSize = settings.buttonTextSize;
        //}



        //fadeDuration = settings.textColorFadeDuration;
        //speed = 1f / fadeDuration;

        //UnityEditor.EditorUtility.SetDirty(this);
    }
}
