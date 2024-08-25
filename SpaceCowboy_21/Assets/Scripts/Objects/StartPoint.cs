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
        //애니메이션 시작 
        animator.SetTrigger("spawn");
    }

    void ReadyPlayer()
    {
        Vector2 pos = transform.position;
        Quaternion rot = transform.rotation;
        playerObj = GameManager.Instance.SpawnPlayer(pos,rot);
        shuttleObj = GameManager.Instance.SpawnShuttle(pos, rot);
    }

    //포탈 생성 > 뱉어내기 애니메이션 실행 시 자동으로 실행할 메소드
    void StartLauchPlayer()
    {
        Debug.Log("Launch");
        launchDir = transform.right;
        playerObj.SetActive(true);
        shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);
        //플레이어의 기본 무기를 장착시킨다.
        GameManager.Instance.playerManager.playerWeapon.BackToBaseWeapon();

    }
}
