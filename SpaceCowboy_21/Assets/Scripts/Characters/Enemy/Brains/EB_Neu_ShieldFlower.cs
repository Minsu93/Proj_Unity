using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_ShieldFlower : EnemyBrain
{
    public float recoverAmount = 1f;
    public float recoverCool = 0.5f;
    float coolTime;

    [SerializeField] PlayerHealth pHealth;

    protected override void Update()
    {
        if (!activate) return;
        if (pHealth == null) return;

        if(coolTime < recoverCool)
        {
            coolTime += Time.deltaTime;
        }
        else
        {
            pHealth.RecoverShield(recoverAmount);
            coolTime = 0f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pHealth = collision.GetComponent<PlayerHealth>();
            coolTime = 0f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pHealth = null;
        }
    }
    public override void BrainStateChange()
    {
        return;
    }

    protected override void AfterHitEvent()
    {
        return;
    }

    protected override void WhenDieEvent()
    {
        return;
    }



}
