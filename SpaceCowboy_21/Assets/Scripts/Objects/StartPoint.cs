using Cinemachine;
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

        GameManager.Instance.TransitionFadeOut(false);
        GameManager.Instance.cameraManager.SetStartCamera(CamDist.Back);
        GameManager.Instance.cameraManager.ZoomCamera(CamDist.Middle, ZoomSpeed.Fast);
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

    //포탈 생성 > 뱉어내기 애니메이션 실행 시 자동으로 실행(trigger)할 메소드
    void StartLauchPlayer()
    {
        Debug.Log("Launch");
        launchDir = transform.right;
        playerObj.SetActive(true);
        shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);
        //플레이어의 기본 무기를 장착시킨다.
        GameManager.Instance.playerManager.playerWeapon.BackToBaseWeapon();

        StartCoroutine(AfterLaunchPlayer());
    }

    //플레이어 활성화 후에 이벤트
    IEnumerator AfterLaunchPlayer()
    {
        //시작 UI
        //StartCoroutine(GameManager.Instance.ShowStageStartUI(1f, 3f));

        yield return null;
        //4초 후 웨이브 시작
        WaveManager.instance.WaveStart();
    }
}
