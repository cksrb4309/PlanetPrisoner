using UnityEngine;

public class M_MurlocGroupHelper : MonoBehaviour
{
    [SerializeField] M_Murloc[] mulocs;

    private void Start()
    {
        mulocs = GetComponentsInChildren<M_Murloc>();
    }
}
