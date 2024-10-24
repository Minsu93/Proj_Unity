using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjCollecter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            Debug.Log("Object is Collide with PlayerRange");
            if(collision.TryGetComponent<AutoCollectable>(out AutoCollectable autoColl))
            {
                autoColl.StartAutoCollect();
            }
        }
    }
}
