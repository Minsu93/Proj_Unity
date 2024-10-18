using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bubble_Weapon : SelfCollectable
{
    [SerializeField] private WeaponData weaponData;
    public event System.Action<WeaponData> WeaponConsumeEvent;
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

    }

    protected override void OnDisable()
    {
        base.OnDisable();

    }

    protected override bool ConsumeEvent()
    {
        //무기 교체
        if(WeaponConsumeEvent != null) WeaponConsumeEvent(weaponData);

        GameManager.Instance.arrowManager.RemoveArrow(this.gameObject, 1);

        return GameManager.Instance.playerManager.ChangeWeapon(weaponData);
    }
    public void SetBubble(WeaponData w_Data)
    {
        this.weaponData = w_Data;
        spr.sprite = w_Data.BubbleIcon;
        GameManager.Instance.arrowManager.CreateArrow(this.gameObject, 1);

    }

    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up * 0.5f, weaponData.name) ;
    #endif
    }


}
