using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private TMP_Text tooltipTextUI;
    public void OnShowInteractText(string tooltipText)
    {
        tooltipTextUI.gameObject.SetActive(true);

        tooltipTextUI.text = tooltipText;
    }
    public void OnHideInteractText()
    {
        tooltipTextUI.gameObject.SetActive(false);
    }
}
