using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePingPong : MonoBehaviour
{
    public float speed = 1.0f;
    public float size = 1.0f;
    // Update is called once per frame
    void Update()
    {
        float f = Mathf.PingPong(Time.time * speed, size);
        f = f + 1;

        transform.localScale = new Vector3(f, f, f);
    }
}
