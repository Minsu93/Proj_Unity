using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactBombSlot : MonoBehaviour
{
    /// <summary>
    /// 투척 위성 기술 슬롯.
    /// 작은 캡슐을 던진 후, 도착 지점에서 터지면서 캡슐이 활성화된다. 
    /// </summary>
    /// 

    public GameObject usingBomb { get; set; } //플레이어가 사용할 투척 위성 >> orb
    public float sateCool; //투척 후 회복 쿨타임. 
    public int maxSate;    //투척 최대 개수. 일반적으로는 1. 

    float coolTimer;
    int curSate;

    [SerializeField] private GameObject throwCapsulePrefab; //던지기 캡슐의 프리팹
    [SerializeField] private AnimationCurve throwCurve;
    [SerializeField] private float throwTravelTime = 0.5f;  //던질 때 이동 시간
    [SerializeField] private float throwDistance = 3.0f;    //던지는 거리


    public void ChangeBomb(GameObject bomb)
    {
        usingBomb = bomb;
    }

    public void ThrowBomb(Vector2 pos, Vector2 dir)
    {
        if (curSate == 0) return;

        curSate--;

        //타겟 위치
        Vector2 targetPoint = pos + (dir * throwDistance);

        // 폭죽 프리팹을 랜덤한 위치에 생성합니다.
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

        // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
        while (time <= throwTravelTime)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, throwCurve.Evaluate(time / throwTravelTime));
            firework.transform.position = pos;
            yield return null;
        }

        // 오브 생성.
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
