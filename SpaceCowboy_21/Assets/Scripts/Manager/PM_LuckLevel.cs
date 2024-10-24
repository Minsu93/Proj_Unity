using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PM_LuckLevel : MonoBehaviour
{
    private struct LuckyStat
    {
        public string name;
        public int level;
        public float curExp;
        public float maxExp;

        public LuckyStat(string name, int level, float curExp, float maxExp)
        {
            this.name = name;
            this.level = level;
            this.curExp = curExp;
            this.maxExp = maxExp;
        }
    }

    LuckyStat[] luckyStats = new LuckyStat[3];

    //����ġ ����Ʈ 
    [SerializeField] float[] expSheet = new float[3];

    ////���� ����Ʈ
    //[Serializable]
    //private struct BaseWeaponByLevel
    //{
    //    public int level;
    //    public WeaponData data;

    //    public BaseWeaponByLevel(int level, WeaponData data)
    //    {
    //        this.level = level;
    //        this.data = data;
    //    }
    //}

    //[SerializeField] BaseWeaponByLevel[] baseWeaponByLvs;

    public static int itemTier = 0;



    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    public void InitializeLevel()
    {
        luckyStats[0] = new LuckyStat("Weapon", 0, 0, expSheet[0]);
        //luckyStats[1] = new LuckyStat("Drone", 0, 0, expSheet[0]);
        //luckyStats[2] = new LuckyStat("Item", 0, 0, expSheet[0]);
    }

    /// <summary>
    /// ����ġ ȹ��. 0 = Weapon, 1 = Drone, 2 = item;
    /// </summary>
    public void GainExp(int index, float exp)
    {
        if (luckyStats[index].level > expSheet.Length - 1) return;

        luckyStats[index].curExp += exp;

        if (luckyStats[index].curExp >= luckyStats[index].maxExp)
        {
            luckyStats[index].level++;
            LevelChange(index, luckyStats[index].level);

            //����ġ ��ȭ
            if (luckyStats[index].level > expSheet.Length - 1)
            {
                luckyStats[index].curExp = 1;
                luckyStats[index].maxExp = 1;
            }
            else
            {
                luckyStats[index].curExp = luckyStats[index].curExp - luckyStats[index].maxExp;
                luckyStats[index].maxExp = expSheet[luckyStats[index].level];
            }
        }

        //ui������Ʈ
        GameManager.Instance.playerManager.UpdateLuckyLevel(index, luckyStats[index].level, luckyStats[index].curExp, luckyStats[index].maxExp, true);

    }

    public void LossExp()
    {
        int index = UnityEngine.Random.Range(0, 3);

        if (luckyStats[index].level == 0)
        {
            luckyStats[index].curExp = 0;
        }
        else
        {
            luckyStats[index].curExp -= expSheet[luckyStats[index].level - 1];
            while (luckyStats[index].curExp < 0)
            {
                luckyStats[index].level--;
                LevelChange(index, luckyStats[index].level);
                luckyStats[index].curExp += expSheet[luckyStats[index].level];
                luckyStats[index].maxExp = expSheet[luckyStats[index].level];

                if (luckyStats[index].level == 0 && luckyStats[index].curExp < 0)
                {
                    luckyStats[index].curExp = 0;
                    luckyStats[index].maxExp = expSheet[0];
                    break;
                }
            }
        }

        //ui������Ʈ
        GameManager.Instance.playerManager.UpdateLuckyLevel(index, luckyStats[index].level, luckyStats[index].curExp, luckyStats[index].maxExp, false);
    }

    //ui �ѱ� �����
    public void GainExpUI(float exp)
    {
        if (luckyStats[0].level > expSheet.Length - 1) return;

        luckyStats[0].curExp += exp;

        if (luckyStats[0].curExp >= luckyStats[0].maxExp)
        {
            luckyStats[0].level++;
            LevelChange(0, luckyStats[0].level);

            //����ġ ��ȭ
            if (luckyStats[0].level > expSheet.Length - 1)
            {
                luckyStats[0].curExp = 1;
                luckyStats[0].maxExp = 1;
            }
            else
            {
                luckyStats[0].curExp = luckyStats[0].curExp - luckyStats[0].maxExp;
                luckyStats[0].maxExp = expSheet[luckyStats[0].level];
            }
        }

        //ui������Ʈ
        GameManager.Instance.playerManager.UpdateLuckyLevel(0, luckyStats[0].level, luckyStats[0].curExp, luckyStats[0].maxExp, true);
    }
    public void LossExpUI(float exp)
    {

        if (luckyStats[0].level == 0)
        {
            luckyStats[0].curExp = 0;
        }
        else
        {
            luckyStats[0].curExp -= exp;
            while (luckyStats[0].curExp < 0)
            {
                luckyStats[0].level--;
                LevelChange(0, luckyStats[0].level);
                luckyStats[0].curExp += expSheet[luckyStats[0].level];
                luckyStats[0].maxExp = expSheet[luckyStats[0].level];

                if (luckyStats[0].level == 0 && luckyStats[0].curExp < 0)
                {
                    luckyStats[0].curExp = 0;
                    luckyStats[0].maxExp = expSheet[0];
                    break;
                }
            }
        }

        //ui������Ʈ
        GameManager.Instance.playerManager.UpdateLuckyLevel(0, luckyStats[0].level, luckyStats[0].curExp, luckyStats[0].maxExp, false);
    }

    /// <summary>
    /// ������ ���� ��ȭ
    /// </summary>
    void LevelChange(int index, int level)
    {
        Debug.Log(luckyStats[index].name + " is Level change to : " + level.ToString());
        ChangeItemTierByLevel(level);

        switch (index)
        {
            case 0:
                //1. weapon�� �⺻ ���� ��ȭ, ��� ���� ���� ��ȭ
                //BaseWeaponChangeByLevel(level);
                break;
            case 1:
                //2. drone�� ��� ��� ���� ��ȭ
                break;
            case 2:
                //3. item�� �����Ǵ� ������ ���� ��ȭ, ��Ÿ�� ��ȭ? >> ���� �ָ�...
                break;
        }
    }
    
    //void BaseWeaponChangeByLevel(int level)
    //{
    //    foreach(var weaponByLevel in baseWeaponByLvs)
    //    {
    //        if(weaponByLevel.level == level)
    //        {
    //            GameManager.Instance.playerManager.ChangeBaseWeapon(weaponByLevel.data);
    //            break;
    //        }
    //    }
    //}

    void ChangeItemTierByLevel(int level)
    {
        switch (level)
        {
            case 0:
                itemTier = 0;
                break;
            case 1:
                itemTier = 1;
                break;
            case 2:
                itemTier = 2;
                break;
        }
        GameManager.Instance.playerManager.ChangeTier();
    }
}

