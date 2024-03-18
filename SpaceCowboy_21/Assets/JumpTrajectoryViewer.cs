using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrajectoryViewer : MonoBehaviour
{
    public float trajectoryLength { get; set; }  //선의 길이 
    public Vector2 trajectoryVector { get; set; }  //선의 방향
    public Material lineMaterial;
    public SpriteRenderer arrowTip;
    public float viwerLength = 1f; //상수 
    bool activate;
    Vector2 endPoint;
    public bool Activate 
    { 
        get { return activate; } 
        set 
        {  
            if(lineRenderer != null)
            {
                ResetPointPos();
                lineRenderer.enabled = value;   
                arrowTip.enabled = value;   
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

        arrowTip.color = lineMaterial.GetColor("_Color");
    }

    private void LateUpdate()
    {
        if (!activate) return;

        SetPointPosition();

        RotateArrowTip();
    }

    void ResetPointPos()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    void SetPointPosition()
    {
        endPoint = (Vector2)transform.position + (trajectoryVector * trajectoryLength * viwerLength);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }

    void RotateArrowTip()
    {
        arrowTip.transform.position = (Vector2)transform.position + (trajectoryVector * trajectoryLength * viwerLength);
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * trajectoryVector;
        arrowTip.transform.rotation = Quaternion.LookRotation(Vector3.forward, upVec);

    }
}
