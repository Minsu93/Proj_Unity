using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [SerializeField] float launchPower = 5.0f;
    [SerializeField] Vector2 launchDir;
    GameObject playerObj;
    GameObject shuttleObj;
    [SerializeField] Animator animator;

    private void Start()
    {
        ReadyPlayer();
        //�ִϸ��̼� ���� 
        animator.SetTrigger("spawn");
    }

    void ReadyPlayer()
    {
        Vector2 pos = transform.position;
        Quaternion rot = transform.rotation;
        playerObj = GameManager.Instance.SpawnPlayer(pos,rot);
        shuttleObj = GameManager.Instance.SpawnShuttle(pos, rot);
    }

    //��Ż ���� > ���� �ִϸ��̼� ���� �� �ڵ����� ������ �޼ҵ�
    void StartLauchPlayer()
    {
        Debug.Log("Launch");
        launchDir = transform.right;
        playerObj.SetActive(true);
        shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);
        //�÷��̾��� �⺻ ���⸦ ������Ų��.
        GameManager.Instance.playerManager.playerWeapon.BackToBaseWeapon();

    }
}
