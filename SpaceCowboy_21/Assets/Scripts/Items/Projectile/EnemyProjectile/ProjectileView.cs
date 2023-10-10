using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    public Projectile proj;

    MeshRenderer _renderer;
    MaterialPropertyBlock block;

    float damageSec = 0.1f;
    float checkSec;

    void Start()
    {
        if (proj == null) return;

        proj.ResetProjectile += ResetRenderer;
        proj.ProjectileHitEvent += DamageHit;

        _renderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(block);

    }

    public void ResetRenderer()
    {
        SetColorOriginal();
    }

    public void DamageHit()
    {
        checkSec = damageSec;

        SetColorWhite();
    }

    private void Update()
    {
        if(checkSec > 0)
        {
            checkSec -= Time.deltaTime;
            if(checkSec <=0)
            {
                SetColorOriginal();
            }
        }
    }


    void SetColorWhite()
    {
        int id = Shader.PropertyToID("_Black");
        block.SetColor(id, Color.white);
        _renderer.SetPropertyBlock(block);
    }

    void SetColorOriginal()
    {
        int id = Shader.PropertyToID("_Black");
        block.SetColor(id, Color.black);
        _renderer.SetPropertyBlock(block);
    }

}
