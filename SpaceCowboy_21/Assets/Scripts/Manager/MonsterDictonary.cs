using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDictonary : MonoBehaviour
{
    //���� �̸� string , �׸��� �� prefab�� ���ִ� Dictonary�̴�. 
    
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
