using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode, CustomEditor(typeof(SpawnPoint)), CanEditMultipleObjects]
public class SpawnPointEditor : Editor
{/*
    SpawnPoint current;
    float lineSize = 3;

    void Awake()
    {
        current = (SpawnPoint)target;
    }        

    void OnSceneGUI()
    {
        Handles.DrawDottedLine(new Vector2(-0.5f + current.transform.position.x, 0.5f + current.transform.position.y), new Vector2(0.5f + current.transform.position.x, 0.5f + current.transform.position.y), lineSize);
        Handles.DrawDottedLine(new Vector2(0.5f + current.transform.position.x, 0.5f + current.transform.position.y), new Vector2(0.5f + current.transform.position.x, -0.5f + current.transform.position.y), lineSize);
        Handles.DrawDottedLine(new Vector2(0.5f + current.transform.position.x, -0.5f + current.transform.position.y), new Vector2(-0.5f + current.transform.position.x, -0.5f + current.transform.position.y), lineSize);
        Handles.DrawDottedLine(new Vector2(-0.5f + current.transform.position.x, -0.5f + current.transform.position.y), new Vector2(-0.5f + current.transform.position.x, 0.5f + current.transform.position.y), lineSize);
    }*/
}
