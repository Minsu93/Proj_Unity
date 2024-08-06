using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 쿨타임마다 무기ID를 가지고 아이템을 생성시킨다.
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
        //아이템 생성 시작
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

    //오브젝트에 코인을 넣어서 활성화
    protected override void ObjectActivate()
    {
        //오브젝트 interaction을 끈다
        interactActive = false;

        triggerObj.SetActive(false);
        activateGenerate = true;

        //UI조절
        goldTransform.gameObject.SetActive(false);

    }

    //오브젝트를 전부 사용하고 나서 ...비활성화
    protected override void ObjectDeactivate()
    {
        triggerObj.SetActive(true);
    }

    // 재생성 시작
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
