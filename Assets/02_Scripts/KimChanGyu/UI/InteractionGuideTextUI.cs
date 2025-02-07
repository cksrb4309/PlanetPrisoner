using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionGuideTextUI : MonoBehaviour
{ 
    [SerializeField] private TMP_Text[] tooltipTextUIs;
    [SerializeField] private Image crossHairImage;

    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowInteractText(string tooltipText)
    {
        Color color = new Color(0, 0, 0, 0);
        crossHairImage.color = color;

        canvasGroup.alpha = 1;

        tooltipTextUIs[0].text = tooltipText;
        tooltipTextUIs[1].text = tooltipText;
    }
    public void HideInteractText()
    {
        crossHairImage.color = Color.white;

        canvasGroup.alpha = 0;

        tooltipTextUIs[0].text = string.Empty;
        tooltipTextUIs[1].text = string.Empty;
    }
}
