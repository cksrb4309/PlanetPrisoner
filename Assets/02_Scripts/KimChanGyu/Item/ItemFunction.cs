using UnityEngine;

public class ItemFunction : MonoBehaviour // 아이템 액션 모음 클래스
{
    #region 무기 휘두르기
    public void UseWeapon(PlayerInfo playerInfo)
    {

    }
    #endregion

    #region 함정 설치
    public void DropTrap(PlayerInfo playerInfo)
    {

    }
    #endregion

    #region 스캐너 동작
    public void ScanArea(PlayerInfo playerInfo)
    {

    }
    #endregion

    #region 강화 신발 장착
    public void EquipEnhancedShoes(PlayerInfo playerInfo)
    {
        playerInfo.playerController.EquipEnhancedShoes();
    }
    #endregion

    #region 강화 우주복 장착
    public void EquipEnhancedSuit(PlayerInfo playerInfo)
    {
        playerInfo.playerSpaceSuit.EquipEnhancedSuit();
    }
    #endregion

}
