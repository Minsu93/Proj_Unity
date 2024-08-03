using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    ////Ǯ�� ������Ʈ���� ������ ���
    //public GameObject[] playerProjectiles;
    //public GameObject[] enemyProjectiles;
    //public GameObject[] dropItems;
    //public GameObject[] enemies;

    ////������� ������ Ǯ�� ������Ʈ List���� ������ List 
    //List<GameObject>[] playerProjPools;
    //List<GameObject>[] enemyProjPools;
    //List<GameObject>[] dropItemPools;
    //List<GameObject>[] enemyPools;

    
    public PoolList[] poolLists;


    private void Awake()
    {
        Init();

    }

    void Init()
    {
        //�ʱ�ȭ
        //playerProjPools = new List<GameObject>[playerProjectiles.Length];

        //for (int i = 0; i < playerProjPools.Length; i++)
        //{
        //    playerProjPools[i] = new List<GameObject>();
        //}


        //enemyProjPools = new List<GameObject>[enemyProjectiles.Length];

        //for (int index = 0; index < enemyProjectiles.Length; index++)
        //{
        //    enemyProjPools[index] = new List<GameObject>();
        //}


        //dropItemPools = new List<GameObject>[dropItems.Length];
        //for (int t = 0; t < dropItems.Length; t++)
        //{
        //    dropItemPools[t] = new List<GameObject>();
        //}

        //enemyPools = new List<GameObject>[enemies.Length];
        //for (int t = 0; t < enemies.Length; t++)
        //{
        //    enemyPools[t] = new List<GameObject>();
        //}

        //������
        for(int i = 0; i < poolLists.Length; i++)
        {
            poolLists[i].poolObjList = new List<GameObject>[poolLists[i].pool.Length];
            for(int j = 0; j < poolLists[i].pool.Length; j++)
            {
                poolLists[i].poolObjList[j] = new List<GameObject>();
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="poolIndex"> "0 = PlayerProj", "1 = EnemyProj" , "2 = DropItems" , "3 = Enemies" , "4 = StageObjs"  </param>
    /// <returns></returns>
    public GameObject GetPoolObj(GameObject obj, int poolIndex)
    {
        GameObject select = null;
        int index = 0;

        GameObject[] prefabs = poolLists[poolIndex].pool;
        List<GameObject>[] objLists = poolLists[poolIndex].poolObjList;
        
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == obj)
            {
                index = i;
            }
        }

        List<GameObject> targetList = objLists[index];

        //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
        foreach (GameObject item in targetList)
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
            select = Instantiate(prefabs[index], transform);
            targetList.Add(select);
        }

        return select;
    }


    //public GameObject Get(GameObject proj)
    //{
    //    GameObject select = null;
    //    int index = 0;

    //    for (int i = 0; i < playerProjectiles.Length; i++)
    //    {
    //        if (playerProjectiles[i] == proj)
    //        {
    //            index = i;
    //        }
    //    }

    //    //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
    //    foreach (GameObject item in playerProjPools[index])
    //    {
    //        if (!item.activeSelf)
    //        {
    //            select = item;
    //            select.SetActive(true);
    //            break;
    //        }
    //    }
    //    //������ ���� �����Ѵ�
    //    if (!select)
    //    {
    //        select = Instantiate(playerProjectiles[index], transform);
    //        playerProjPools[index].Add(select);

    //    }

    //    return select;
    //}


    //public GameObject GetEnemyProj(GameObject enemyProj)
    //{
    //    int index = 0;
    //    GameObject select = null;

    //    for (int i  = 0; i < enemyProjectiles.Length; i++)
    //    {
    //       if( enemyProjectiles[i] == enemyProj)
    //        {
    //            index = i;
    //        }
    //    }
        


    //    //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
    //    foreach (GameObject item in enemyProjPools[index])
    //    {
    //        if (!item.activeSelf)
    //        {
    //            select = item;
    //            select.SetActive(true);
    //            break;
    //        }
    //    }
    //    //������ ���� �����Ѵ�
    //    if (!select)
    //    {
    //        select = Instantiate(enemyProjectiles[index], transform);
    //        enemyProjPools[index].Add(select);

    //    }

    //    return select;
    //}



    //public GameObject GetItem(GameObject item)
    //{
    //    int index = 0;
    //    GameObject select = null;

    //    //item�� ���° index���� ã�´�
    //    for (int i = 0; i < dropItems.Length; i++)
    //    {
    //        if (dropItems[i] == item)
    //        {
    //            index = i;
    //        }
    //    }



    //    //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
    //    foreach (GameObject t in dropItemPools[index])
    //    {
    //        if (!t.activeSelf)
    //        {
    //            select = t;
    //            select.SetActive(true);
    //            break;
    //        }
    //    }


    //    //������ ���� �����Ѵ�
    //    if (!select)
    //    {
    //        select = Instantiate(dropItems[index], transform);
    //        dropItemPools[index].Add(select);

    //    }

    //    return select;
    //}



    //public GameObject GetEnemy(GameObject enemy)
    //{
    //    int index = 0;
    //    GameObject select = null;

    //    for (int i = 0; i < enemies.Length; i++)
    //    {
    //        if (enemies[i] == enemy)
    //        {
    //            index = i;
    //        }
    //    }



    //    //����Ʈ���� ��� �ִ°� �ִٸ� �װ��� �ҷ��´�
    //    foreach (GameObject item in enemyPools[index])
    //    {
    //        if (!item.activeSelf)
    //        {
    //            select = item;
    //            select.SetActive(true);
    //            break;
    //        }
    //    }
    //    //������ ���� �����Ѵ�
    //    if (!select)
    //    {
    //        select = Instantiate(enemies[index], transform);
    //        enemyPools[index].Add(select);

    //    }

    //    return select;
    //}
}


[Serializable]
public class PoolList
{
    public string name;
    public GameObject[] pool;
    public List<GameObject>[] poolObjList;
}