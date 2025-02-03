using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonImageSetting : MonoBehaviour, IConfigurable
{
    public UISettings settings;

    float fadeDuration = 0.2f;

    Color color = Color.white;

    Coroutine currCoroutine = null;

    Image image = null;

    float speed;

    private void Awake()
    {
        image = GetComponent<Image>();

        speed = 1f / fadeDuration;
    }

    public void OnColorChange(EventType order)
    {
        if (currCoroutine != null) StopCoroutine(currCoroutine);

        Color changeColor = Color.white;
        
        if (order == EventType.Exit) 
        {
            changeColor = settings.imageExitColor;

            image.sprite = settings.normalButtonSprite;
        }

        else if (order == EventType.Enter) 
        {
            changeColor = settings.imageEnterColor;

            image.sprite = settings.highlightedButtonSprite;
        }

        else if (order == EventType.Down) 
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
        GetComponent<Image>().sprite = settings.normalButtonSprite;

        this.settings = settings;

        UnityEditor.EditorUtility.SetDirty(this);
    }
}
public enum EventType
{
    Enter,
    Exit,
    Down,
}