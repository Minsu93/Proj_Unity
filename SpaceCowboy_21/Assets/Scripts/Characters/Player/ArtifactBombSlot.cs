using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactBombSlot : MonoBehaviour
{
    /// <summary>
    /// ��ô ���� ��� ����.
    /// ���� ĸ���� ���� ��, ���� �������� �����鼭 ĸ���� Ȱ��ȭ�ȴ�. 
    /// </summary>
    /// 

    public GameObject usingBomb { get; set; } //�÷��̾ ����� ��ô ���� >> orb
    public float sateCool; //��ô �� ȸ�� ��Ÿ��. 
    public int maxSate;    //��ô �ִ� ����. �Ϲ������δ� 1. 

    float coolTimer;
    int curSate;

    [SerializeField] private GameObject throwCapsulePrefab; //������ ĸ���� ������
    [SerializeField] private AnimationCurve throwCurve;
    [SerializeField] private float throwTravelTime = 0.5f;  //���� �� �̵� �ð�
    [SerializeField] private float throwDistance = 3.0f;    //������ �Ÿ�


    public void ChangeBomb(GameObject bomb)
    {
        usingBomb = bomb;
    }

    public void ThrowBomb(Vector2 pos, Vector2 dir)
    {
        if (curSate == 0) return;

        curSate--;

        //Ÿ�� ��ġ
        Vector2 targetPoint = pos + (dir * throwDistance);

        // ���� �������� ������ ��ġ�� �����մϴ�.
        GameObject firework = GameManager.Instance.poolManager.GetPoolObj(throwCapsulePrefab, 2);
        firework.transform.position = pos;
        firework.transform.rotation = Quaternion.identity;
        StartCoroutine(MoveAndExplode(firework, targetPoint));
    }


    IEnumerator MoveAndExplode(GameObject firework, Vector2 targetPos)
    {
        Vector2 startPos = firework.transform.position;
        Vector3 targetPosition = targetPos;
        float time = 0f;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= throwTravelTime)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, throwCurve.Evaluate(time / throwTravelTime));
            firework.transform.position = pos;
            yield return null;
        }

        // ���� ����.
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(usingBomb, 2);
        newOrb.transform.position = targetPosition;
        newOrb.transform.rotation = Quaternion.identity;
        firework.SetActive(false);
    }

    private void Update()
    {
        if(curSate < maxSate)
        {
            coolTimer += Time.deltaTime;
            if (coolTimer >= sateCool)
            {
                coolTimer = 0f;
                curSate++;
            }
        }
    }
}
