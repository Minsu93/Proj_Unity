using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetSpace;


public class Gravity : MonoBehaviour
{
    public List<Planet> gravityPlanets = new List<Planet>();
    public Planet nearestPlanet;
    protected Planet preNearestPlanet;
    protected float currentGravityForce;
    public float gravityMultiplier = 1f;

    public bool activate = true;
    public int oxygenInt = -1;

    protected Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (!activate)
            return;

        //���� ������ �ִ� �༺ üũ
        nearestPlanet = GetNearestPlanet();

        //��� üũ
        CheckOxygenState();

    }

    protected virtual void FixedUpdate()
    {
        if (!activate)
            return;

    }

    Planet GetNearestPlanet()       //���� ����� Planet ��ũ��Ʈ�� �ش� ��ũ��Ʈ�� GravityForce�� �����´�.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            Vector2 targetVec = gravityPlanets[i].transform.position - this.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetVec.normalized, 10f, LayerMask.GetMask("Planet"));
            if (hit.collider != null)
            {
                if (hit.distance < minDist)
                {
                    minDist = hit.distance;
                    targetPlanet = gravityPlanets[i];
                }
            }

            /*
            else //�浹�Ǵ� ���� ������, �׳� transform�� ���� �������� �������.
            {
                if (targetVec.magnitude < minDist)
                {
                    minDist = targetVec.magnitude;
                    targetPlanet = gravityPlanets[i];
                }
            }
            */

        }
        return targetPlanet;
    }

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
            oxygenInt =1;

        }
        else
        {   //Blue�� ������ 
            oxygenInt = 2;
        }
    }




}
