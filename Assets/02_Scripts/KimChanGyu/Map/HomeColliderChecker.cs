using UnityEngine;

public class HomeColliderChecker : MonoBehaviour
{
    public static HomeColliderChecker Instance { get; private set; }

    public BoxCollider boxCollider;

    private void Awake()
    {
        Instance = this;
    }
    public bool IsObjectInsideBox(Vector3 position)
    {
        // �ڽ� �ݶ��̴��� ���� ���� ��ġ
        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * 0.5f;

        // �ڽ� �ݶ��̴��� ȸ��
        Quaternion rotation = boxCollider.transform.rotation;

        // OverlapBox�� ����Ͽ� position�� �ڽ� �ȿ� �ִ��� Ȯ��
        Collider[] hitColliders = Physics.OverlapBox(worldCenter, halfExtents, rotation);

        // hitColliders �迭�� ���Ե� ��� �ݶ��̴��� �˻��Ͽ� position�� ��ġ���� Ȯ��
        foreach (Collider collider in hitColliders)
        {
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }
}
