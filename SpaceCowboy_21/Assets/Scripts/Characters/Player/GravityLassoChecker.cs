using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLassoChecker : MonoBehaviour
{
    public GravityLasso lasso { get; set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            if(lasso != null)
            {
                lasso.TriggerByBig();
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            if(lasso != null)
            {
                lasso.TriggerByMedium(collision);
            }
        }
        else if (collision.CompareTag("Item"))
        {
            if(lasso != null)
            {
                lasso.TriggerBySmall();
            }
        }
    }
}
