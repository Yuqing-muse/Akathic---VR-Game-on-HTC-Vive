using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Edwon.VR
{
    [CustomEditor(typeof(VRGestureRig))]
    public class VRGestureRigEditor : Editor
    {
        SerializedProperty head;
        SerializedProperty handLeft;
        SerializedProperty handRight;
        SerializedProperty handLeftModel;
        SerializedProperty handRightModel;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            head = serializedObject.FindProperty("head");
            handLeft = serializedObject.FindProperty("handLeft");
            handRight = serializedObject.FindProperty("handRight");
            handLeftModel = serializedObject.FindProperty("handLeftModel");
            handRightModel = serializedObject.FindProperty("handRightModel");

            VRGestureRig vrGestureRig = (VRGestureRig)target;
            if (GUILayout.Button("Auto Setup"))
            {
                vrGestureRig.AutoSetup();
            }

            EditorGUILayout.PropertyField(head);
            EditorGUILayout.PropertyField(handLeft);
            EditorGUILayout.PropertyField(handRight);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Spawn Controller Models");
            vrGestureRig.spawnControllerModels = EditorGUILayout.Toggle(vrGestureRig.spawnControllerModels);
            EditorGUILayout.EndHorizontal();

            if (vrGestureRig.spawnControllerModels)
            {
                EditorGUILayout.PropertyField(handLeftModel);
                EditorGUILayout.PropertyField(handRightModel);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}