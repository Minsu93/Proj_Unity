using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Transform planetTr;
    public float moveSpeed = 2f;

    private void FixedUpdate()
    {
        float _distance = (planetTr.position - transform.position).magnitude;
        float angle = (moveSpeed * 180) / (Mathf.PI * _distance);
        transform.RotateAround(planetTr.position, Vector3.forward, -1f * angle * Time.deltaTime);
    }
}
