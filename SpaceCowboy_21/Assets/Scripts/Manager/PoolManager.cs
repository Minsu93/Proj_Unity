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
        //Ǯ ����Ʈ�� �� ��ŭ Pool���� ����
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
                    // �� ����Ʈ�� null�� �ƴ� ��� Clear()�� ȣ��
                    if (poolLists[j].objPools[i] != null)
                    {
                        poolLists[j].objPools[i].Clear();  // ����Ʈ�� ��� ��� ����
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
    public GameObject[] prefabArray;   //Ǯ�� �� ������Ʈ �������� ����Ʈ
    public List<GameObject>[] objPools;  //������ ������Ʈ ������ Ǯ �迭
    
}