using UnityEngine;

public class ScanResultCamera : MonoBehaviour
{
    static ScanResultCamera instance = null;
    public static ScanResultCamera Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }

}
