using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{
    [SerializeField] 
    List<GameObject> skillList = new List<GameObject>();
    Dictionary<string, GameObject> skillNameDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        for(int i = 0; i< skillList.Count; i++)
        {
            skillNameDictionary.Add(skillList[i].name, skillList[i]);
        }
    }

    public GameObject GetSkillPrefab(string name)
    {
        if(!skillNameDictionary.TryGetValue(name, out GameObject obj))
        {
            Debug.Log("해당 skill이 Dictionary에 없습니다");
        }

        return obj;
    }
}
