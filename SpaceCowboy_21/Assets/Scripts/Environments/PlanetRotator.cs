using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    Rigidbody2D rb;
    public float rotationSpeed;
    float degree;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        degree = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        degree += rotationSpeed * Time.deltaTime;
        rb.MoveRotation(degree);
    }
}
