using System.Collections;
using System.Collections.Generic;
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
                //���� ����
                GameObject hpPotion = GameManager.Instance.poolManager.GetPoolObj(hpPotionPrefab, 2);
                hpPotion.transform.position = this.transform.position;

                //�������� �߻��Ѵ�
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
        //������Ʈ interaction�� ����
        interactActive = false;

        //������ ����
        kickActivate = true;
        count = maxCount;
        triggerObj.SetActive(false);
        kickColl.enabled = true;

        //costUI ����
        goldTransform.gameObject.SetActive(false);
    }

    protected override void ObjectDeactivate()
    {
        kickActivate = false;
        triggerObj.SetActive(true);
        kickColl.enabled = false;
    }
}
