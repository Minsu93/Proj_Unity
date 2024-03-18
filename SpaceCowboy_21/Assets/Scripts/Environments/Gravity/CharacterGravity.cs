using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{

    public event System.Action PlanetChangedEvent;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (nearestPlanet != preNearestPlanet)       //Planet이 바뀌면 한번만 발동한다.
        {
            ChangePlanet();
        }
    }

    protected virtual void ChangePlanet()
    {
        preNearestPlanet = nearestPlanet;

        if (PlanetChangedEvent != null) PlanetChangedEvent();
    }

   

}
