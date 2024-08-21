using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{
    [SerializeField] 
    List<GameObject> skillList = new List<GameObject>();
    Dictionary<string, GameObject> skillPrefabDictionary = new Dictionary<string, GameObject>();
    bool once = false;



    //스킬 Dictionary 를 등록한다.게임 실행 시 한번만 하면 된다. 
    void SetskillDictionary()
    {
        skillPrefabDictionary.Clear();

        for (int i = 0; i < skillTotal.skillDataList.Count; i++)
        {
            string skillName = skillTotal.skillDataList[i].name;
            GameObject skillPrefab = skillList.Find(item => item.name.Equals(skillName));
            skillPrefabDictionary.Add(skillName, skillPrefab);
        }
    }

    public GameObject GetSkillPrefab(string name)
    {
        if(!skillPrefabDictionary.TryGetValue(name, out GameObject obj))
        {
            Debug.Log("해당 skill이 Dictionary에 없습니다");
        }

        return obj;
    }

    #region Skill 전체 데이터 저장 & 불러오기 
    public SkillTotalData skillTotal = new SkillTotalData();
    public void SaveSkillDictionary()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "skillTotalData.json");
        string str = JsonUtility.ToJson(skillTotal, true);
        File.WriteAllText(path, str);
    }

    /// <summary>
    /// Lobby 상점에 진입시 실행. 
    /// </summary>
    public void LoadSkillDictionary()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "skillTotalData.json");
        string loadJson = File.ReadAllText(path);
        skillTotal = JsonUtility.FromJson<SkillTotalData>(loadJson);

        if(!once)
        {
            once = true;
            SetskillDictionary();
        }
    }
    #endregion

    #region Shuttle에 사용될 Equipped Skill 데이터 저장 & 불러오기

    public void SaveEquippedSkill(List<string> equippedSkills)
    {
        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/skillData.json");
        string str = JsonUtility.ToJson(equippedSkills, true);
        File.WriteAllText(path, str);
    }

    public List<string> LoadEquippedSkill()
    {
        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/skillData.json");
        string loadJson = File.ReadAllText(path);
        return JsonUtility.FromJson<List<string>>(loadJson);
    }
    #endregion
}

[Serializable]
public class SkillData
{
    public bool unlocked;
    public string name;
}
[Serializable]
public class SkillTotalData
{
    public List<SkillData> skillDataList = new List<SkillData>();
}
