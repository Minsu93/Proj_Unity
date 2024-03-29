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

    protected override void FixedUpdate()
    {
        if (!activate)
            return;

        if (nearestPlanet == null)
            return;

        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier * characterGravityMultiplier * Time.deltaTime, ForceMode2D.Force);

        if (nearestPlanet != preNearestPlanet)       //Planet�� �ٲ�� �ѹ��� �ߵ��Ѵ�.
        {
            GameManager.Instance.playerNearestPlanet = nearestPlanet;
            ChangePlanet();
        }
    }

    protected override void ChangePlanet()
    {   
        //�߷� ���� ǥ��
        if (preNearestPlanet != null)
        {
            preNearestPlanet.graviteyViewOff();
        }
        if(nearestPlanet != null)
        {
            nearestPlanet.graviteyViewOn();

            //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //�༺ �ٲ� �̺�Ʈ ����
            playerBehavior.ChangePlanet();

            preNearestPlanet = nearestPlanet;

            CameraManager.instance.ChangeCamera(nearestPlanet.planetFOV);
        }
        



    }



}
