using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popper : MonoBehaviour
{
    [SerializeField] private AnimationCurve fireworkCurve;  //�߻� ������ 
    [SerializeField] private float launchTimer = 0.5f;  //�߻� �̵� �ð�
    [SerializeField] GameObject weaponBubble;
    [SerializeField] GameObject droneBubble;


    public IEnumerator CreateWeaponBubble(Vector2 startPos, Vector2 targetPos, WeaponData w_Data)
    {
        float time = 0f;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
            transform.position = pos;
            yield return null;
        }

        // ���� ����.
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubble, 2);
        newOrb.transform.position = targetPos;
        newOrb.transform.rotation = Quaternion.identity;
        Bubble_Weapon bubble = newOrb.GetComponent<Bubble_Weapon>();
        bubble.SetBubble(w_Data);

        this.gameObject.SetActive(false);
        
    }
    public IEnumerator CreateDroneBubble(Vector2 startPos, Vector2 targetPos, GameObject dronePrefab)
    {
        float time = 0f;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
            transform.position = pos;
            yield return null;
        }

        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(droneBubble, 2);
        newOrb.transform.position = targetPos;
        newOrb.transform.rotation = Quaternion.identity;
        Bubble_Drone bubble = newOrb.GetComponent<Bubble_Drone>();
        bubble.SetDrone(dronePrefab);

        this.gameObject.SetActive(false);

    }
}
