using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : CharacterGravity
{
    PlayerBehavior playerBehavior;
    //OxygenHealth oxygenHealth;

    protected override void Awake()
    {
        base.Awake();

        playerBehavior = GetComponent<PlayerBehavior>();
        //oxygenHealth = GetComponent<OxygenHealth>();

    }



    protected override void ChangePlanet()
    {   
        //중력 범위 표시
        if (preNearestPlanet != null)
        {
            preNearestPlanet.graviteyViewOff();
        }
        nearestPlanet.graviteyViewOn();

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //행성 바꿈 이벤트 실행
        playerBehavior.ChangePlanet();

        preNearestPlanet = nearestPlanet;

        CameraManager.instance.ChangeCamera(nearestPlanet.planetFOV);

    }



}
