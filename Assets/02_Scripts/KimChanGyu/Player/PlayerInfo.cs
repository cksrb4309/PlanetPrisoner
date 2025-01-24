using UnityEngine;

public class PlayerInfo
{
    public PlayerSpaceSuit playerSpaceSuit = null;
    public PlayerController playerController = null;
    public PlayerScanner playerScanner = null;
    public PlayerItemHandler playerItemHandler = null;
    public PlayerAttacker playerAttacker = null;
    public PlayerInfo(
        PlayerSpaceSuit playerSpaceSuit,
        PlayerController playerController,
        PlayerScanner playerScanner,
        PlayerItemHandler playerItemHandler,
        PlayerAttacker playerAttacker)
    {
        this.playerSpaceSuit = playerSpaceSuit;
        this.playerController = playerController;
        this.playerScanner = playerScanner;
        this.playerItemHandler = playerItemHandler;
        this.playerAttacker = playerAttacker;
    }
}
