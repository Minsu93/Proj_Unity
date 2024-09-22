using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : CharacterGravity
{
    PlayerBehavior playerBehavior;

    protected override void Awake()
    {
        base.Awake();
        playerBehavior = GetComponent<PlayerBehavior>();
    }


    protected override void GravityFunction()
    {
        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier * characterGravityMultiplier * Time.deltaTime, ForceMode2D.Force);

    }

    protected override void ChangePlanet()
    {
        //GameManager.Instance.playerManager.playerNearestPlanet = nearestPlanet;

        ////중력 범위 표시
        //if (preNearestPlanet != null)
        //{
        //    preNearestPlanet.graviteyViewOff();
        //}

        //if(nearestPlanet != null)
        //{
        //    nearestPlanet.graviteyViewOn();

        //    GameManager.Instance.cameraManager.ChangeCamera(nearestPlanet.planetFOV);
        //}

        preNearestPlanet = nearestPlanet;
        playerBehavior.ChangePlanet();


    }



}
