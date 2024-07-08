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
        //���� ��ü
        GameManager.Instance.playerManager.ChangeWeapon(weaponData);

        //������ ��Ȱ��ȭ
        GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);


    }



}
