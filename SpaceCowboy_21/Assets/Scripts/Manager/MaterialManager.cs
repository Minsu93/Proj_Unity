using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public int gold { get; private set; }
    [SerializeField] string goldStr = "gold";
    [SerializeField] int maxGold = 100000;
    TextMeshProUGUI goldText;
    [SerializeField] GameObject goldUI;
    //public int starDust { get; private set; }
    //[SerializeField] string starDustStr = "starDust";
    //[SerializeField] int maxStarDust = 1000;

    private void Awake()
    {
        //GameManager.Instance.materialManager = this;
        GameObject goldCanvas = Instantiate(goldUI,this.transform);
        goldText = goldCanvas.transform.Find("GoldUI/GoldText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        LoadMoney();
    }

    //자원 획득
    public void AddMoney(string str, int amount)
    {
        if(str == goldStr)
        {
            //Debug.Log("Get Gold");
            gold += amount;
            gold = Mathf.Clamp(gold, 0, maxGold);
            goldText.text = gold.ToString();
        }
        //else if (str == starDustStr)
        //{
        //    starDust += amount;
        //    starDust = Mathf.Clamp(starDust, 0, maxStarDust);
        //}
    }

    //자원 소모
    public bool PayMoney(string str, int amount)
    {
        if (str == goldStr)
        {
            if (amount > gold) return false;
            else
            {
                gold -= amount;
                goldText.text = gold.ToString();
                return true;
            }
        }
        //else if (str == starDustStr)
        //{
        //    if (amount > starDust) return false;
        //    else
        //    {
        //        starDust -= amount;
        //        return true;
        //    }
        //}
        else return false;
       
    }



    //플레이어가 가진 재화를 저장한다.
    public void SaveMoney()
    {
        PlayerPrefs.SetInt(goldStr, gold);
        //PlayerPrefs.SetInt(starDustStr, starDust);  
    }

    //저장한 재화를 불러온다.
    void LoadMoney()
    {
        PlayerPrefs.GetInt(goldStr, gold);
        //PlayerPrefs.GetInt (starDustStr, starDust);
        goldText.text = gold.ToString();

    }

    public void ResetMoney()
    {
        PlayerPrefs.SetInt(goldStr, 0);

    }
}


