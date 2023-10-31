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
    protected override void Update()
    {
        base.Update();


        //산소 체크
        //CheckOxygenState();
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
        //행성별 카메라 변경
        //if(nearestPlanet.gravityForce < 400f)
        //{
        //    GameManager.Instance.ChangeCamera(11f);
        //}
        //else
        //{
        //    GameManager.Instance.ChangeCamera(8f);
        //}
        GameManager.Instance.ChangeCamera(nearestPlanet.lens);

        //oxygenHealth.consumeMultiplier = nearestPlanet.oxygenAmount;
    }


    /*
    void CheckOxygenState()
    {

        List<PlanetType> types = new List<PlanetType>();

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            types.Add(gravityPlanets[i].planetType);
        }


        if (types.Contains(PlanetType.Green))
        {   //Green 이 하나라도 있으면
            oxygenInt = 0;
        }
        else if (types.Contains(PlanetType.Red))
        {   //Green은 없고 Red가 있으면
            oxygenInt = 1;

        }
        else
        {   //Blue만 있으면 
            oxygenInt = 2;
        }
    }
    */
}
