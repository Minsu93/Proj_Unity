using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public bool activate = true;
    [SerializeField] protected float gravityMultiplier = 1.0f;

    protected float currentGravityForce;    //���� �߷�

    //����
    public Vector2 nearestPoint { get; set; }


    //Meteor�� ���� ���� �߷�
    //public bool fixedGravity = false;
    //Planet fixedPlanet;
    //Vector2 fixedPoint;
    //Vector2 fixedGravityVector;
    //float fixedGravitySpeed;

    public List<Planet> gravityPlanets = new List<Planet>();

    //��ũ��Ʈ��
    public Planet nearestPlanet;
    public PolygonCollider2D nearestCollider;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentGravityForce = GameManager.Instance.worldGravity;
    }


    protected virtual void FixedUpdate()
    {
        //���� ������ �ִ� �༺ üũ
        if (GetNearestPlanet(out Planet planet, out Vector2 point))
        {
            nearestPoint = point;
            nearestCollider = planet.polyCollider;
        }

        if (nearestPlanet != planet)       //Planet�� �ٲ�� �ѹ��� �ߵ��Ѵ�.
        {
            ChangePlanet(planet);
        }

        if (nearestPlanet == null) return;
        if (!activate) return;

        GravityFunction();
    }

    [SerializeField] const float k = 0.05f; //���
    protected float lerpF;
    protected virtual void GravityFunction()
    {
        Vector2 gravVec = nearestPoint - (Vector2)transform.position;
        //�Ÿ��� �־������� 0�� ������, �Ÿ��� ����������� 1�� �����.
        
        float sqrDistance = gravVec.sqrMagnitude;  //�Ÿ��� ����
        lerpF = 1f / (1f + k * sqrDistance);  //�Ÿ��� Ŭ���� 0�� ������, �Ÿ��� �������� 1�� ����� ��.
        float GForce = Mathf.Lerp(0, currentGravityForce, lerpF);

        rb.AddForce(GForce * nearestPlanet.gravityMultiplier * gravityMultiplier * Time.fixedDeltaTime * gravVec.normalized, ForceMode2D.Force);
    }

    public bool GetNearestPlanet(out Planet planet, out Vector2 point)       //���� ����� Planet ��ũ��Ʈ�� �ش� ��ũ��Ʈ�� GravityForce�� �����´�.
    {
        planet = null;
        point = Vector2.zero;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            Vector2 closestPoint = gravityPlanets[i].polyCollider.ClosestPoint(transform.position);
            float dist = (closestPoint - (Vector2)transform.position).magnitude;

            if (dist < minDist)
            {
                minDist = dist;
                planet = gravityPlanets[i];
                point = closestPoint;
            }
        }
        

        return planet != null ? true : false;
    }
    

    protected virtual void ChangePlanet(Planet planet)
    {
        nearestPlanet = planet;
    }


    #region �߷� �༺ ����Ʈ�� �༺ �߰�
    public void AddToGravityList(Planet planet)
    {
        foreach(var gravity in gravityPlanets)
        {
            if (gravity == planet)
                return;
        }

        gravityPlanets.Add(planet);
    }

    public void RemoveFromGravityList(Planet planet)
    {
        gravityPlanets.Remove(planet);
    }
    #endregion

}
