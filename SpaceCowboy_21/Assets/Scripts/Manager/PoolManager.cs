using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    //Ǯ�� ������Ʈ���� ������ ���
    public GameObject[] playerProjectiles;
    public GameObject[] enemyProjectiles;
    public GameObject[] dropItems;
    public GameObject[] enemies;

    //������� ������ Ǯ�� ������Ʈ List���� ������ List 
    List<GameObject>[] playerProjPools;
    List<GameObject>[] enemyProjPools;
    List<GameObject>[] dropItemPools;
    List<GameObject>[] enemyPools;



    private void Awake()
    {
        instance = this;
                

        //�ʱ�ȭ
        playerProjPools = new List<GameObject>[playerProjectiles.Length];

        for(int i = 0; i < playerProjPools.Length; i++)
        {
            playerProjPools[i] = new List<GameObject>();
        }


        enemyProjPools = new List<GameObject>[enemyProjectiles.Length];

        for(int index = 0; index < enemyProjectiles.Length; index++)
        {
            enemyProjPools[index] = new List<GameObject>();
        }

        
        dropItemPools = new List<GameObject>[dropItems.Length];
        for(int t = 0; t < dropItems.Length; t++)
        {
            dropItemPools[t] = new List<GameObject>();
        }

        enemyPools = new List<GameObject>[enemies.Length];
        for (int t = 0; t < enemies.Length; t++)
        {
            enemyPools[t] = new List<GameObject>();
        }
    }

    public GameObject Get(GameObject proj)
    {
        GameObject select = null;
        int index = 0;

        for (int i = 0; i < playerProjectiles.Length; i++)
        {
            if (playerProjectiles[i] == proj)
            {
                index = i;
                //Debug.LogFormat("Proj index is = {0}", index);
            }
        }

        //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
        foreach (GameObject item in playerProjPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        //������ ���� �����Ѵ�
        if (!select)
        {
            select = Instantiate(playerProjectiles[index], transform);
            playerProjPools[index].Add(select);

        }

        return select;
    }


    public GameObject GetEnemyProj(GameObject enemyProj)
    {
        int index = 0;
        GameObject select = null;

        for (int i  = 0; i < enemyProjectiles.Length; i++)
        {
           if( enemyProjectiles[i] == enemyProj)
            {
                index = i;
            }
        }
        


        //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
        foreach (GameObject item in enemyProjPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        //������ ���� �����Ѵ�
        if (!select)
        {
            select = Instantiate(enemyProjectiles[index], transform);
            enemyProjPools[index].Add(select);

        }

        return select;
    }



    public GameObject GetItem(GameObject item)
    {
        int index = 0;
        GameObject select = null;

        //item�� ���° index���� ã�´�
        for (int i = 0; i < dropItems.Length; i++)
        {
            if (dropItems[i] == item)
            {
                index = i;
            }
        }



        //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
        foreach (GameObject t in dropItemPools[index])
        {
            if (!t.activeSelf)
            {
                select = t;
                select.SetActive(true);
                break;
            }
        }


        //������ ���� �����Ѵ�
        if (!select)
        {
            select = Instantiate(dropItems[index], transform);
            dropItemPools[index].Add(select);

        }

        return select;
    }



    public GameObject GetEnemy(GameObject enemy)
    {
        int index = 0;
        GameObject select = null;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == enemy)
            {
                index = i;
            }
        }



        //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
        foreach (GameObject item in enemyPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        //������ ���� �����Ѵ�
        if (!select)
        {
            select = Instantiate(enemies[index], transform);
            enemyPools[index].Add(select);

        }

        return select;
    }
}
