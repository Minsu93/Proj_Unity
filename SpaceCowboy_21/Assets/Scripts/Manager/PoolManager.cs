using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //풀링 오브젝트들을 보관할 장소
    public GameObject[] playerProjectiles;
    public GameObject[] enemyProjectiles;

    //만들어진 각각의 풀링 오브젝트들을 보관할 List 
    List<GameObject>[] playerProjPools;
    List<GameObject>[] enemyProjPools;


    private void Awake()
    {
        playerProjPools = new List<GameObject>[playerProjectiles.Length];

        for(int i = 0; i < playerProjPools.Length; i++)
        {
            //초기화
            playerProjPools[i] = new List<GameObject>();
        }

        enemyProjPools = new List<GameObject>[enemyProjectiles.Length];

        for(int index = 0; index < enemyProjectiles.Length; index++)
        {
            enemyProjPools[index] = new List<GameObject>();
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

        //리스트에서 놀고 있는게 있다면 그것을 불러온다
        foreach (GameObject item in playerProjPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        //없으면 새로 생성한다
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
        


        //리스트에서 놀고 있는게 있다면 그것을 불러온다
        foreach (GameObject item in enemyProjPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        //없으면 새로 생성한다
        if (!select)
        {
            select = Instantiate(enemyProjectiles[index], transform);
            enemyProjPools[index].Add(select);

        }

        return select;
    }
}
