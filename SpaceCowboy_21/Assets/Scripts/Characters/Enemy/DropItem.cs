using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private void Awake()
    {
        EnemyAction enemyAction = GetComponent<EnemyAction>();
        if (enemyAction != null)
            enemyAction.EnemyDieEvent += GenerateItem;
    }

    public void GenerateItem()
    {
        GameManager.Instance.dropManager.GenerateDrops(this.transform);

        //popperGenerate
        GameManager.Instance.popperManager.CreatePopper(this.transform);
    }

}



