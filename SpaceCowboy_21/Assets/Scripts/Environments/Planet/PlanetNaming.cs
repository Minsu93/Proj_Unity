using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetSpace;
using UnityEditor;

public class PlanetNaming :MonoBehaviour
{
    Planet planet;
    string planetSize;

    private void OnValidate()
    {
        planet = GetComponent<Planet>();

    }


    private void OnDrawGizmos()
    {
        switch (planet.planetSize)
        {
            case PlanetSize.XS:
                planetSize = "Xsmall";
                break;
            case PlanetSize.S:
                planetSize = "Small";
                break;
            case PlanetSize.SM:
                planetSize = "SMedium";
                break;
            case PlanetSize.M:
                planetSize = "Medium";
                break;
            case PlanetSize.L:
                planetSize = "Large";
                break;
            case PlanetSize.XL:
                planetSize = "Xlarge";
                break;

            default: break;
        }

        bool bSelelctObject = false;
        if (Selection.activeGameObject == transform.gameObject)
        {
            bSelelctObject = true;
        }


        GUIStyle style = new GUIStyle();
        if(bSelelctObject)
        {
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.green;
        }
        else
        {
            style.normal.textColor = new Color(0, 1, 0, 0.5f);
        }
        style.alignment = TextAnchor.MiddleCenter;

        Handles.Label(transform.position, planetSize,style);
     

    }
}
