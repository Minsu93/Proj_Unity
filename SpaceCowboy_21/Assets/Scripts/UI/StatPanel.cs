using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //�׽�Ʈ��
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

        //�ִ� �������� ����
        int maxEnergy = (int)playerWeapon.gunPowerMax;
        if (panelMaxEnergy != maxEnergy)
        {
            ChangeMaxEnergy(maxEnergy);
        }

        //���� �������� ����
        int currEnergy = (int)playerWeapon.curGunPower;
        if (panelEnergy != currEnergy)
        {
            Debug.Log(currEnergy);
            ChangeCurEnergy(currEnergy);
        }

        //���� �������� ���� ���⸦ Ȱ��ȭ
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
            ////�ִ� ü���� �پ������� �پ�� ��ŭ ����Ʈ���� ����
            //for(int i = panelMaxHealth - maxHealth; i >0; i--)
            //{
            //    Destroy(healthList[i-1]);
            //    healthList.RemoveAt(i-1);
            //}
            //panelMaxHealth = maxHealth;

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
            ////�ִ� ü���� �þ���� �þ ��ŭ ����Ʈ�� �߰�
            //for (int i = 0; i < maxHealth - panelMaxHealth; i++)
            //{
            //    GameObject health = Instantiate(healthPrefab, healthLayout);
            //    health.SetActive(false);
            //    healthList.Add(health);
            //}
            //panelMaxHealth = maxHealth;

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
            ////���� ü���� �þ���� ǥ�õǴ� ü���� �ø���
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

            //���� ü���� �þ���� ǥ�õǴ� ü���� �ø���
            for (int i = panelHealth; i < _currHealth; i++)
            {
                healthList[i].SetActive(true);
            }
            panelHealth = _currHealth;
        }
        else
        {
            ////���� ü���� �پ������� ǥ�õǴ� ü���� ���δ�
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

            //���� ü���� �پ������� ǥ�õǴ� ü���� ���δ�
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
            //�ִ� ü���� �پ������� �پ�� ��ŭ ����Ʈ���� ����
            for (int i = panelMaxEnergy; i > _maxEnergy; i--)
            {
                Destroy(energyList[i - 1]);
                energyList.RemoveAt(i - 1);
            }
            panelMaxEnergy = _maxEnergy;
        }
        else
        {
            //�ִ� ü���� �þ���� �þ ��ŭ ����Ʈ�� �߰�
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
            //������ ���
            for (int i = panelEnergy; i < _currEnergy; i++)
            {
                energyList[i].SetActive(true);
            }
            panelEnergy = _currEnergy;
        }
        else
        {
            //������ �ϰ�
            for (int i = panelEnergy; i > _currEnergy; i--)
            {
                energyList[i - 1].SetActive(false);
            }
            panelEnergy = _currEnergy;
        }
    }

}
