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
    {   //�༺�� �ٲ� �� ����
        //�༺�� ������ ���� x
        //�߷� ǥ�ø� ����
        if (preNearestPlanet != null) preNearestPlanet.graviteyViewOff();
        nearestPlanet.graviteyViewOn();

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //�༺ �ٲ� �̺�Ʈ ����
        playerBehavior.ChangePlanet();

        preNearestPlanet = nearestPlanet;
        currentGravityForce = nearestPlanet.gravityForce;   //�༺���� �ٸ� �߷� ����

        //GameManager.Instance.ChangeCamera(nearestPlanet.lens);
        GameManager.Instance.ChangeCamera(nearestPlanet.planetFOV);

    }



}
