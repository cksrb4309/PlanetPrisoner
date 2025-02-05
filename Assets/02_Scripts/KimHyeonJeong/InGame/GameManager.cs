using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static int d_days = 3;
    public static int chance = 3;
    private void Awake()
    {
        d_days = 3;
        chance = 3;

        Instance = this;
    }
}
