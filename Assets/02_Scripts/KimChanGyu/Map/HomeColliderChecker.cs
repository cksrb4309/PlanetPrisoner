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
        // 박스 콜라이더의 월드 공간 위치
        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * 0.5f;

        // 박스 콜라이더의 회전
        Quaternion rotation = boxCollider.transform.rotation;

        // OverlapBox를 사용하여 position이 박스 안에 있는지 확인
        Collider[] hitColliders = Physics.OverlapBox(worldCenter, halfExtents, rotation);

        // hitColliders 배열에 포함된 모든 콜라이더를 검사하여 position과 겹치는지 확인
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
