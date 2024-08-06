using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// ��Ÿ�Ӹ��� ����ID�� ������ �������� ������Ų��.
/// </summary>
public class Gobj_WeaponVending : GoldObject
{
    [SerializeField] GameObject weaponBubblePrefab;

    [SerializeField] float generateCooltime = 30.0f;
    [SerializeField] GameObject circleObj;
    [SerializeField] Image circleImage;
    float timer;
    bool activateGenerate;

    protected override void Awake()
    {
        base.Awake();
        circleObj.SetActive(false);
    }

    private void Update()
    {
        //������ ���� ����
        if (activateGenerate)
        {
            if(timer < generateCooltime)
            {
                timer += Time.deltaTime;
                circleImage.fillAmount = timer / generateCooltime;
            }
            else
            {
                timer = 0;
                GenerateWeapon();
                GenerationCompelete();
            }
        }
    }

    void GenerateWeapon()
    {
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubblePrefab, 2);
        newOrb.transform.position = transform.position + (transform.up);
        newOrb.transform.rotation = Quaternion.identity;
        if(newOrb.TryGetComponent<Bubble_Weapon>(out Bubble_Weapon bubble))
        {
            bubble.WeaponConsumeEvent += GenerationReactivate;
        }
        
    }

    //������Ʈ�� ������ �־ Ȱ��ȭ
    protected override void ObjectActivate()
    {
        //������Ʈ interaction�� ����
        interactActive = false;

        triggerObj.SetActive(false);
        activateGenerate = true;

        //UI����
        goldTransform.gameObject.SetActive(false);

    }

    //������Ʈ�� ���� ����ϰ� ���� ...��Ȱ��ȭ
    protected override void ObjectDeactivate()
    {
        triggerObj.SetActive(true);
    }

    // ����� ����
    void GenerationReactivate()
    {
        activateGenerate = true;
        circleObj.SetActive(true);

    }

    void GenerationCompelete()
    {
        activateGenerate = false;
        circleObj.SetActive(false);
    }
}
