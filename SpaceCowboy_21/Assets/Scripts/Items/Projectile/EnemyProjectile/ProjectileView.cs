using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    public Projectile proj;

    MeshRenderer _renderer;
    MaterialPropertyBlock block;

    

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
        int id = Shader.PropertyToID("_Black");
        block.SetColor(id, Color.black);
        _renderer.SetPropertyBlock(block);
    }

    public void DamageHit()
    {
        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        int id = Shader.PropertyToID("_Black");
        block.SetColor(id,Color.white);
        _renderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(0.1f);

        block.SetColor(id,Color.black); 
        _renderer.SetPropertyBlock (block);  

    }

}
