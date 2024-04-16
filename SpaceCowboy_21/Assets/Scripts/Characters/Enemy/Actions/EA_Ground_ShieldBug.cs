using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_ShieldBug : EA_Ground
{
    public float explodeRadius = 3f;
    public ParticleSystem explodeParticle;
    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
    }

    protected override void OnDieAction()
    {
        base.OnDieAction();

        StartCoroutine(ShieldExplodeRoutine());
    }


    IEnumerator ShieldExplodeRoutine()
    {
        //1ÃÊÈÄ Æø¹ß
        yield return new WaitForSeconds(1f);

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, explodeRadius, Vector2.right, 0f, LayerMask.GetMask("Player"));
        if(hit.collider != null)
        {
            hit.transform.GetComponent<PlayerHealth>().AnyDamage(99f);
        }

        ParticleHolder.instance.GetParticle(explodeParticle, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
