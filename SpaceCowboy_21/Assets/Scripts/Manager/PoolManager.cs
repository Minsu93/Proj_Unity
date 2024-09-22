using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : MonoBehaviour
{
    public PoolList[] poolLists;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPools;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetPools;
    }

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        //풀 리스트의 수 만큼 Pool들을 생성
        for(int i = 0; i < poolLists.Length; i++)
        {
            poolLists[i].objPools = new List<GameObject>[poolLists[i].prefabArray.Length];
            for(int j = 0; j < poolLists[i].prefabArray.Length; j++)
            {
                poolLists[i].objPools[j] = new List<GameObject>();
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="poolIndex"> "0 = PlayerProj", "1 = EnemyProj" , "2 = DropItems" , "3 = Enemies" , "4 = StageObjs" , "5 = Drones" </param>
    /// <returns></returns>
    /// 
    public GameObject GetPoolObj(GameObject obj, int poolIndex)
    {
        GameObject select = null;
        int index = 0;

        GameObject[] prefabs = poolLists[poolIndex].prefabArray;   
        List<GameObject>[] objLists = poolLists[poolIndex].objPools;
        
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == obj)
            {
                index = i;
            }
        }

        List<GameObject> targetList = objLists[index];

        //리스트에서 놀고 있는게 있다면 그것을 불러온다
        foreach (GameObject item in targetList)
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
            select = Instantiate(prefabs[index]);
            targetList.Add(select);
        }

        return select;
    }

    public void ResetPools(Scene scene, LoadSceneMode mode)
    {
        //foreach(PoolList pool in poolLists)
        //{
        //    foreach(List<GameObject> objList in pool.objPools)
        //    {
        //        objList.Clear();
                
        //    }
            
        //}

        for(int j = 0; j < poolLists.Length; j++)
        {
            if (poolLists[j].objPools != null)
            {
                for (int i = 0; i < poolLists[j].objPools.Length; i++)
                {
                    // 각 리스트가 null이 아닌 경우 Clear()를 호출
                    if (poolLists[j].objPools[i] != null)
                    {
                        poolLists[j].objPools[i].Clear();  // 리스트의 모든 요소 제거
                    }
                }
            }
        }

        Debug.Log("Reset Pools");
    }
}


[Serializable]
public class PoolList
{
    public string name;
    public GameObject[] prefabArray;   //풀에 들어갈 오브젝트 프리팹의 리스트
    public List<GameObject>[] objPools;  //생성될 오브젝트 각각의 풀 배열
    
}