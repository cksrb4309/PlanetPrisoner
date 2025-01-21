using UnityEngine;

public class BearTrap : Item
{
    public MeshRenderer meshRenderer;

    Material material;

    bool isSet = false; // 설치되었는지에 대한 트리거 변수
    bool isUsed = false;

    private void Awake()
    {
        material = meshRenderer.material;

        material.SetFloat("_TrapOpenAmount", 0.5f);
    }
    public void ActivateTrap()
    {
        if (isUsed) return;

        isSet = true;

        material.SetFloat("_TrapOpenAmount", 1f);
    }
    private void OnEnable()
    {
        material.SetFloat("_TrapOpenAmount", 0.5f);

        isUsed = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isSet && other.TryGetComponent(out IDamagable damagable))
        {
            material.SetFloat("_TrapOpenAmount", 0f);

            isSet = false;
            isUsed = true;

            damagable.Hit(1f);
        }
    }
}
