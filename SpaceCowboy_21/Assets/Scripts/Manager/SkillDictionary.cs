using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{
    [SerializeField] 
    List<GameObject> skillList = new List<GameObject>();    //���ӿ� �����ϴ� ��� ��ų�� Prefab ����Ʈ
    public SkillTotalData skillTotal = new SkillTotalData();    //���ӿ� �����ϴ� ��� ��ų �ر� ����
    Dictionary<string, GameObject> skillPrefabDictionary = new Dictionary<string, GameObject>();    //���� �������� ��ų ������
    Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();   //�������� ��ų�� ������
    Dictionary<string, ShuttleSkill> shuttleSkillDictionary = new Dictionary<string, ShuttleSkill>();//�������� ��ų�� ���� 
    bool once = false;



    //��ų D�ε� �� ����. skillList���� �������� �̾� skillPrefabDictionary�� IconList�� �ִ´�. 
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
            Debug.Log("�ش� skill�� Dictionary�� �����ϴ�");
        }

        return obj;
    }

    public Sprite GetSkillIcon(string name)
    {
        if (!iconDictionary.TryGetValue(name, out Sprite icon))
        {
            Debug.Log("�ش� skill�� Dictionary�� �����ϴ�");
        }

        return icon;
    }

    public ShuttleSkill GetSkillData(string name)
    {
        if(!shuttleSkillDictionary.TryGetValue(name, out ShuttleSkill skill))
        {
            Debug.Log("�ش� skill�� Dictionary�� �����ϴ�");
        }
        return skill;

    }

    #region Skill ��ü ������ ���� & �ҷ����� 
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
//skillTotal�� ���. ��ü ��ų�� �ر� ���� Ȯ��
public class SkillData
{
    public bool unlocked;
    public string name;
}

[Serializable]
//������ ����Ʈ
public class SkillTotalData
{
    public List<SkillData> skillDataList = new List<SkillData>();
}

[Serializable]
public class EquippedSkill
{
    public List<string> skillNames = new List<string>();
}
