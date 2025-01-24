using UnityEngine;
using UnityEditor;
using System.Linq;


//[CustomEditor(typeof(PlayerSpaceSuit))]
//[CanEditMultipleObjects]
//public class ButtonHelper : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector(); // 기존 인스펙터 UI 유지

//        PlayerSpaceSuit[] targetsArray = targets.Cast<PlayerSpaceSuit>().ToArray();

//        if (targetsArray == null || targetsArray.Length == 0)
//        {
//            EditorGUILayout.HelpBox("No valid targets selected.", MessageType.Warning);

//            return;
//        }

//        if (GUILayout.Button("Hit"))
//        {
//            foreach (var target in targetsArray)
//            {
//                if (target != null)
//                {
//                    target.Hit(0.1f);
//                }
//            }
//        }
//    }
//}
[CustomEditor(typeof(PlayerSpaceSuit))]
public class ButtonHelper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기존 인스펙터 UI 유지

        PlayerSpaceSuit targetObject = (PlayerSpaceSuit)target;

        if (targetObject == null)
        {
            EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Hit"))
        {
            targetObject.Damaged(0.1f);
        }
    }
}
