using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "UISettings", menuName = "UI/UISettings")]
public class UISettings : ScriptableObject
{
    [Header("Images")]
    public Sprite buttonSprite;

    [Header("Colors")]
    public Color imageExitColor;
    public Color imageEnterColor;
    public Color imageDownColor;

    public Color textExitColor;
    public Color textEnterColor;
    public Color textDownColor;

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
