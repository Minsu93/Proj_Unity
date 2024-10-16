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
        if (nearestPlanet != null) nearestPlanet.ShowDirectionArrows(false);
        nearestPlanet = planet;
        if (nearestPlanet != null) nearestPlanet.ShowDirectionArrows(true);
        playerBehavior.ChangePlanet();
        //GameManager.Instance.cameraManager.ChangeCamFollow(planet.transform);
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, lerpF.ToString());
    }

}
