using System.Collections.Generic;
using UnityEngine;

public class Transceiver : MonoBehaviour
{
    [SerializeField] RequiredQuest requiredQuest;
    
    private Renderer objectRenderer;
    private Material objectMaterials;

    // 전송기 collider에 들어온 오브젝트 리스트
    public List<GameObject> objectInTransceiver = new List<GameObject>(); 

    //private Color OriginalEmissionColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectMaterials = objectRenderer.materials[2]; // index 2 
        objectMaterials.DisableKeyword("_EMISSION"); // 중앙만 꺼두기

        //OriginalEmissionColor = objectMaterials.GetColor("_EmissionColor");
    }

    public void TurnOffEmission()
    {
        //objectMaterials.SetColor("_EmissionColor",Color.black);
        objectMaterials.DisableKeyword("_EMISSION");
    }

    public void TurnOnEmission()
    {
        objectMaterials.EnableKeyword("_EMISSION");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Entered: " + other.gameObject.name);
        // 자식들 중에 CapsuleCollider만 isTrigger이므로 이 함수는 캡슐에 닿을 때 실행됨
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (!objectInTransceiver.Contains(other.gameObject))
            {
                objectInTransceiver.Add(other.gameObject); // 리스트에 추가
                requiredQuest.CheckQuestItem(other.gameObject.GetComponent<Item>().itemData, "In");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 마찬가지로 CapsuleCollider에서 나갈 때 실행됨
        if (objectInTransceiver.Contains(other.gameObject))
        {
            objectInTransceiver.Remove(other.gameObject); // 리스트에서 삭제
            requiredQuest.CheckQuestItem(other.gameObject.GetComponent<Item>().itemData, "Out");
        }
    }

    public void DestroyObjectInTransceiver() // 전송기 안에 있는 오브젝트들 모두 Destroy
    {
        foreach (GameObject obj in objectInTransceiver)
        {
            Destroy(obj);
        }

        // 리스트 초기화
        objectInTransceiver.Clear();
    }
}
