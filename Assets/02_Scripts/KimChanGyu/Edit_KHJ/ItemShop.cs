using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] OxygenTank oxygenTank;
    [SerializeField] Image oxygenTankForeground;

    public void OnClickedXbutton() // 상점 닫기 클릭
    {
        shopUI.SetActive(false);
        inGameUI.SetActive(true);

        PlayerItemHandler.Instance.GetComponent<PlayerController>().EnableMovement();
        CursorController.DisableCursor();
    }

    public void OnClickedFlashlight() // 손전등 아이템 클릭
    {
        // 구매가 가능할 경우 해당 값을 치루고 true를 반환하는 함수를 통해
        // 구매 시의 아이템 전송을 if 안에 구현
        if (oxygenTank.PurchaseItem(300f))
        {
            FillOxygenTank(); // 산소 이미지 업데이트

            // TODO: 전송기에서 아이템 뱉어내기
        }
    }
 
    public void FillOxygenTank()
    {
        // 산소량을 Tank이미지에 표시
        oxygenTankForeground.fillAmount = oxygenTank.GetOxygen() / 2000f;
    }
}
