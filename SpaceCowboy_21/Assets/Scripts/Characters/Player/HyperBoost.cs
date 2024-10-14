using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperBoost : MonoBehaviour
{
    /// ĳ���� ���� -> ĳ���� ���̵��� (���� ���� in)
    /// �������� Ŭ���� -> ĳ���� ���Ƽ� ������ �̵�(���� ���� out)
    /// é�� Ŭ���� -> ĳ���� �Է¿� ���� ������ �ν�Ʈ(���� & �߻�)
    ///
    [SerializeField] float highSpeed = 5.0f;
    [SerializeField] float lowSpeed = 1.0f;
    [SerializeField] float middleHeight = 3.0f;
    [SerializeField] AnimationCurve moveCurve;

    public IEnumerator SuperBoostIn(Vector2 startPoint,  Transform endTr , PlayerManager.ChangeSceneDel del)
    {
        //�÷��̾� ��ġ �ʱ�ȭ
        transform.position= startPoint;
        //��� ��ġ �ʱ�ȭ
        GameManager.Instance.playerManager.MoveAndActivateDrone(startPoint);
        GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(true);

        //�߰� �������� ����
        //Vector2 middlePoint = endTr.position + (endTr.up * middleHeight);
        //PlayerBoostView(BoostPower.Middle);
        //yield return StartCoroutine(MoveAtoBRoutine(startPoint, middlePoint, highSpeed));

        //GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(false);
        //PlayerBoostView(BoostPower.Low);
        //yield return StartCoroutine(MoveAtoBRoutine(middlePoint, endTr.position, lowSpeed));

        PlayerBoostView(BoostPower.Middle);
        yield return StartCoroutine(MoveAtoBRoutine(startPoint, endTr.position, highSpeed));
        GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(false);

        PlayerBoostView(BoostPower.Zero);

        //ĳ���� Ȱ��ȭ
        ActivatePlayer();
        if (del != null) del();
    }
 
    public IEnumerator SuperBoostOut(Transform startTr, Vector2 endPoint, PlayerManager.ChangeSceneDel del)
    {
        //ĳ���� ��Ȱ��ȭ
        DeactivatePlayer();

        //Vector2 middlePoint = startTr.position + (startTr.up * middleHeight);
        //PlayerBoostView(BoostPower.Low);
        //yield return StartCoroutine(MoveAtoBRoutine(startTr.position, middlePoint, lowSpeed));

        //PlayerBoostView(BoostPower.Middle);
        //GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(true);
        //yield return StartCoroutine(MoveAtoBRoutine(middlePoint, endPoint, highSpeed));
        
        PlayerBoostView(BoostPower.Middle);
        GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(true);
        yield return StartCoroutine(MoveAtoBRoutine(startTr.position, endPoint, highSpeed));

        //��� ��Ȱ��ȭ
        GameManager.Instance.playerManager.DeactivateDrone();
        GameManager.Instance.playerManager.playerBehavior.ShowBoostEffect(false);

        if (del != null) del();

    }

    public void HyperBoostReady()
    {

    }

    public void HyperBoostGo()
    {

    }

    delegate void BoostOverDel();
    IEnumerator MoveAtoBRoutine(Vector2 Apos, Vector2 Bpos, float speed)
    {
        float time = 0;
        float dist = Vector2.Distance(Apos, Bpos);
        float duration = dist / speed;
        Vector3 vectorToTarget = (Bpos - Apos).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);


        while (time < duration)
        {
            time += Time.deltaTime;
            //�̵�
            transform.position = Vector2.Lerp(Apos, Bpos, moveCurve.Evaluate(time / duration));
            //ȸ��
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100);

            yield return null;
        } 
    }

    #region BoostOverEvents

    void ActivatePlayer()
    {
        //GameManager.Instance.cameraManager.StartCameraFollow();
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(true);
    }

    void DeactivatePlayer()
    {
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(false);


    }

    enum BoostPower { Zero, Low, Middle, High }
    void PlayerBoostView(BoostPower power)
    {
        switch (power)
        {
            case BoostPower.Zero:
                break;
            case BoostPower.Low:
                break;

            case BoostPower.Middle:
                break;

            case BoostPower.High:
                break;
        }
    }
    #endregion
}
