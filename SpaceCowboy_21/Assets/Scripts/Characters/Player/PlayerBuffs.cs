using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    //�ѱ⿡ ����� ���� ����
    public WeaponStats weaponBuffStats { get; private set; }

    //�����ӿ� ����� ���� ����

    //������ ����� ����

    private void Awake()
    {
        weaponBuffStats = new WeaponStats();
    }


    void BuffStart()
    {
        //���� �߰�
    }
    void BuffOver()
    {
        //���� ���� - �ð��� �����ų� Ȥ�� �ٸ� ������ ���� ���
    }

    void ResetBuff()
    {
        //���� ���� �ʱ�ȭ
        GameManager.Instance.playerManager.playerWeapon.ResetBuff();
        //������ ���� �ʱ�ȭ

        //���� ���� �ʱ�ȭ
    }

}
