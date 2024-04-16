using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{
    public float characterGravityMultiplier = 1.0f;


    protected override void GravityFunction()
    {
        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier * characterGravityMultiplier * Time.deltaTime, ForceMode2D.Force);
    }



}
