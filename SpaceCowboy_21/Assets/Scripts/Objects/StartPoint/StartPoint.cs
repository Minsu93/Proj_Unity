using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [SerializeField] float launchPower = 5.0f;
    [SerializeField] Vector2 launchDir;
    [SerializeField] Animator animator;



    public void SpawnPlayer()
    {
        //�ִϸ��̼� ���� 
        animator.SetTrigger("spawn");
    }

    //��Ż ���� > ���� �ִϸ��̼� ���� �� �ڵ����� ����(trigger)�� �޼ҵ�
    void StartLauchPlayer()
    {
        launchDir = transform.right;
        GameManager.Instance.player.position = transform.position;
        GameManager.Instance.player.gameObject.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);
        GameManager.Instance.playerManager.MoveAndActivateDrone(transform.position);
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(true);

    }

    public delegate void MoveStageDel();
    public MoveStageDel MoveStage;
    //�ٽ� ��Ż�� ����. 
    public void OpenPortal(MoveStageDel del)
    {
        transform.position = GameManager.Instance.player.position;
        animator.SetTrigger("portalOpen");
        MoveStage = del;

    }

    //portalOpen -> portalClose �ִϸ��̼�. close �� �� �ڵ����� ���ŵȴ�. 
    void RemovePlayer()
    {
        GameManager.Instance.player.gameObject.SetActive(false);
        GameManager.Instance.playerManager.DeactivateDrone();
    }

    //portal Close �ִϸ��̼ǿ��� ����. ���� ���������� �̵��ϴ� �̺�Ʈ�� �����Ѵ�. 
    void MoveToNextStage()
    {
        if (MoveStage != null) MoveStage();
        Debug.Log("NextStage");
    }

}
