using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionGuideTextUI : MonoBehaviour
{ 
    [SerializeField] private TMP_Text tooltipTextUI;
    public void ShowInteractText(string tooltipText)
    {
        tooltipTextUI.gameObject.SetActive(true);

        tooltipTextUI.text = tooltipText;
    }
    public void HideInteractText()
    {
        tooltipTextUI.gameObject.SetActive(false);
    }
}
