using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDictonary : MonoBehaviour
{
    //몬스터 이름 string , 그리고 값 prefab이 들어가있는 Dictonary이다. 
    
    public List<Monster> monsterList = new List<Monster>();
    public Dictionary<string, GameObject> monsDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        for(int i = 0; i< monsterList.Count; i++)
        {
            monsDictionary.Add(monsterList[i].name, monsterList[i].monsterPrefab);
        }
    }
}

[Serializable]
public struct Monster
{
    public string name;
    public GameObject monsterPrefab;

    public Monster(string name, GameObject monsterPrefab)
    {
        this.name = name;
        this.monsterPrefab = monsterPrefab;
    }
}
