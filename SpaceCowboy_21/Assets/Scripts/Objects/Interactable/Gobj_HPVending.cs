using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Gobj_HPVending : GoldObject, IKickable
{
    [SerializeField] bool activated = false;
    [SerializeField] int maxCount = 3;
    [SerializeField] GameObject hpPotionPrefab;
    [SerializeField] float launchPower = 5f;
    int count;


    public void Kicked(Vector2 hitPos)
    {
        if (activated)
        {
            if(count > 0)
            {
                count--;
                //물약 생성
                GameObject hpPotion = GameManager.Instance.poolManager.GetPoolObj(hpPotionPrefab, 2);
                hpPotion.transform.position = this.transform.position;

                //아이템을 발사한다
                Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
                float randomPow = UnityEngine.Random.Range(launchPower - 2f, launchPower + 2f);
                hpPotion.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
            }
            
            if(count == 0)
            {
                ObjectDeactivate();
            }
        }
    }

    protected override void ObjectActivate()
    {
        activated = true;
        //coll.enabled = true;
        int count = maxCount;
    }

    protected override void ObjectDeactivate()
    {
        activated = false;
        //coll.enabled = false;
    }
}
