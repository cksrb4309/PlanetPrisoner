using UnityEngine;

public class ItemShop : MonoBehaviour
{
    [SerializeField]
    GameObject shopUI;
    [SerializeField]
    GameObject inGameUI;
    [SerializeField]
    PlayerController player;


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void OnClickedXbutton()
    {
        shopUI.SetActive(false);
        inGameUI.SetActive(true);
        player.canMove = true;
    }
}
