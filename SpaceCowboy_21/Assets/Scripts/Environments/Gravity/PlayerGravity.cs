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
        //�߷� ���� ǥ��
        if (preNearestPlanet != null)
        {
            preNearestPlanet.graviteyViewOff();
        }
        nearestPlanet.graviteyViewOn();

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //�༺ �ٲ� �̺�Ʈ ����
        playerBehavior.ChangePlanet();

        preNearestPlanet = nearestPlanet;

        CameraManager.instance.ChangeCamera(nearestPlanet.planetFOV);

    }



}
