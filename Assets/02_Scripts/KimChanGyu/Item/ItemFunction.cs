using UnityEngine;

public class ItemFunction : MonoBehaviour // 아이템 액션 모음 클래스
{
    #region 무기 휘두르기
    public float weaponDamage, weaponRange;
    public void UseWeapon(PlayerInfo playerInfo)
    {
        playerInfo.playerAttacker.Attack(weaponDamage, weaponRange);
    }
    #endregion

    #region 함정 설치
    public void ActivateTrap(PlayerInfo playerInfo)
    {
        BearTrap trap = playerInfo.playerItemHandler.GetCurrentItem() as BearTrap;

        trap?.ActivateTrap();
    }
    #endregion

    #region 스캐너 동작

    [SerializeField] float scannerScanRange = 40f;

    public void ScanArea(PlayerInfo playerInfo)
    {
        playerInfo.playerScanner.Scan(scannerScanRange);
    }
    #endregion

    #region 강화 신발 장착
    public void EquipEnhancedShoes(PlayerInfo playerInfo)
    {
        Debug.Log("강화 신발 장착");

        playerInfo.playerController.EquipEnhancedShoes();
    }
    #endregion

    #region 강화 우주복 장착
    public void EquipEnhancedSuit(PlayerInfo playerInfo)
    {
        Debug.Log("강화 우주복 장착");

        playerInfo.playerSpaceSuit.EquipEnhancedSuit();
    }
    #endregion

    #region 함정 설치
    public void ActivateFlashlight(PlayerInfo playerInfo)
    {
        Flashlight flashlight = playerInfo.playerItemHandler.GetCurrentItem() as Flashlight;

        flashlight?.ToggleFlashlight();

        Debug.Log("ActivateFlashlight");
    }
    #endregion
}
