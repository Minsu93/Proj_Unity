using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterShield : MonoBehaviour
{
    public GameObject shieldObj;
    float shieldTime = 0.2f;
    float timer;

    private void Update()
    {
        if(timer < shieldTime)
        {
            timer += Time.deltaTime;

            if(timer >= shieldTime )
            {
                ShieldOFF();
            }
        }
    }

    //�ǵ� �ѱ�
    public void TryShieldOn()
    {
        shieldObj.SetActive(true);
        timer = 0;
    }

    //�ǵ� ����
    void ShieldOFF()
    {
        shieldObj.SetActive(false);
    }
}
