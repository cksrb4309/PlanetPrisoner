using TMPro;
using UnityEditor;
using UnityEngine;

public class TempFunctioe : MonoBehaviour
{
    public GameObject targetObject;
    public void RenderLayerSetting()
    {
        MeshRenderer[] meshRenderers = targetObject.GetComponentsInChildren<MeshRenderer>();

        uint layer = 1 << 1;

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.renderingLayerMask = layer;
        }
    }
    public void FontSetting()
    {
        TMP_Text[] components = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);

        foreach (TMP_Text component in components)
        {
            if (component.GetComponent<TextFontSetting>() == null)
            {
                component.gameObject.AddComponent<TextFontSetting>();
            }
        }
    }
}



[CustomEditor(typeof(TempFunctioe))]
public class TempFunctionEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기존 인스펙터 UI 유지

        TempFunctioe targetObject = (TempFunctioe)target;

        if (targetObject == null)
        {
            EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Cave Meshrenderer Setting"))
        {
            targetObject.RenderLayerSetting();
        }
        if (GUILayout.Button("TextObject Add FontSetting"))
        {
            targetObject.FontSetting();
        }
    }
}