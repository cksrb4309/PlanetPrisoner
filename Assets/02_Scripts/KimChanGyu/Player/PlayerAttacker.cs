using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public Transform cameraTransform;

    public LayerMask layerMask;

    Ray ray;

    public void Attack(float damage, float range)
    {
        Debug.Log("PlayerAttacker Attack");

        ray.origin = cameraTransform.position;

        ray.direction = cameraTransform.forward;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask))
        {
            Debug.Log("공격 적중 : " + hitInfo.collider.gameObject.name);
            hitInfo.collider.GetComponent<IDamagable>().Damaged(damage);
        }
        else
        {
            Debug.Log("공격 미스");
        }
    }
}
