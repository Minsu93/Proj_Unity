using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBall : MonoBehaviour
{
    public float oxygenAmount;
    //y�� �ִϸ��̼�
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OxygenHealth playerHealth = collision.gameObject.GetComponent<OxygenHealth>();
            playerHealth.GetOxygen(oxygenAmount);

            //������Ʈ ��Ȱ��ȭ
            this.gameObject.SetActive(false);
        }
    }
}
