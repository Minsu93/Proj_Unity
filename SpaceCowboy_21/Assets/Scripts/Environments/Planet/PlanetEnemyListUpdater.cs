using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlanetEnemyListUpdater : MonoBehaviour
{
    public Planet planet;


    private void Start()
    {
        //ResetEnemyList();
    }

    //private void Update()
    //{
    //    if (planet == null)
    //        return;

    //    if (Application.isPlaying)
    //        this.enabled = false;
    //    ResetEnemyList();

    //}


    void ResetEnemyList()
    {
        //�ֺ��� �ִ� ������Ʈ�� �˻��Ѵ�
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, planet.gravityRadius, Vector2.right, 0f, LayerMask.GetMask("Enemy"));

        //�� ���� �ִ´�. 
        List<EnemyBrain> enemies = new List<EnemyBrain>();
        foreach (RaycastHit2D hit in hits)
        {
            AttachToPlanet atp = hit.transform.GetComponentInParent<AttachToPlanet>();
            if (atp.currPlanet == planet)
            {
                EnemyBrain eb = hit.transform.GetComponentInParent<EnemyBrain>();
                enemies.Add(eb);
            }

        }

        //planet.enemyList.Clear();
       // planet.enemyList = enemies;
    }
}
