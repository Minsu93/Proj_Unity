using PlanetSpace;
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
    {   //행성이 바뀔 때 실행
        //행성이 없으면 실행 x
        //중력 표시를 끈다
        if (preNearestPlanet != null) preNearestPlanet.graviteyViewOff();
        nearestPlanet.graviteyViewOn();

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //행성 바꿈 이벤트 실행
        playerBehavior.ChangePlanet();

        preNearestPlanet = nearestPlanet;
        currentGravityForce = nearestPlanet.gravityForce;   //행성별로 다른 중력 적용

        //GameManager.Instance.ChangeCamera(nearestPlanet.lens);
        GameManager.Instance.ChangeCamera(nearestPlanet.planetFOV);

    }



}
