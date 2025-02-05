using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    public LayerMask groundLayerMask;

    public Transform topPosition;
    public bool IsGround => isGround;
    public bool IsColliderBelow => isColliderBelow;
    [SerializeField] float range;

    bool isTriggered = false;
    bool isGround = false;
    bool isColliderBelow = false;
    Ray ray = new Ray();

    private void Start()
    {
        ray.direction = Vector3.down;
    }
    private void Update()
    {
        ray.origin = topPosition.position;

        isGround = isTriggered ? false : Physics.Raycast(ray, range, groundLayerMask);
        isColliderBelow = Physics.Raycast(ray, range);
    }
    private void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
