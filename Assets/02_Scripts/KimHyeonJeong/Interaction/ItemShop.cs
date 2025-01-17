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

    public void OnClickedXbutton() // 상점 닫기
    {
        shopUI.SetActive(false);
        inGameUI.SetActive(true);
        player.canMove = true;
    }

    public void OnClickedFlashlight() // 손전등 아이템 클릭
    {
        // 게임매니저에서 산소 0.3L(300ml)감소
        gameManager.ChangeOxygen(-300f);
        // TODO: 캐릭터 인벤토리에 손전등 추가
    }

    public void FillOxygenTank()
    {
        oxygenTankForeground.fillAmount = gameManager.GetOxygen() / 2000f;
    }
}
