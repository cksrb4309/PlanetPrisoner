using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "UISettings", menuName = "UI/UISettings")]
public class UISettings : ScriptableObject
{
    [Header("Images")]
    public Sprite normalButtonSprite;
    public Sprite highlightedButtonSprite;
    public Sprite pressedButtonSprite;

    [Header("Colors")]
    public Color imageExitColor;
    public Color imageEnterColor;
    public Color imageDownColor;

    public Color redImageExitColor;
    public Color redImageEnterColor;
    public Color redImageDownColor;

    public Color textExitColor;
    public Color textEnterColor;
    public Color textDownColor;

    public Color redTextExitColor;
    public Color redTextEnterColor;
    public Color redTextDownColor;

    [Header("Audio Clips")]
    public AudioClip buttonEnterClip;
    public AudioClip buttonDownClip;

    [Header("Text Size")]
    public float buttonTextSize;
    public float itemNameTextSize;
    public float itemExplainTextSize;

    [Header("Text Font")]
    public TMP_FontAsset fontAsset;

    [Header("Fade Duration")]
    public float textColorFadeDuration;
    public float imageColorFadeDuration;

    public void Execute()
    {
        IConfigurable[] configurables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IConfigurable>().ToArray();

        foreach (IConfigurable configurable in configurables)
        {
            configurable.Configure(this);
        }
    }
}
