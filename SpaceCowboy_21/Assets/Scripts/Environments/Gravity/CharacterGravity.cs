using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{
    public Vector2 nearestPoint;
    public EdgeCollider2D nearestEdgeCollider;
    [Header("Test")]
    public float maxGravDIst;


    protected override void Update()
    {
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

        if(nearestPlanet != preNearestPlanet)       //Planet이 바뀌면 한번만 발동한다.
        {
            
            SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);
            preNearestPlanet = nearestPlanet;
            currentGravityForce = nearestPlanet.gravityForce;
        }

        Vector2 grav = (nearestPoint - (Vector2)transform.position).normalized;

        float gravDist = (nearestPoint - (Vector2)transform.position).magnitude;        //지면과 가까울수록 더 중력이 강하게 적용
        gravDist = Mathf.Clamp(maxGravDIst / gravDist, 1, maxGravDIst * maxGravDIst);

        rb.AddForce(grav * currentGravityForce * gravityMultiplier * gravDist * Time.deltaTime, ForceMode2D.Force);
        return;
    }


    void GetNearestPoint()
    {
        nearestEdgeCollider = nearestPlanet.GetComponentInChildren<EdgeCollider2D>();
        nearestPoint = nearestEdgeCollider.ClosestPoint(transform.position);
    }
}
