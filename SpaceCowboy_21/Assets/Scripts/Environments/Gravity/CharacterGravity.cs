using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{
    public float characterGravityMultiplier = 1.0f;

    public event System.Action PlanetChangedEvent;

    protected override void FixedUpdate()
    {
        if (!activate)
            return;

        if (nearestPlanet == null)
            return;

        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier  * characterGravityMultiplier * Time.deltaTime, ForceMode2D.Force);

        if (nearestPlanet != preNearestPlanet)       //Planet�� �ٲ�� �ѹ��� �ߵ��Ѵ�.
        {
            ChangePlanet();
        }
    }

    protected virtual void ChangePlanet()
    {
        if (PlanetChangedEvent != null) PlanetChangedEvent();
        preNearestPlanet = nearestPlanet;

    }



}
