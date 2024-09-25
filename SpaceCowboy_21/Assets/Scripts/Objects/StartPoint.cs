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
    CircleCollider2D circleColl;
    bool portalActivate;

    private void Awake()
    {
        circleColl = GetComponent<CircleCollider2D>();
        circleColl.enabled = false;
        portalActivate = false;
    }

    private void OnEnable()
    {
        GameManager.Instance.StageEndEvent += OpenPortal;
    }

    private void Start()
    {
        ReadyPlayer();

        //GameManager.Instance.TransitionFadeOut(false);
        //GameManager.Instance.cameraManager.SetStartCamera(CamDist.Back);
        //GameManager.Instance.cameraManager.ZoomCamera(CamDist.Middle, ZoomSpeed.Fast);

        //애니메이션 시작 
        animator.SetTrigger("spawn");
    }

    void ReadyPlayer()
    {
        Vector2 pos = transform.position;
        Quaternion rot = transform.rotation;
        playerObj = GameManager.Instance.SpawnPlayer(pos,rot);
        //shuttleObj = GameManager.Instance.SpawnShuttle(pos, rot);
    }

    //포탈 생성 > 뱉어내기 애니메이션 실행 시 자동으로 실행(trigger)할 메소드
    void StartLauchPlayer()
    {
        launchDir = transform.right;
        playerObj.SetActive(true);
        //shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);


        StartCoroutine(AfterLaunchPlayer());
    }

    //플레이어 활성화 후에 이벤트
    IEnumerator AfterLaunchPlayer()
    {
        yield return null;
        //4초 후 웨이브 시작
        WaveManager.instance.WaveStart();
    }


    //다시 포탈을 연다. 플레이어가 들어오면 다음 스테이지로 이동한다. 
    void OpenPortal()
    {
        animator.SetTrigger("portalOpen");
    }

    void PortalTriggerOn()
    {
        circleColl.enabled = true;
        portalActivate= true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!portalActivate) return;

        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer();
            animator.SetTrigger("portalClose");
        }
    }

    void MoveToNextStage()
    {
        GameManager.Instance.MoveToNextStage();

    }
}
