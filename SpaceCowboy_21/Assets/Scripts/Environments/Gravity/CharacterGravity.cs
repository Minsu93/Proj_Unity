using PlanetSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{
    public Vector2 nearestPoint;
    public EdgeCollider2D nearestEdgeCollider;
    [Header("Test")]
    public float maxGravDIst;
    public int oxygenInt = -1;



    protected override void Update()
    {
        if (!activate)
            return;

        base.Update();

        if (nearestPlanet == null)
            return;

        GetNearestPoint();



    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (nearestPlanet == null)
            return;

        if(nearestPlanet != preNearestPlanet)       //Planet�� �ٲ�� �ѹ��� �ߵ��Ѵ�.
        {
            ChangePlanet();
        }


        Vector2 grav = (nearestPoint - (Vector2)transform.position).normalized;

        float gravDist = (nearestPoint - (Vector2)transform.position).magnitude;        //����� �������� �� �߷��� ���ϰ� ����
        gravDist = Mathf.Clamp(maxGravDIst / gravDist, 1, maxGravDIst * maxGravDIst);

        rb.AddForce(grav * currentGravityForce * gravityMultiplier * gravDist * Time.deltaTime, ForceMode2D.Force);
        return;
    }

    protected virtual void ChangePlanet()
    {

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //�༺ �ٲ� �̺�Ʈ ����

        preNearestPlanet = nearestPlanet;
        currentGravityForce = nearestPlanet.gravityForce;   //�༺���� �ٸ� �߷� ����
    }

    void GetNearestPoint()
    {
        nearestEdgeCollider = nearestPlanet.edgeColl;
        nearestPoint = nearestEdgeCollider.ClosestPoint(transform.position);
    }

    public override Planet GetNearestPlanet()       //���� ����� Planet ��ũ��Ʈ�� �ش� ��ũ��Ʈ�� GravityForce�� �����´�.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            //�� planet���� edgecollider.nearestPoint������ �Ÿ��� ���Ѵ�. 
            Vector2 distVec = gravityPlanets[i].edgeColl.ClosestPoint(transform.position) - (Vector2)transform.position;
            float dist = distVec.magnitude;

            //���� ª�� ���� nearest planet���� �����Ѵ�. 
            if (dist < minDist)
            {
                minDist = dist;
                targetPlanet = gravityPlanets[i];
            }
        }

        //edgecol �� point ���� ���ÿ� ó���Ѵ�. 
        
        return targetPlanet;
    }

}
