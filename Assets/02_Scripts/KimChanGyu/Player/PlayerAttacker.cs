using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public Transform cameraTransform;

    public LayerMask layerMask;

    [SerializeField] ParticleSystem bloodEffect;

    Ray ray;

    public void Attack(float damage, float range)
    {
        ray.origin = cameraTransform.position;

        ray.direction = cameraTransform.forward;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<IDamagable>().Damaged(damage);

            PlayerAudioController.PlayerAudioPlay(AudioName.PlayerAttackHit);

            bloodEffect.transform.position = hitInfo.point;

            bloodEffect.Play();
        }
        else
        {
            Debug.Log("공격 미스");
        }
    }
}
