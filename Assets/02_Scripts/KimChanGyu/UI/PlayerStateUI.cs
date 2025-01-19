using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    static PlayerStateUI instance = null;
    public static PlayerStateUI Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] Image oxygenFillImage;
    [SerializeField] Image spaceSuitFillImage;

    private void Awake()
    {
        instance = this;
    }
    public void SetOxygenFillImage(float fillAmount)
    {
        oxygenFillImage.fillAmount = fillAmount;
    }
    public void SetSuitSpaceFillImage(float fillAmount)
    {
        spaceSuitFillImage.fillAmount = fillAmount;
    }
}
