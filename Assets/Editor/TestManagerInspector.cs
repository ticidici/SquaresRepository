using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode, CustomEditor(typeof(TestManager)), CanEditMultipleObjects]
public class TestManagerInspector : Editor {
    TestManager t;

    void Awake()
    {
        t = (TestManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorList.Show(serializedObject.FindProperty("_colors"), EditorListOption.Buttons | EditorListOption.ListLabel);
        EditorList.Show(serializedObject.FindProperty("_spawnPoints"), EditorListOption.ListLabel);
        serializedObject.ApplyModifiedProperties();

        SerializedProperty list = serializedObject.FindProperty("_spawnPoints");

        if (list.arraySize < 4 && GUILayout.Button("New SpawnPoint"))
        {
            Undo.RecordObject(t, "Add New");
            t.AddNew();
            EditorUtility.SetDirty(t);
            Debug.Log("New SpawnPoint");
        }
        if (list.arraySize > 0 && GUILayout.Button("Delete SpawnPoint"))
        {
            Undo.RecordObject(t, "Remove Last");
            t.RemoveLast();
            EditorUtility.SetDirty(t);
            Debug.Log("Delete SpawnPoint");
        }

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button(new GUIContent("MOTIVATE!", "Motivate!"), GUILayout.Height(80)))
        {
            Debug.LogWarning("You are being motivated!!!");
        }
        GUILayout.EndHorizontal();
    }
}
