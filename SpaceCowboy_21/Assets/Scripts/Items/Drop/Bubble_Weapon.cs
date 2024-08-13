using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bubble_Weapon : SelfCollectable
{
    [SerializeField] private WeaponData weaponData;
    public Action WeaponConsumeEvent;
    SpriteRenderer spr;
    protected override void Awake()
    {
        base.Awake();
        spr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        //action 초기화.
        WeaponConsumeEvent = null;
    }
    protected override bool ConsumeEvent()
    {
        //무기 교체
        if(WeaponConsumeEvent != null) WeaponConsumeEvent();
        
        Debug.Log(weaponData.name);
        
        return GameManager.Instance.playerManager.ChangeWeapon(weaponData);
    }
    public void SetBubble(WeaponData w_Data)
    {
        this.weaponData = w_Data;
        spr.sprite = w_Data.BubbleIcon;
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(transform.position + (Vector3.right * 1.1f), weaponData.name);
    }
}
