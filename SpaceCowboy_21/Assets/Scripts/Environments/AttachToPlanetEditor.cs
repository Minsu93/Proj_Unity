#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttachToPlanet)), CanEditMultipleObjects]
public class AttachToPlanetEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    AttachToPlanet script = (AttachToPlanet)target;

    //    DrawDefaultInspector();

    //    if(GUILayout.Button("Get Nearest Planet"))
    //    {
            
    //        script.GetNearestPlanet();
    //    }


    //}
}
#endif
