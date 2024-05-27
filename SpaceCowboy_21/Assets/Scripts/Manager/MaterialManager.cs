using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public int gold { get; private set; }
    [SerializeField] string goldStr = "gold";
    [SerializeField] int maxGold = 100000;
    public int starDust { get; private set; }
    [SerializeField] string starDustStr = "starDust";
    [SerializeField] int maxStarDust = 1000;

    private void Awake()
    {
        GameManager.Instance.materialManager = this;

    }

    private void Start()
    {
        LoadMoney();
    }

    //�ڿ� ȹ��
    public void AddMoney(string str, int amount)
    {
        if(str == goldStr)
        {
            gold += amount;
            gold = Mathf.Clamp(gold, 0, maxGold);
        }
        else if (str == starDustStr)
        {
            starDust += amount;
            starDust = Mathf.Clamp(starDust, 0, maxStarDust);
        }
    }

    //�ڿ� �Ҹ�
    public bool PayMoney(string str, int amount)
    {
        if (str == goldStr)
        {
            if (amount > gold) return false;
            else
            {
                gold -= amount;
                return true;
            }
        }
        else if (str == starDustStr)
        {
            if (amount > starDust) return false;
            else
            {
                starDust -= amount;
                return true;
            }
        }
        else return false;
       
    }



    //�÷��̾ ���� ��ȭ�� �����Ѵ�.
    public void SaveMoney()
    {
        PlayerPrefs.SetInt(goldStr, gold);
        PlayerPrefs.SetInt(starDustStr, starDust);  
    }

    //������ ��ȭ�� �ҷ��´�.
    void LoadMoney()
    {
        PlayerPrefs.GetInt(goldStr, gold);
        PlayerPrefs.GetInt (starDustStr, starDust);
    }
}


