using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetEnvironment))]
public class PlanetEnvironmentEditor : Editor
{

    public override void OnInspectorGUI()
    {
        
        PlanetEnvironment script = (PlanetEnvironment)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Generate Planet Environment"))
        {
            script.GeneratePlanet();
        }
    }
}
