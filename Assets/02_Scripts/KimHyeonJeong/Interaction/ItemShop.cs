using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] PlayerController player;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image oxygenTankForeground;

    void Start()
    {
        
    }

    
    void Update()
    {
        FillOxygenTank();
    }

    public void OnClickedXbutton() // 상점 닫기 클릭
    {
        shopUI.SetActive(false);
        inGameUI.SetActive(true);
        player.canMove = true;
    }

    public void OnClickedFlashlight() // 손전등 아이템 클릭
    {
        // 게임매니저에서 산소 0.3L(300ml)감소
        gameManager.ChangeOxygen(-300f);

        // TODO: 전송기에서 아이템 뱉어내기
    }

    public void FillOxygenTank()
    {
        // 산소량을 Tank이미지에 표시
        oxygenTankForeground.fillAmount = gameManager.GetOxygen() / 2000f;
    }
}
