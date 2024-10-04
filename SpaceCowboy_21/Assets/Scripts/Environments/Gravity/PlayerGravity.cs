using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerGravity : CharacterGravity
{
    PlayerBehavior playerBehavior;

    protected override void Awake()
    {
        base.Awake();
        playerBehavior = GetComponent<PlayerBehavior>();
    }


    protected override void ChangePlanet(Planet planet)
    {
        nearestPlanet = planet;
        playerBehavior.ChangePlanet();
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, lerpF.ToString());
    }

}
