using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int d_days;

    public static GameManager Instance { get; private set; }


    void Awake()
    {
        Instance = this;

        d_days = 3; // 예시로 3일
    }
}
