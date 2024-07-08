using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : InteractableOBJ
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private ParticleSystem consumeEffect;


    public override void InteractAction()
    {
        //무기 교체
        GameManager.Instance.playerManager.ChangeWeapon(weaponData);

        //아이템 비활성화
        GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);


    }



}
