using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionGuideTextUI : MonoBehaviour
{ 
    [SerializeField] private TMP_Text[] tooltipTextUIs;

    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowInteractText(string tooltipText)
    {
        canvasGroup.alpha = 1;

        tooltipTextUIs[0].text = tooltipText;
        tooltipTextUIs[1].text = tooltipText;
    }
    public void HideInteractText()
    {
        canvasGroup.alpha = 0;

        tooltipTextUIs[0].text = string.Empty;
        tooltipTextUIs[1].text = string.Empty;
    }
}
