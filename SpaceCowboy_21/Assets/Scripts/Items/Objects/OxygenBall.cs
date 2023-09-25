using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBall : MonoBehaviour
{
    public float oxygenAmount;
    //y축 애니메이션
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OxygenHealth playerHealth = collision.gameObject.GetComponent<OxygenHealth>();
            playerHealth.GetOxygen(oxygenAmount);

            //오브젝트 비활성화
            this.gameObject.SetActive(false);
        }
    }
}
