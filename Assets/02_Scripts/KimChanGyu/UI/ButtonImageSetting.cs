using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ButtonImageSetting : MonoBehaviour, IConfigurable
{
    [SerializeField] UISettings settings;

    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] float speed;

    Color color = Color.white;

    Coroutine currCoroutine = null;

    Image image = null;


    private void Awake()
    {
        image = GetComponent<Image>();

        speed = 1f / fadeDuration;
    }

    public void OnColorChange(int order)
    {
        if (currCoroutine != null) StopCoroutine(currCoroutine);

        Color changeColor = Color.white;
        
        if (order == (int)EventType.Enter) 
        {
            changeColor = settings.imageExitColor;

            image.sprite = settings.normalButtonSprite;
        }

        else if (order == (int)EventType.Exit) 
        {
            changeColor = settings.imageEnterColor;

            image.sprite = settings.highlightedButtonSprite;
        }

        else if (order == (int)EventType.Down) 
        {
            changeColor = settings.imageDownColor;

            image.sprite = settings.pressedButtonSprite;
        }

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
    public void Configure(UISettings settings)
    {
        GetComponent<Image>().color = settings.imageExitColor;
        fadeDuration = settings.imageColorFadeDuration;
        speed = 1f / fadeDuration;
        GetComponent<Image>().sprite = settings.normalButtonSprite;

        this.settings = settings;

        UnityEditor.EditorUtility.SetDirty(GetComponent<Image>());
    }
}

[Serializable]
public enum EventType
{
    Enter,
    Exit,
    Down,
}