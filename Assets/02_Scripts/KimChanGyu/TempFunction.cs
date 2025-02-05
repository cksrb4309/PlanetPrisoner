//using TMPro;
//using UnityEngine;

//public class TempFunctioe : MonoBehaviour
//{
//    public GameObject targetObject;
//    public void RenderLayerSetting()
//    {
//        MeshRenderer[] meshRenderers = targetObject.GetComponentsInChildren<MeshRenderer>();

//        uint layer = 1 << 1;

//        foreach (MeshRenderer meshRenderer in meshRenderers)
//        {
//            meshRenderer.renderingLayerMask = layer;
//        }
//    }
//    public void FontSetting()
//    {
//        TMP_Text[] components = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);

//        foreach (TMP_Text component in components)
//        {
//            if (component.GetComponent<TextFontSetting>() == null)
//            {
//                component.gameObject.AddComponent<TextFontSetting>();
//            }
//        }
//    }
//}

