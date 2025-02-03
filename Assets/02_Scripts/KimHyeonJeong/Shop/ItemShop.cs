using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] PlayerController player;
    [SerializeField] OxygenTank oxygenTank;
    [SerializeField] Image oxygenTankForeground;
    [SerializeField] Transceiver transceiver;

    [SerializeField] Vector3 itemSpawnPosition;

    [SerializeField] List<GameObject> itemPrefabs = new List<GameObject>();

    void Start()
    {
        itemSpawnPosition = new Vector3(2.1f, 0.8f, -1.3f);
    }

    
    void Update()
    {
        
    }

    public void OnClickedXbutton() // 상점 닫기 클릭
    {
        if (transceiver != null)
        {
            transceiver.TurnOffEmission();
        }
        shopUI.SetActive(false);
        inGameUI.SetActive(true);
        //player.canMove = true;
    }

    public void OnClickedFlashlight() // 손전등 아이템 클릭
    {
        // 게임매니저에서 산소 0.3L(300ml)감소
        oxygenTank.ChangeOxygen(-300f);
        FillOxygenTank(); // 산소 이미지 업데이트

        StartCoroutine(InstantiateEffect());

        // 아이템을 SpawnPosition에 생성
        if (itemPrefabs.Count > 0) // itemPrefabs 리스트에 아이템이 있는지 확인
        {
            Instantiate(itemPrefabs[0], itemSpawnPosition, Quaternion.identity);
        }
    }
 
    public void FillOxygenTank()
    {
        // 산소량을 Tank이미지에 표시
        oxygenTankForeground.fillAmount = oxygenTank.GetOxygen() / 2000f;
    }

    private IEnumerator InstantiateEffect()
    {
        transceiver.TurnOnEmission();

        yield return new WaitForSeconds(2f);

        transceiver.TurnOffEmission();
    }
}
