using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bubble_Weapon : SelfCollectable
{
    [SerializeField] private WeaponData weaponData;
    //public event System.Action WeaponConsumeEvent;
    SpriteRenderer spr;
    protected override void Awake()
    {
        base.Awake();
        spr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //action 초기화.
        //WeaponConsumeEvent = null;
        GameManager.Instance.arrowManager.CreateArrow(this.gameObject, 1);

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.Instance.arrowManager.RemoveArrow(this.gameObject, 1);
    }

    protected override bool ConsumeEvent()
    {
        //무기 교체
        //if(WeaponConsumeEvent != null) WeaponConsumeEvent();
        
        return GameManager.Instance.playerManager.ChangeWeapon(weaponData);
    }
    public void SetBubble(WeaponData w_Data)
    {
        this.weaponData = w_Data;
        spr.sprite = w_Data.BubbleIcon;
    }



}
