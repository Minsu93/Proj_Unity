using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxygen : MonoBehaviour
{
    ///��� ��ũ
    ///���ֿ� �����ų�, Ȥ�� �ٸ� ������ �طο� ��� �༺�� ���� �� ��Ұ� �����ϱ� �����Ѵ�. 
    ///�༺ �ֺ��� �����ϸ� �ٽ� �����ӵ��� ��������. 
    ///

    public float oxygenMax = 10f;   //�ִ� ��ҷ�. ���� ���׷��̵� ����
    public float currOxygen { get; private set; }
    public float decreseSpeed = 1f;
    public float increseSpeed = 3f;
    public bool oxygenDepleted { get; set; }    //��Ұ� ������
    public bool activate = true;

    PlayerBehavior playerBehavior;

    private void Awake()
    {
        currOxygen = oxygenMax;
        playerBehavior = GetComponent<PlayerBehavior>();    
    }

    private void Update()
    {
        if (!activate)
            return;
        if (playerBehavior.onSpace)
        {
            //��Ұ� ��������.
            if(currOxygen > 0)
            {
                currOxygen -= Time.deltaTime * decreseSpeed;

                if(currOxygen <= 0)
                {
                    //��� �� �� ���� �̺�Ʈ

                }
            }
            
        }
        else
        {
            //��Ұ� �����Ѵ�
            if(currOxygen < oxygenMax)
            {
                currOxygen += Time.deltaTime * increseSpeed;
                if(currOxygen >= oxygenMax)
                {
                    currOxygen = oxygenMax;
                }
            }
        }
    }


}
