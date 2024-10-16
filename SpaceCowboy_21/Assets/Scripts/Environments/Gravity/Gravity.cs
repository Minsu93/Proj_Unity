using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public bool activate = true;
    [SerializeField] protected float gravityMultiplier = 1.0f;

    protected float currentGravityForce;    //월드 중력

    //참조
    public Vector2 nearestPoint { get; set; }


    //Meteor용 고정 방향 중력
    //public bool fixedGravity = false;
    //Planet fixedPlanet;
    //Vector2 fixedPoint;
    //Vector2 fixedGravityVector;
    //float fixedGravitySpeed;

    public List<Planet> gravityPlanets = new List<Planet>();

    //스크립트들
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
        //가장 가까이 있는 행성 체크
        if (GetNearestPlanet(out Planet planet, out Vector2 point))
        {
            nearestPoint = point;
            nearestCollider = planet.polyCollider;
        }

        if (nearestPlanet != planet)       //Planet이 바뀌면 한번만 발동한다.
        {
            ChangePlanet(planet);
        }

        if (nearestPlanet == null) return;
        if (!activate) return;

        GravityFunction();
    }

    [SerializeField] const float k = 0.05f; //상수
    protected float lerpF;
    protected virtual void GravityFunction()
    {
        Vector2 gravVec = nearestPoint - (Vector2)transform.position;
        //거리가 멀어질수록 0에 가깝고, 거리가 가까워질수록 1에 가까움.
        
        float sqrDistance = gravVec.sqrMagnitude;  //거리의 제곱
        lerpF = 1f / (1f + k * sqrDistance);  //거리가 클수록 0에 가깝고, 거리가 가까울수록 1에 가까운 수.
        float GForce = Mathf.Lerp(0, currentGravityForce, lerpF);

        rb.AddForce(GForce * nearestPlanet.gravityMultiplier * gravityMultiplier * Time.fixedDeltaTime * gravVec.normalized, ForceMode2D.Force);
    }

    public bool GetNearestPlanet(out Planet planet, out Vector2 point)       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
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


    #region 중력 행성 리스트에 행성 추가
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
