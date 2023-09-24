using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    //드롭이 시작되었을 때 아이템이 나올 확률
    [Range(0f, 1f)]
    public float itemChance;
    //총 반복 횟수
    public int dropCount;
    //나올 아이템의 목록
    public GameObject[] items;
    //아이템이 드롭되었을 때, 아이템 별 드롭 확률
    public float[] itemDropRate;

    public void GenerateItem()
    {
        //dropCount횟수만큼 아이템 드롭을 시도한다
        for(int i = 0; i < dropCount; i++)
        {
            //아이템 찬스 내부면
            if (Random.value <= itemChance)
            {
                //아이템을 생성한다
     
                GameObject item = Instantiate(items[Choose(itemDropRate)], (Vector2)transform.position, Quaternion.identity);

                Vector2 randomUpDir = (transform.up + (transform.right * Random.Range(-1, 1f))).normalized;
                item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * 2f ,ForceMode2D.Impulse);
            }     
        }
    }

    int Choose(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }



}
