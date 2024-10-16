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

    //경험치 리스트 
    [SerializeField] float[] expSheet = new float[5];

    //무기 리스트
    [Serializable]
    private struct BaseWeaponByLevel
    {
        public int level;
        public WeaponData data;

        public BaseWeaponByLevel(int level, WeaponData data)
        {
            this.level = level;
            this.data = data;
        }
    }

    [SerializeField] BaseWeaponByLevel[] baseWeaponByLvs;





    /// <summary>
    /// 초기화
    /// </summary>
    public void InitializeLevel()
    {
        luckyStats[0] = new LuckyStat("Weapon", 0, 0, expSheet[0]);
        luckyStats[1] = new LuckyStat("Drone", 0, 0, expSheet[0]);
        luckyStats[2] = new LuckyStat("Item", 0, 0, expSheet[0]);
    }

    /// <summary>
    /// 경험치 획득. 0 = Weapon, 1 = Drone, 2 = item;
    /// </summary>
    public void GainExp(int index, float exp)
    {
        if (luckyStats[index].level > expSheet.Length - 1) return;

        luckyStats[index].curExp += exp;

        if (luckyStats[index].curExp >= luckyStats[index].maxExp)
        {
            luckyStats[index].level++;
            LevelChange(index, luckyStats[index].level);

            //경험치 변화
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

        //ui업데이트
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

        //ui업데이트
        GameManager.Instance.playerManager.UpdateLuckyLevel(index, luckyStats[index].level, luckyStats[index].curExp, luckyStats[index].maxExp, false);
    }

    /// <summary>
    /// 레벨에 따른 변화
    /// </summary>
    void LevelChange(int index, int level)
    {
        Debug.Log(luckyStats[index].name + " is Level change to : " + level.ToString());

        switch(index)
        {
            case 0:
                //1. weapon은 기본 무기 변화, 드롭 무기 세팅 변화
                WeaponLevel(level);
                break;
            case 1:
                //2. drone은 드롭 드론 세팅 변화
                break;
            case 2:
                //3. item은 생성되는 아이템 세팅 변화, 쿨타임 변화? >> 조금 애매...
                break;
        }
    }
    
    void WeaponLevel(int level)
    {
        foreach(var weaponByLevel in baseWeaponByLvs)
        {
            if(weaponByLevel.level == level)
            {
                GameManager.Instance.playerManager.ChangeBaseWeapon(weaponByLevel.data);
                break;
            }
        }
    }
}

