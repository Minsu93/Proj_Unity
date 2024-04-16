using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class ProjectileGravity : Gravity
{
    public float gravTimer = 0.3f;
    float gTimer;

    Projectile projectile;

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponent<Projectile>();

        projectile.ProjectileInitEvent += ResetPlanet;
    }
    protected override void FixedUpdate()
    {
        if (!activate)
            return;

        if (nearestPlanet == null)
            return;

        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        if(gTimer < gravTimer)
        {
            gTimer += Time.fixedDeltaTime;
            
        }
        else
        {
            rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier * Time.deltaTime, ForceMode2D.Force);
        }
    }

    void ResetPlanet()
    {
        gravityPlanets.Clear();
        gTimer = 0f;
        activate = true;
    }

}
