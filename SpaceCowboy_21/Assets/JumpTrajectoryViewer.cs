using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrajectoryViewer : MonoBehaviour
{
    bool activate;
    public bool Activate 
    { 
        get { return activate; } 
        set 
        {  
            if(lineRenderer != null)
            {
                SetPointPosition();
                lineRenderer.enabled = value;   
            }    
            activate = value; 
        } 
    }

    LineRenderer lineRenderer;
    PlayerBehavior playerBehavior;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerBehavior = GetComponentInParent<PlayerBehavior>();
    }

    private void Update()
    {
        if (!activate) return;

        SetPointPosition();
    }

    void SetPointPosition()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, playerBehavior.mousePos);
    }
}
