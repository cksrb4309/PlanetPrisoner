using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] float oxygen = 2000; // 2L=2000ml

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public float GetOxygen() // oxygen변수 getter
    {
        return oxygen;
    }
    public void ChangeOxygen(float amount)
    {
        oxygen += amount;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
