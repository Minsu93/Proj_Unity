using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.U2D;


public class SpaceBorderGetter : MonoBehaviour
{
    [SerializeField] SpriteShapeController ssController;
    [SerializeField] SpriteShapeController ss2Controller;

    private void OnValidate()
    {
        if(ssController == null)
            ssController = GetComponentsInChildren<SpriteShapeController>()[0];
        if (ss2Controller == null)
            ss2Controller = GetComponentsInChildren<SpriteShapeController>()[1];
    }

    [ContextMenu("GetBorder")]
    void GetBorder()
    {
        //ss1 > ss2
        
        int pointCounts = ssController.spline.GetPointCount();

        ss2Controller.spline.Clear();

        for (int i = 0; i< pointCounts; i++)
        {
            ss2Controller.spline.InsertPointAt(i, ssController.spline.GetPosition(i));
        }
    }

}
