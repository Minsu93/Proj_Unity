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



    //��ų Dictionary �� ����Ѵ�.���� ���� �� �ѹ��� �ϸ� �ȴ�. 
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
            Debug.Log("�ش� skill�� Dictionary�� �����ϴ�");
        }

        return obj;
    }

    #region Skill ��ü ������ ���� & �ҷ����� 
    public SkillTotalData skillTotal = new SkillTotalData();
    public void SaveSkillDictionary()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "skillTotalData.json");
        string str = JsonUtility.ToJson(skillTotal, true);
        File.WriteAllText(path, str);
    }

    /// <summary>
    /// Lobby ������ ���Խ� ����. 
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

    #region Shuttle�� ���� Equipped Skill ������ ���� & �ҷ�����

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
