using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxygen : MonoBehaviour
{
    ///산소 탱크
    ///우주에 나가거나, 혹시 다른 우주의 해로운 대기 행성에 갔을 때 산소가 감소하기 시작한다. 
    ///행성 주변에 도착하면 다시 빠른속도로 차오른다. 
    ///

    public float oxygenMax = 10f;   //최대 산소량. 추후 업그레이드 가능
    public float currOxygen { get; private set; }
    public float decreseSpeed = 1f;
    public float increseSpeed = 3f;
    public bool oxygenDepleted { get; set; }    //산소가 없는지
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
            //산소가 떨어진다.
            if(currOxygen > 0)
            {
                currOxygen -= Time.deltaTime * decreseSpeed;

                if(currOxygen <= 0)
                {
                    //산소 고갈 시 죽음 이벤트

                }
            }
            
        }
        else
        {
            //산소가 증가한다
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
