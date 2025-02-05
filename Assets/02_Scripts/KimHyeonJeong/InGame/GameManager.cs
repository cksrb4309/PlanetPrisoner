using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int d_days;

    public static GameManager Instance{ get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        d_days = 3; // 예시로 3일
    }

    void Update()
    {
        
    }
}
