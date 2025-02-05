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

    [SerializeField] Transform itemSpawnPosition;

    [SerializeField] List<GameObject> itemPrefabs = new List<GameObject>();

    public void OnClickedXbutton() // 상점 닫기 클릭
    {
        if (transceiver != null) transceiver.TurnOffEmission();

        shopUI.SetActive(false);
        inGameUI.SetActive(true);

        player.EnableMovement();
    }

    public void OnClickedFlashlight() // 손전등 아이템 클릭
    {
        // 게임매니저에서 산소 20L감소
        oxygenTank.ChangeOxygen(-20f);
        FillOxygenTank(); // 산소 이미지 업데이트

        StartCoroutine(InstantiateEffect());

        // 아이템을 SpawnPosition에 생성
        if (itemPrefabs.Count > 0) // itemPrefabs 리스트에 아이템이 있는지 확인
        {
            Instantiate(itemPrefabs[0], itemSpawnPosition.position, Quaternion.identity);
        }
    }

    public void OnClickedScanner() // 스캐너 아이템 클릭
    {
        // 게임매니저에서 산소 35L감소
        oxygenTank.ChangeOxygen(-35f);
        FillOxygenTank(); // 산소 이미지 업데이트

        StartCoroutine(InstantiateEffect());

        // 아이템을 SpawnPosition에 생성
        if (itemPrefabs.Count > 0) // itemPrefabs 리스트에 아이템이 있는지 확인
        {
            Instantiate(itemPrefabs[1], itemSpawnPosition.position, Quaternion.identity);
        }
    }

    public void OnClickedNewShoes() // 신형호흡기 아이템 클릭
    {
        // 게임매니저에서 산소 60L감소
        oxygenTank.ChangeOxygen(-60f);
        FillOxygenTank(); // 산소 이미지 업데이트

        StartCoroutine(InstantiateEffect());

        // 아이템을 SpawnPosition에 생성
        if (itemPrefabs.Count > 0) // itemPrefabs 리스트에 아이템이 있는지 확인
        {
            Instantiate(itemPrefabs[2], itemSpawnPosition.position, Quaternion.identity);
        }
    }

    public void OnClickedNewSuit() // 신형우주복 아이템 클릭
    {
        // 게임매니저에서 산소 80L감소
        oxygenTank.ChangeOxygen(-80f);
        FillOxygenTank(); // 산소 이미지 업데이트

        StartCoroutine(InstantiateEffect());

        // 아이템을 SpawnPosition에 생성
        if (itemPrefabs.Count > 0) // itemPrefabs 리스트에 아이템이 있는지 확인
        {
            Instantiate(itemPrefabs[3], itemSpawnPosition.position, Quaternion.identity);
        }
    }

    public void FillOxygenTank()
    {
        // 산소량을 Tank이미지에 표시
        oxygenTankForeground.fillAmount = oxygenTank.GetOxygen() / 200f;
    }

    private IEnumerator InstantiateEffect()
    {
        transceiver.TurnOnEmission();

        yield return new WaitForSeconds(2f);

        transceiver.TurnOffEmission();
    }
}