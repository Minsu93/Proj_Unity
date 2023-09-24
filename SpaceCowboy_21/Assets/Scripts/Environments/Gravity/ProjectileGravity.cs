using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGravity : Gravity
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();//������� �۵��ϴ°� �ƴ϶�, �ڼտ��� �θ��� Update�� �����ϴ°�. �Ʒ��ʰ��� ���� X...

        if (!activate)
            return; 

        if (nearestPlanet == null)
            return;

        if (nearestPlanet != preNearestPlanet)       //Planet�� �ٲ�� �ѹ��� �ߵ��Ѵ�.
        {
            preNearestPlanet = nearestPlanet;
            currentGravityForce = nearestPlanet.gravityForce;
        }

        float totalGrav = 0f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            Planet planet = gravityPlanets[i].GetComponent<Planet>();

            float planetRadius = planet.planetRadius;   //�༺�� ������
            float gravRadius = planet.gravityRadius;   //�༺�� �߷� ������

            Vector2 targetVec = gravityPlanets[i].transform.position - this.transform.position;   //�߷��� ����

            if (targetVec.magnitude < planetRadius)  // �༺ ������ ��� �ش� �༺�� �߷¸� ��°�� �޴´�
            {
                totalGrav = 1f;
            }
            else
            {
                float upVec = gravityPlanets[i] == nearestPlanet ? 1f : -1f;
                float percent = Mathf.Clamp01((targetVec.magnitude - planetRadius) / (gravRadius - planetRadius));
                float tempGrav = upVec * (1f - percent);

                totalGrav += tempGrav;
            }

        }

        Vector2 gravDir = (nearestPlanet.transform.position - this.transform.position).normalized;
        rb.AddForce(gravDir * totalGrav * currentGravityForce * gravityMultiplier * Time.deltaTime, ForceMode2D.Force);

        //Vector2 currDir = rb.velocity;
        //float vel = currDir.magnitude;
        //currDir = currDir + (gravDir * totalGrav * currentGravityForce * gravityMultiplier * 0.1f * Time.deltaTime);

        //rb.velocity = currDir.normalized * vel;

    }
}
