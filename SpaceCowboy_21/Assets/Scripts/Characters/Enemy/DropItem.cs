using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    //����� ���۵Ǿ��� �� �������� ���� Ȯ��
    [Range(0f, 1f)]
    public float itemChance;
    //�� �ݺ� Ƚ��
    public int dropCount;
    //���� �������� ���
    public GameObject[] items;
    //�������� ��ӵǾ��� ��, ������ �� ��� Ȯ��
    public float[] itemDropRate;

    public void GenerateItem()
    {
        //dropCountȽ����ŭ ������ ����� �õ��Ѵ�
        for(int i = 0; i < dropCount; i++)
        {
            //������ ���� ���θ�
            if (Random.value <= itemChance)
            {
                //�������� �����Ѵ�
     
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
