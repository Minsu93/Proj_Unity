using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.Progress;

public class Gobj_HPVending : GoldObject, IKickable, ITarget
{
    [SerializeField] bool kickActivate = false;
    [SerializeField] int maxCount = 3;
    [SerializeField] GameObject hpPotionPrefab;
    [SerializeField] float launchPower = 5f;
    int count;

    public Collider2D GetCollider()
    {
        return null;
    }

    public void Kicked(Vector2 hitPos)
    {
        if (kickActivate)
        {
            Debug.Log("Kicked");
            if(count > 0)
            {
                count--;
                //물약 생성
                GameObject hpPotion = GameManager.Instance.poolManager.GetPoolObj(hpPotionPrefab, 2);
                hpPotion.transform.position = this.transform.position;
                HealthDrop hpDrop = hpPotion.GetComponent<HealthDrop>();

                //아이템을 발사한다
                Quaternion randomQuat = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-180f, 180f));
                Vector2 hemisphereDir = randomQuat * (Vector2)transform.up;
                //Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
                float randomPow = UnityEngine.Random.Range(launchPower * 0.5f, launchPower);
                hpDrop.LaunchPotion(hemisphereDir * randomPow);
                //hpPotion.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
            }
            
            if(count == 0)
            {
                ObjectDeactivate();
            }
        }
    }

    protected override void ObjectActivate()
    {
        //오브젝트 interaction을 끈다
        interactActive = false;

        //발차기 가능
        kickActivate = true;
        count = maxCount;
        triggerObj.SetActive(false);
        kickColl.enabled = true;

        //costUI 제거
        goldTransform.gameObject.SetActive(false);
    }

    protected override void ObjectDeactivate()
    {
        kickActivate = false;
        triggerObj.SetActive(true);
        kickColl.enabled = false;
    }
}
