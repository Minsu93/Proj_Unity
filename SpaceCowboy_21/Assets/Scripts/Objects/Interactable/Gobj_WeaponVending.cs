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
    [SerializeField] int generateWeaponId;
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
                GenerateWeapon(generateWeaponId);
            }
        }
    }

    void GenerateWeapon(int id)
    {
        GameObject prefab = GameManager.Instance.techDocument.GetPrefab(id);
        //아이템 생성
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(prefab, 2);
        newOrb.transform.position = transform.position + (transform.up);
        newOrb.transform.rotation = Quaternion.identity;
    }
    protected override void ObjectActivate()
    {
        //오브젝트 interaction을 끈다
        interactActive = false;

        triggerObj.SetActive(false);
        activateGenerate = true;

        //UI조절
        goldTransform.gameObject.SetActive(false);
        circleObj.SetActive(true);

    }

    protected override void ObjectDeactivate()
    {
        triggerObj.SetActive(true);
    }
}
