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

        //�ִϸ��̼� ���� 
        animator.SetTrigger("spawn");
    }

    void ReadyPlayer()
    {
        Vector2 pos = transform.position;
        Quaternion rot = transform.rotation;
        playerObj = GameManager.Instance.SpawnPlayer(pos,rot);
        //shuttleObj = GameManager.Instance.SpawnShuttle(pos, rot);
    }

    //��Ż ���� > ���� �ִϸ��̼� ���� �� �ڵ����� ����(trigger)�� �޼ҵ�
    void StartLauchPlayer()
    {
        launchDir = transform.right;
        playerObj.SetActive(true);
        //shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);


        StartCoroutine(AfterLaunchPlayer());
    }

    //�÷��̾� Ȱ��ȭ �Ŀ� �̺�Ʈ
    IEnumerator AfterLaunchPlayer()
    {
        yield return null;
        //4�� �� ���̺� ����
        WaveManager.instance.WaveStart();
    }


    //�ٽ� ��Ż�� ����. �÷��̾ ������ ���� ���������� �̵��Ѵ�. 
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
