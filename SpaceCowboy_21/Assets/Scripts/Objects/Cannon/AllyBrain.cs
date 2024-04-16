using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBrain : MonoBehaviour
{
    //���� ����� �� ���� �� ������Ʈ


    public float checkRadius = 10f;


    public bool CheckNearestEnemyBrain(out EnemyBrain eBrain)
    {
        eBrain = null;
        Transform nearestTr = null;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, checkRadius, Vector2.right, 0f, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            float dist = float.MaxValue;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.distance < dist)
                {
                    dist = hit.distance;
                    nearestTr = hit.transform;
                }
            }

            if(nearestTr != null)
                eBrain = nearestTr.GetComponent<EnemyBrain>();

            return true;
        }
        else return false;
    }

}
