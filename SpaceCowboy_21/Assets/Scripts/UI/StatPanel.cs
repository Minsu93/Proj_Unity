using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    /// <summary>
    /// 체력, 에너지량, 유물이 준비되었는지 확인.
    /// </summary>
    /// 
    public bool activate;

    [Space]

    public GameObject healthPrefab;
    public List<GameObject> healthList = new List<GameObject>();
    public Transform healthLayout;
    int panelHealth;
    int panelMaxHealth;

    [Space] 

    public GameObject energyPrefab;
    public List<GameObject> energyList = new List<GameObject>();
    public Transform energyLayout;
    int panelEnergy;
    int panelMaxEnergy;

    [Space]

    int artifactCost_A;
    int artifactCost_B;
    int artifactCost_C;
    public GameObject artifact_A;
    public GameObject artifact_B;
    public GameObject artifact_C;

    //테스트용
    //public int currHealth;
    //public int maxHealth;

    Health playerHealth;
    PlayerWeapon playerWeapon;

    

    private void Awake()
    {
        var playerTr = GameManager.Instance.player;
        playerHealth = playerTr.GetComponent<Health>();
        playerWeapon = playerTr.GetComponent<PlayerWeapon>();

        artifactCost_A = (int)playerWeapon.weapons[1].PowerCost;
        artifactCost_B = (int)playerWeapon.weapons[2].PowerCost;
        artifactCost_C = (int)playerWeapon.weapons[3].PowerCost;

    }

    private void LateUpdate()
    {
        if (!activate)
        {
            return;
        }

        //최대 체력을 조절
        int maxHealth = (int)playerHealth.maxHealth;
        if (panelMaxHealth != maxHealth)
        {
            ChangeMaxHealth(maxHealth);
        }

        //현재 체력을 조절
        int currHealth = (int)playerHealth.currHealth;
        if (panelHealth != currHealth)
        {
            ChangeCurHealth(currHealth);
        }

        //최대 에너지를 조절
        int maxEnergy = (int)playerWeapon.gunPowerMax;
        if (panelMaxEnergy != maxEnergy)
        {
            ChangeMaxEnergy(maxEnergy);
        }

        //현재 에너지를 조절
        int currEnergy = (int)playerWeapon.curGunPower;
        if (panelEnergy != currEnergy)
        {
            Debug.Log(currEnergy);
            ChangeCurEnergy(currEnergy);
        }

        //현재 에너지에 따라 무기를 활성화
        if (currEnergy >= artifactCost_A)
        {
            if(!artifact_A.activeSelf)
                artifact_A.SetActive(true);
        }
        else
        {
            if(artifact_A.activeSelf)
                artifact_A.SetActive(false);
        }
        //B
        if (currEnergy >= artifactCost_B)
        {
            if (!artifact_B.activeSelf)
                artifact_B.SetActive(true);
        }
        else
        {
            if (artifact_B.activeSelf)
                artifact_B.SetActive(false);
        }
        //C
        if (currEnergy >= artifactCost_C)
        {
            if (!artifact_C.activeSelf)
                artifact_C.SetActive(true);
        }
        else
        {
            if (artifact_C.activeSelf)
                artifact_C.SetActive(false);
        }

    }


    void ChangeMaxHealth(int _maxHealth)
    {
        if (panelMaxHealth > _maxHealth)
        {
            ////최대 체력이 줄어들었으면 줄어든 만큼 리스트에서 제거
            //for(int i = panelMaxHealth - maxHealth; i >0; i--)
            //{
            //    Destroy(healthList[i-1]);
            //    healthList.RemoveAt(i-1);
            //}
            //panelMaxHealth = maxHealth;

            //최대 체력이 줄어들었으면 줄어든 만큼 리스트에서 제거
            for (int i = panelMaxHealth; i > _maxHealth; i--)
            {
                Destroy(healthList[i - 1]);
                healthList.RemoveAt(i - 1);
            }
            panelMaxHealth = _maxHealth;
        }
        else
        {
            ////최대 체력이 늘어났으면 늘어난 만큼 리스트에 추가
            //for (int i = 0; i < maxHealth - panelMaxHealth; i++)
            //{
            //    GameObject health = Instantiate(healthPrefab, healthLayout);
            //    health.SetActive(false);
            //    healthList.Add(health);
            //}
            //panelMaxHealth = maxHealth;

            //최대 체력이 늘어났으면 늘어난 만큼 리스트에 추가
            for (int i = panelMaxHealth; i < _maxHealth; i++)
            {
                GameObject health = Instantiate(healthPrefab, healthLayout);
                health.SetActive(false);
                healthList.Insert(i, health);
            }
            panelMaxHealth = _maxHealth;
        }
    }

    void ChangeCurHealth(int _currHealth)
    {
        if (panelHealth < _currHealth)
        {
            ////현재 체력이 늘어났으면 표시되는 체력을 늘린다
            //for(int i = 0; i < currHealth - panelHealth; i++)
            //{
            //    foreach(var health in healthList)
            //    {
            //        if (!health.activeSelf)
            //        {
            //            health.SetActive(true);
            //            break;
            //        }
            //    }
            //}
            //panelHealth = currHealth;

            //현재 체력이 늘어났으면 표시되는 체력을 늘린다
            for (int i = panelHealth; i < _currHealth; i++)
            {
                healthList[i].SetActive(true);
            }
            panelHealth = _currHealth;
        }
        else
        {
            ////현재 체력이 줄어들었으면 표시되는 체력을 줄인다
            //for(int i = 0; i<panelHealth - currHealth; i++)
            //{
            //    foreach(var health in healthList)
            //    {
            //        if (health.activeSelf)
            //        {
            //            health.SetActive(false);
            //            break;
            //        }
            //    }
            //}
            //panelHealth = currHealth;

            //현재 체력이 줄어들었으면 표시되는 체력을 줄인다
            for (int i = panelHealth; i > _currHealth; i--)
            {
                healthList[i - 1].SetActive(false);
            }
            panelHealth = _currHealth;
        }
    }

    void ChangeMaxEnergy(int _maxEnergy)
    {
        if (panelMaxEnergy > _maxEnergy)
        {
            //최대 체력이 줄어들었으면 줄어든 만큼 리스트에서 제거
            for (int i = panelMaxEnergy; i > _maxEnergy; i--)
            {
                Destroy(energyList[i - 1]);
                energyList.RemoveAt(i - 1);
            }
            panelMaxEnergy = _maxEnergy;
        }
        else
        {
            //최대 체력이 늘어났으면 늘어난 만큼 리스트에 추가
            for (int i = panelMaxEnergy; i < _maxEnergy; i++)
            {
                GameObject energy = Instantiate(energyPrefab, energyLayout);
                energy.SetActive(false);
                energyList.Insert(i, energy);
            }
            panelMaxEnergy = _maxEnergy;
        }
    }

    void ChangeCurEnergy(int _currEnergy)
    {
        if (panelEnergy < _currEnergy)
        {
            //에너지 상승
            for (int i = panelEnergy; i < _currEnergy; i++)
            {
                energyList[i].SetActive(true);
            }
            panelEnergy = _currEnergy;
        }
        else
        {
            //에너지 하강
            for (int i = panelEnergy; i > _currEnergy; i--)
            {
                energyList[i - 1].SetActive(false);
            }
            panelEnergy = _currEnergy;
        }
    }

}
