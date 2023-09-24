using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideProjectile : MonoBehaviour
{

    RaycastHit2D target;

    public  void updateFunction()
    {
        chaseNearEnemy();
    }

    void chaseNearEnemy()
    {
        if(target.collider == null)
        {
            target = Physics2D.CircleCast(transform.position, 3f, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
        }
        else
        {
            Vector2 targetDir = target.transform.position - transform.position;
            targetDir = targetDir.normalized;
            //Vector2 moveDir = Vector2.Lerp(rb.velocity.normalized, targetDir, Time.deltaTime * 5f);
            //rb.velocity = rb.velocity.magnitude * moveDir.normalized;
        }



    }

    /*
    Transform GetNearest()
    {
        Transform result = null;
        float diff = 10;
        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
    */
}
