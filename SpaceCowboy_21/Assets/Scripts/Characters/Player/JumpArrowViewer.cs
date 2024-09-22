using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrowViewer : MonoBehaviour
{
    //public Vector2 trajectoryVector { get; set; }  //���� ����
    public Transform arrowTip;

    [SerializeField] private float viwerLength = 1f; //��� 


    public void SetArrowTip(Vector2 vec)
    {
        arrowTip.position = (Vector2)transform.position + (vec * viwerLength);
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * vec;
        arrowTip.rotation = Quaternion.LookRotation(Vector3.forward, upVec);
    }
}
