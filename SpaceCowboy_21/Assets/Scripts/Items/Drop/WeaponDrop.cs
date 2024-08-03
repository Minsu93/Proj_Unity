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
        //아이템을 처음 먹었을 때 작동.
        //RootItem();

        //무기 교체
        GameManager.Instance.playerManager.ChangeWeapon(weaponData);

        //아이템 비활성화
        GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);


    }

    //아이템을 처음 먹었을 때 상태가 1(미발견)인 경우, Available 로 변경.
    //public void RootItem()
    //{
    //    if(GameManager.Instance.techDocument.GetItemState(weaponData.ItemID) == (int)ItemStateName.Unlocked)
    //    {
    //        GameManager.Instance.techDocument.SetItemState(weaponData.ItemID, ItemStateName.Available);
    //    }
    //}


}
