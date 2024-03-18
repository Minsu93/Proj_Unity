using SpaceEnemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    public int dropRepeat;       //�� �ݺ� Ƚ��

    [Range(0f, 1f)]
    public float itemChance;    //�������� ���� Ȯ��
    public DropTable[] dropTable;

    [SerializeField] float launchPowerMin = 3f;
    [SerializeField] float launchPowerMax = 4f;
    private void Awake()
    {
        EnemyAction enemyAction = GetComponent<EnemyAction>();
        if(enemyAction != null )
            enemyAction.EnemyDieEvent += GenerateItem;
    }

    public void GenerateItem()
    {
        //dropCountȽ����ŭ ������ ����� �õ��Ѵ�
        for(int i = 0; i < dropRepeat; i++)
        {
            //������ ���� ���θ�
            if (UnityEngine.Random.value <= itemChance)
            {
                //�������� �����Ѵ�
                GameObject item = PoolManager.instance.GetItem(Choose());
                item.transform.position = transform.position;

                //�������� �߻��Ѵ�
                Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
                float randomPow = UnityEngine.Random.Range(launchPowerMin, launchPowerMax);
                item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
            }     
        }
    }

    GameObject Choose()
    {
        float total = 0;
        int length = dropTable.Length;

        for(int j = 0; j < length; j++)
        {
            total += dropTable[j].dropRate;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < length; i++)
        {
            if (randomPoint < dropTable[i].dropRate)
            {
                return dropTable[i].item;
            }
            else
            {
                randomPoint -= dropTable[i].dropRate;
            }
        }
        return dropTable[length - 1].item;    //randomPoint = total�� ���
    }



}
[System.Serializable]
public struct DropTable
{
    public GameObject item;
    public float dropRate;

    public DropTable(GameObject item, float dropRate)
    {
        this.item = item;
        this.dropRate = dropRate;
    }
}
