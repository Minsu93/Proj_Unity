using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2 (1,0);
    Material cloudMat;
    private void Awake()
    {
        cloudMat = GetComponentInChildren<Renderer>().material;
    }
    private void FixedUpdate()
    {
        cloudMat.SetVector("_ScrollSpeed", scrollSpeed);
    }


}
