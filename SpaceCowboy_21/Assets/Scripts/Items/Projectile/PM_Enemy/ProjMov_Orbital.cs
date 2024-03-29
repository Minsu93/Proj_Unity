using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Orbital : ProjectileMovement
{
    bool revolveOn = false;
    int direction = 0;
    public bool isRight { get; set; }
    public Planet centerPlanet { get; set; }

    public override void StartMovement(float speed)
    {
        //기본 속도 움직임 
        revolveOn = true;
        this.speed = speed;
        direction = isRight? -1 : 1;
    }

    public override void StopMovement()
    {
        revolveOn = false;
    }

    private void FixedUpdate()
    {
        if (!revolveOn) return;

        transform.RotateAround(centerPlanet.transform.position, Vector3.forward, direction * speed * Time.deltaTime);
    }
}
