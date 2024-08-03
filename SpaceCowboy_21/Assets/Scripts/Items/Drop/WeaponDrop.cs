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
        //�������� ó�� �Ծ��� �� �۵�.
        //RootItem();

        //���� ��ü
        GameManager.Instance.playerManager.ChangeWeapon(weaponData);

        //������ ��Ȱ��ȭ
        GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);


    }

    //�������� ó�� �Ծ��� �� ���°� 1(�̹߰�)�� ���, Available �� ����.
    //public void RootItem()
    //{
    //    if(GameManager.Instance.techDocument.GetItemState(weaponData.ItemID) == (int)ItemStateName.Unlocked)
    //    {
    //        GameManager.Instance.techDocument.SetItemState(weaponData.ItemID, ItemStateName.Available);
    //    }
    //}


}
