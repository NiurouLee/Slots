using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DragonPlus.SpineExtensions
{

    public class SpineAnimationChangeEditor : UnityEditor.EditorWindow
    {
        [MenuItem("Window/SpineExtensions/SpineAnimationChange")]

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SpineAnimationChangeEditor));
        }
        public GameObject goRoot;
        private SpineAnimationChange[] myTests;
        void OnGUI()
        {
            goRoot = EditorGUILayout.ObjectField(goRoot, typeof(GameObject), true) as GameObject;

            // The actual window code goes here
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Huang On"))
            {
                myTests = goRoot.GetComponentsInChildren<SpineAnimationChange>();
                if (myTests != null)
                {
                    bool init = SpineAnimationChange.InitInEditor();
                    Debug.LogError($"Arthur--->Huang On={init}");
                    EditorApplication.update -= OnAnimationEditorUpdate;
                    EditorApplication.update += OnAnimationEditorUpdate;
                    foreach (var item in myTests)
                    {
                        item.StartInEditor();
                    };

                }
            }
            if (GUILayout.Button("Huang Off"))
            {
                bool release = SpineAnimationChange.ReleaseEditor();
                Debug.LogError($"Arthur--->Huang On={release}");
                EditorApplication.update -= OnAnimationEditorUpdate;
            }
            EditorGUILayout.EndVertical();
            if (myTests != null)
            {
                EditorGUILayout.BeginVertical();
                foreach (var item in myTests)
                {
                    if (item == null) continue;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item.gameObject.name);
                    EditorGUILayout.LabelField(item.PlayName);
                    EditorGUILayout.EndHorizontal();
                };
                EditorGUILayout.EndVertical();
            }
        }

        void OnDestroy()
        {
            EditorApplication.update -= OnAnimationEditorUpdate;
        }
        void OnAnimationEditorUpdate()
        {
            //Debug.Log("Arthur----->OnAnimationEditorUpdate");
            if (myTests != null)
            {
                foreach (var item in myTests)
                {
                    item.OnAnimationEditorUpdate();
                };
            }
        }
    }
}

