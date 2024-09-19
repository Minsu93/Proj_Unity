using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popper : MonoBehaviour
{
    [SerializeField] private AnimationCurve fireworkCurve;  //발사 움직임 
    [SerializeField] private float launchTimer = 0.5f;  //발사 이동 시간
    [SerializeField] GameObject weaponBubble;

    public IEnumerator MoveAndExplode(GameObject firework, Vector2 startPos, Vector2 targetPos, WeaponData w_Data)
    {
        float time = 0f;

        // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
            firework.transform.position = pos;
            yield return null;
        }

        // 오브 생성.
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubble, 2);
        newOrb.transform.position = targetPos;
        newOrb.transform.rotation = Quaternion.identity;
        Bubble_Weapon bubble = newOrb.GetComponent<Bubble_Weapon>();
        bubble.SetBubble(w_Data);

        firework.SetActive(false);

    }
}
