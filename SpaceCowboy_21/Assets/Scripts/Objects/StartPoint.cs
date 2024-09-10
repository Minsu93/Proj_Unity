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

    //��Ż ���� > ���� �ִϸ��̼� ���� �� �ڵ����� ����(trigger)�� �޼ҵ�
    void StartLauchPlayer()
    {
        Debug.Log("Launch");
        launchDir = transform.right;
        playerObj.SetActive(true);
        shuttleObj.SetActive(true);
        GameManager.Instance.playerManager.playerBehavior.LauchPlayer(launchDir, launchPower);
        //�÷��̾��� �⺻ ���⸦ ������Ų��.
        GameManager.Instance.playerManager.playerWeapon.BackToBaseWeapon();

        StartCoroutine(AfterLaunchPlayer());
    }

    //�÷��̾� Ȱ��ȭ �Ŀ� �̺�Ʈ
    IEnumerator AfterLaunchPlayer()
    {
        //���� UI
        //StartCoroutine(GameManager.Instance.ShowStageStartUI(1f, 3f));

        yield return null;
        //4�� �� ���̺� ����
        WaveManager.instance.WaveStart();
    }
}
