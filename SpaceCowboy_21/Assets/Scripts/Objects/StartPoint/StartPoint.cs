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
        //애니메이션 시작 
        animator.SetTrigger("spawn");
    }

    //포탈 생성 > 뱉어내기 애니메이션 실행 시 자동으로 실행(trigger)할 메소드
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
    //다시 포탈을 연다. 
    public void OpenPortal(MoveStageDel del)
    {
        transform.position = GameManager.Instance.player.position;
        animator.SetTrigger("portalOpen");
        MoveStage = del;

    }

    //portalOpen -> portalClose 애니메이션. close 될 때 자동으로 제거된다. 
    void RemovePlayer()
    {
        GameManager.Instance.player.gameObject.SetActive(false);
        GameManager.Instance.playerManager.DeactivateDrone();
    }

    //portal Close 애니메이션에서 실행. 다음 스테이지로 이동하는 이벤트를 실행한다. 
    void MoveToNextStage()
    {
        if (MoveStage != null) MoveStage();
        Debug.Log("NextStage");
    }

}
