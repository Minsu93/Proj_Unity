using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    /// <summary>
    /// ü��, ��������, ������ �غ�Ǿ����� Ȯ��.
    /// </summary>
    /// 
    public bool activate;

    [Space]

    public GameObject healthPrefab;
    public List<GameObject> healthList = new List<GameObject>();
    public Transform healthLayout;
    int panelHealth;
    int panelMaxHealth;

    public Image LeftShieldImg;
    public Image RightShieldImg;
    public GameObject shieldPrefab;
    public List<GameObject> shieldList = new List<GameObject>();
    public Transform shieldLayout;
    float panelShield;
    float panelMaxShield;



    PlayerHealth playerHealth;
    PlayerWeapon playerWeapon;

    

    private void Awake()
    {
        var playerTr = GameManager.Instance.player;
        playerHealth = playerTr.GetComponent<PlayerHealth>();
        playerWeapon = playerTr.GetComponent<PlayerWeapon>();


    }

    private void LateUpdate()
    {
        if (!activate)
        {
            return;
        }

        //�ִ� ü���� ����
        int maxHealth = (int)playerHealth.maxHealth;
        if (panelMaxHealth != maxHealth)
        {
            ChangeMaxHealth(maxHealth);
        }

        //���� ü���� ����
        int currHealth = (int)playerHealth.currHealth;
        if (panelHealth != currHealth)
        {
            ChangeCurHealth(currHealth);
        }

        //�ִ� �ǵ�
        float maxShield = playerHealth.maxShield;
        if (panelMaxShield != maxShield)
        {
            ChangeMaxShield(maxShield);
        }

        //���� �ǵ�
        float currShield = playerHealth.currShield;
        if (panelShield != currShield)
        {
            ChangeCurShield(currShield);
        }


        ////�ִ� �������� ����
        //int maxEnergy = (int)playerWeapon.gunPowerMax;
        //if (panelMaxEnergy != maxEnergy)
        //{
        //    ChangeMaxEnergy(maxEnergy);
        //}

        ////���� �������� ����
        //int currEnergy = (int)playerWeapon.curGunPower;
        //if (panelEnergy != currEnergy)
        //{
        //    //Debug.Log(currEnergy);
        //    ChangeCurEnergy(currEnergy);
        //}

        ////���� �������� ���� ���⸦ Ȱ��ȭ
        //if (currEnergy >= artifactCost_A)
        //{
        //    if(!artifact_A.activeSelf)
        //        artifact_A.SetActive(true);
        //}
        //else
        //{
        //    if(artifact_A.activeSelf)
        //        artifact_A.SetActive(false);
        //}
        ////B
        //if (currEnergy >= artifactCost_B)
        //{
        //    if (!artifact_B.activeSelf)
        //        artifact_B.SetActive(true);
        //}
        //else
        //{
        //    if (artifact_B.activeSelf)
        //        artifact_B.SetActive(false);
        //}


        //C
        //if (currEnergy >= artifactCost_C)
        //{
        //    if (!artifact_C.activeSelf)
        //        artifact_C.SetActive(true);
        //}
        //else
        //{
        //    if (artifact_C.activeSelf)
        //        artifact_C.SetActive(false);
        //}

    }


    void ChangeMaxHealth(int _maxHealth)
    {
        if (panelMaxHealth > _maxHealth)
        {
            //�ִ� ü���� �پ������� �پ�� ��ŭ ����Ʈ���� ����
            for (int i = panelMaxHealth; i > _maxHealth; i--)
            {
                Destroy(healthList[i - 1]);
                healthList.RemoveAt(i - 1);
            }
            panelMaxHealth = _maxHealth;
        }
        else
        {
            //�ִ� ü���� �þ���� �þ ��ŭ ����Ʈ�� �߰�
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
            //���� ü���� �þ���� ǥ�õǴ� ü���� �ø���
            for (int i = panelHealth; i < _currHealth; i++)
            {
                healthList[i].SetActive(true);
            }
            panelHealth = _currHealth;
        }
        else
        {
            //���� ü���� �پ������� ǥ�õǴ� ü���� ���δ�
            for (int i = panelHealth; i > _currHealth; i--)
            {
                healthList[i - 1].SetActive(false);
            }
            panelHealth = _currHealth;
        }
    }


    void ChangeMaxShield(float _maxShield)
    {
        panelMaxShield = _maxShield;

        //if (panelMaxShield > _maxShield)
        //{
        //    //�ִ� ü���� �پ������� �پ�� ��ŭ ����Ʈ���� ����
        //    for (int i = panelMaxShield; i > _maxShield; i--)
        //    {
        //        Destroy(shieldList[i - 1]);
        //        shieldList.RemoveAt(i - 1);
        //    }
        //    panelMaxShield = _maxShield;
        //}
        //else
        //{
        //    //�ִ� ü���� �þ���� �þ ��ŭ ����Ʈ�� �߰�
        //    for (int i = panelMaxShield; i < _maxShield; i++)
        //    {
        //        GameObject shield = Instantiate(shieldPrefab, shieldLayout);
        //        shield.SetActive(false);
        //        shieldList.Insert(i, shield);
        //    }
        //    panelMaxShield = _maxShield;
        //}
    }

    void ChangeCurShield(float _currShield)
    {
        panelShield = _currShield;

        LeftShieldImg.fillAmount = panelShield / panelMaxShield;
        RightShieldImg.fillAmount = panelShield / panelMaxShield;

        //if (panelShield < _currShield)
        //{
        //    //���� ü���� �þ���� ǥ�õǴ� ü���� �ø���
        //    for (int i = panelShield; i < _currShield; i++)
        //    {
        //        shieldList[i].SetActive(true);
        //    }
        //    panelShield = _currShield;
        //}
        //else
        //{
        //    //���� ü���� �پ������� ǥ�õǴ� ü���� ���δ�
        //    for (int i = panelShield; i > _currShield; i--)
        //    {
        //        shieldList[i - 1].SetActive(false);
        //    }
        //    panelShield = _currShield;
        //}
    }

    //void ChangeMaxEnergy(int _maxEnergy)
    //{
    //    if (panelMaxEnergy > _maxEnergy)
    //    {
    //        //�ִ� ü���� �پ������� �پ�� ��ŭ ����Ʈ���� ����
    //        for (int i = panelMaxEnergy; i > _maxEnergy; i--)
    //        {
    //            Destroy(energyList[i - 1]);
    //            energyList.RemoveAt(i - 1);
    //        }
    //        panelMaxEnergy = _maxEnergy;
    //    }
    //    else
    //    {
    //        //�ִ� ü���� �þ���� �þ ��ŭ ����Ʈ�� �߰�
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
    //        //������ ���
    //        for (int i = panelEnergy; i < _currEnergy; i++)
    //        {
    //            energyList[i].SetActive(true);
    //        }
    //        panelEnergy = _currEnergy;
    //    }
    //    else
    //    {
    //        //������ �ϰ�
    //        for (int i = panelEnergy; i > _currEnergy; i--)
    //        {
    //            energyList[i - 1].SetActive(false);
    //        }
    //        panelEnergy = _currEnergy;
    //    }
    //}

}
