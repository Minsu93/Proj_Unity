using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    /// <summary>
    /// 체력, 에너지량, 유물이 준비되었는지 확인.
    /// </summary>
    /// 
    public bool activate;

    [Space]

    //public GameObject healthPrefab;
    //public List<GameObject> healthList = new List<GameObject>();
    //public Transform healthLayout;
    //int panelHealth;
    //int panelMaxHealth;

    public Image LeftHealthImg;
    public Image RightHealthImg;

    float panelHealth;
    float panelMaxHealth;

    [Space]

    public Image LeftShieldImg;
    public Image RightShieldImg;

    //public GameObject shieldPrefab;
    //public List<GameObject> shieldList = new List<GameObject>();
    //public Transform shieldLayout;

    float panelShield;
    float panelMaxShield;


    [Space]

    public Image LeftRegenImg;
    public Image RightRegenImg;

    float preShieldRegenTimer;

    PlayerHealth playerHealth;

    

    private void Awake()
    {
        var playerTr = GameManager.Instance.player;
        playerHealth = playerTr.GetComponent<PlayerHealth>();
    }

    private void LateUpdate()
    {
        if (!activate) return;
        if (playerHealth == null) return;

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

        //최대 실드
        float maxShield = playerHealth.maxShield;
        if (panelMaxShield != maxShield)
        {
            ChangeMaxShield(maxShield);
        }

        //현재 실드
        float currShield = playerHealth.currShield;
        if (panelShield != currShield)
        {
            ChangeCurShield(currShield);
        }

        //실드 리젠 게이지. get한 값은 0 ~ 1 사이. 
        float ShieldRegenTimer = playerHealth.dTimer;
        if(ShieldRegenTimer != preShieldRegenTimer)
        {
            ShieldRegen(ShieldRegenTimer);

        }
    }


    //void ChangeMaxHealth(int _maxHealth)
    //{
    //    if (panelMaxHealth > _maxHealth)
    //    {
    //        //최대 체력이 줄어들었으면 줄어든 만큼 리스트에서 제거
    //        for (int i = panelMaxHealth; i > _maxHealth; i--)
    //        {
    //            Destroy(healthList[i - 1]);
    //            healthList.RemoveAt(i - 1);
    //        }
    //        panelMaxHealth = _maxHealth;
    //    }
    //    else
    //    {
    //        //최대 체력이 늘어났으면 늘어난 만큼 리스트에 추가
    //        for (int i = panelMaxHealth; i < _maxHealth; i++)
    //        {
    //            GameObject health = Instantiate(healthPrefab, healthLayout);
    //            health.SetActive(false);
    //            healthList.Insert(i, health);
    //        }
    //        panelMaxHealth = _maxHealth;
    //    }
    //}

    //void ChangeCurHealth(int _currHealth)
    //{
    //    if (panelHealth < _currHealth)
    //    {
    //        //현재 체력이 늘어났으면 표시되는 체력을 늘린다
    //        for (int i = panelHealth; i < _currHealth; i++)
    //        {
    //            healthList[i].SetActive(true);
    //        }
    //        panelHealth = _currHealth;
    //    }
    //    else
    //    {
    //        //현재 체력이 줄어들었으면 표시되는 체력을 줄인다
    //        for (int i = panelHealth; i > _currHealth; i--)
    //        {
    //            healthList[i - 1].SetActive(false);
    //        }
    //        panelHealth = _currHealth;
    //    }
    //}

    void ChangeMaxHealth(float _maxHealth)
    {
        panelMaxHealth = _maxHealth;
    }

    void ChangeCurHealth(float _currHealth)
    {
        panelHealth = _currHealth;

        LeftHealthImg.fillAmount = panelHealth / panelMaxHealth;
        RightHealthImg.fillAmount = panelHealth / panelMaxHealth;
    }

    void ChangeMaxShield(float _maxShield)
    {
        panelMaxShield = _maxShield;
    }

    void ChangeCurShield(float _currShield)
    {
        panelShield = _currShield;

        LeftShieldImg.fillAmount = panelShield / panelMaxShield;
        RightShieldImg.fillAmount = panelShield / panelMaxShield;
    }

    //void ChangeMaxEnergy(int _maxEnergy)
    //{
    //    if (panelMaxEnergy > _maxEnergy)
    //    {
    //        //최대 체력이 줄어들었으면 줄어든 만큼 리스트에서 제거
    //        for (int i = panelMaxEnergy; i > _maxEnergy; i--)
    //        {
    //            Destroy(energyList[i - 1]);
    //            energyList.RemoveAt(i - 1);
    //        }
    //        panelMaxEnergy = _maxEnergy;
    //    }
    //    else
    //    {
    //        //최대 체력이 늘어났으면 늘어난 만큼 리스트에 추가
    //        for (int i = panelMaxEnergy; i < _maxEnergy; i++)
    //        {
    //            GameObject energy = Instantiate(energyPrefab, energyLayout);
    //            energy.SetActive(false);
    //            energyList.Insert(i, energy);
    //        }
    //        panelMaxEnergy = _maxEnergy;
    //    }
    //}

    //void ChangeCurEnergy(int _currEnergy)
    //{
    //    if (panelEnergy < _currEnergy)
    //    {
    //        //에너지 상승
    //        for (int i = panelEnergy; i < _currEnergy; i++)
    //        {
    //            energyList[i].SetActive(true);
    //        }
    //        panelEnergy = _currEnergy;
    //    }
    //    else
    //    {
    //        //에너지 하강
    //        for (int i = panelEnergy; i > _currEnergy; i--)
    //        {
    //            energyList[i - 1].SetActive(false);
    //        }
    //        panelEnergy = _currEnergy;
    //    }
    //}

    void ShieldRegen(float _currTimer)
    {
        preShieldRegenTimer = _currTimer;
        LeftRegenImg.fillAmount = _currTimer;
        RightRegenImg.fillAmount = _currTimer;
    }
}
