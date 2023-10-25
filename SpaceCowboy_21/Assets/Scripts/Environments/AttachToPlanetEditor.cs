#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttachToPlanet))]
public class AttachToPlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AttachToPlanet script = (AttachToPlanet)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Get Nearest Planet"))
        {
            script.GetNearestPlanet();
        }

        if(GUILayout.Button("Update Position ON/OFF"))
        {
            script.UpdatePosition();
        }

    }
}
#endif
