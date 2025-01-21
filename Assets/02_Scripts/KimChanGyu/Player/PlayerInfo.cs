using UnityEngine;

public class PlayerInfo
{
    public PlayerSpaceSuit playerSpaceSuit = null;
    public PlayerController playerController = null;
    public PlayerInfo(
        PlayerSpaceSuit playerSpaceSuit,
        PlayerController playerController)
    {
        this.playerSpaceSuit = playerSpaceSuit;
        this.playerController = playerController;
    }
}
