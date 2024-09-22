using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{
    [SerializeField] 
    List<GameObject> skillList = new List<GameObject>();    //게임에 존재하는 모든 스킬의 Prefab 리스트
    public SkillTotalData skillTotal = new SkillTotalData();    //게임에 존재하는 모든 스킬 해금 여부
    Dictionary<string, GameObject> skillPrefabDictionary = new Dictionary<string, GameObject>();    //현재 착용중인 스킬 프리팹
    Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();   //착용중인 스킬의 아이콘
    Dictionary<string, ShuttleSkill> shuttleSkillDictionary = new Dictionary<string, ShuttleSkill>();//착용중인 스킬의 정보 
    bool once = false;



    //스킬 D로드 시 실행. skillList에서 프리팹을 뽑아 skillPrefabDictionary와 IconList에 넣는다. 
    void SetskillDictionary()
    {
        skillPrefabDictionary.Clear();
        iconDictionary.Clear();

        for (int i = 0; i < skillTotal.skillDataList.Count; i++)
        {
            string skillName = skillTotal.skillDataList[i].name;
            GameObject skillPrefab = skillList.Find(item => item.name.Equals(skillName));
            skillPrefabDictionary.Add(skillName, skillPrefab);
            iconDictionary.Add(skillName, skillPrefab.GetComponent<ShuttleSkill>().fillicon);
            shuttleSkillDictionary.Add(skillName, skillPrefab.GetComponent<ShuttleSkill>());
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

    public Sprite GetSkillIcon(string name)
    {
        if (!iconDictionary.TryGetValue(name, out Sprite icon))
        {
            Debug.Log("해당 skill이 Dictionary에 없습니다");
        }

        return icon;
    }

    public ShuttleSkill GetSkillData(string name)
    {
        if(!shuttleSkillDictionary.TryGetValue(name, out ShuttleSkill skill))
        {
            Debug.Log("해당 skill이 Dictionary에 없습니다");
        }
        return skill;

    }

    #region Skill 전체 데이터 저장 & 불러오기 
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
        EquippedSkill equip = new EquippedSkill();
        equip.skillNames = equippedSkills;

        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/skillData.json");
        string str = JsonUtility.ToJson(equip, true);
        File.WriteAllText(path, str);
    }

    public List<string> LoadEquippedSkill()
    {
        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/skillData.json");
        string loadJson = File.ReadAllText(path);
        EquippedSkill equip =JsonUtility.FromJson<EquippedSkill>(loadJson);
        return equip.skillNames;
    }
    #endregion
}

[Serializable]
//skillTotal에 사용. 전체 스킬의 해금 여부 확인
public class SkillData
{
    public bool unlocked;
    public string name;
}

[Serializable]
//데이터 리스트
public class SkillTotalData
{
    public List<SkillData> skillDataList = new List<SkillData>();
}

[Serializable]
public class EquippedSkill
{
    public List<string> skillNames = new List<string>();
}
