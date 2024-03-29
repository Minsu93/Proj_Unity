using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neutral_Mine : EB_Neutral
{
    public float mineDamage = 3f;

    //플레이어가 밟으면 터진다. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerBehavior>().DamageEvent(mineDamage);
            DamageEvent(99f);
        }
    }

}
