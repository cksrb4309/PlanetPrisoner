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

    [SerializeField] bool isNextDayControllerButton = false;

    Color color = Color.white;

    Coroutine currCoroutine = null;

    Image image = null;

    AudioSource audioSource = null;

    private void Awake()
    {
        image = GetComponent<Image>();

        image.color = isNextDayControllerButton ? settings.redImageExitColor : settings.imageExitColor;

        speed = 1f / fadeDuration;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnColorChange(int order)
    {
        if (currCoroutine != null) StopCoroutine(currCoroutine);

        Color changeColor = Color.white;
        
        if (order == (int)EventType.Enter) 
        {
            audioSource.PlayOneShot(settings.buttonEnterClip);

            changeColor = isNextDayControllerButton ? settings.redImageExitColor: settings.imageExitColor;

            image.sprite = settings.normalButtonSprite;
        }

        else if (order == (int)EventType.Exit) 
        {
            changeColor = isNextDayControllerButton ? settings.redImageEnterColor : settings.imageEnterColor;

            image.sprite = settings.highlightedButtonSprite;
        }

        else if (order == (int)EventType.Down)
        {
            audioSource.PlayOneShot(settings.buttonDownClip);

            changeColor = isNextDayControllerButton ? settings.redImageDownColor : settings.imageDownColor;

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
            t += Time.unscaledDeltaTime * speed;

            color = Color.Lerp(currColor, changeColor, t);

            image.color = color;

            yield return null;
        }
        currCoroutine = null;
    }
    public void Configure(UISettings settings)
    {
        //Image editImage = GetComponent<Image>();

        //if (isNextDayControllerButton)
        //    editImage.color = settings.imageExitColor;
        
        //else
        //    editImage.color = settings.imageExitColor;
        
        //editImage.sprite = settings.normalButtonSprite;

        //fadeDuration = settings.imageColorFadeDuration;
        //speed = 1f / fadeDuration;

        //this.settings = settings;

        //UnityEditor.EditorUtility.SetDirty(editImage);
        //UnityEditor.EditorUtility.SetDirty(this);
    }
}

[Serializable]
public enum EventType
{
    Enter,
    Exit,
    Down,
}