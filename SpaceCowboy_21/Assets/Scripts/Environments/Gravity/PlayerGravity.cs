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


        //��� üũ
        //CheckOxygenState();
    }


    protected override void ChangePlanet()
    {   //�༺�� �ٲ� �� ����
        //�༺�� ������ ���� x
        //�߷� ǥ�ø� ����
        if (preNearestPlanet != null) preNearestPlanet.graviteyViewOff();
        nearestPlanet.graviteyViewOn();

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //�༺ �ٲ� �̺�Ʈ ����
        playerBehavior.ChangePlanet();

        preNearestPlanet = nearestPlanet;
        currentGravityForce = nearestPlanet.gravityForce;   //�༺���� �ٸ� �߷� ����
        //�༺�� ī�޶� ����
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
        {   //Green �� �ϳ��� ������
            oxygenInt = 0;
        }
        else if (types.Contains(PlanetType.Red))
        {   //Green�� ���� Red�� ������
            oxygenInt = 1;

        }
        else
        {   //Blue�� ������ 
            oxygenInt = 2;
        }
    }
    */
}
