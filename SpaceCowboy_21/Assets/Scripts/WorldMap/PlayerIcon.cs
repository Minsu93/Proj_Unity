using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    //ÁÂ ¿ì·Î ¿Ó´Ù°¬´Ù ÇÏ±â 
    public static PlayerIcon icon;

    public float angle = 5.0f;
    public float seconds = 2.0f;
    public float moveSpeed = 3.0f;
    float rotation = 0f;
    bool moving;

    MapSelector curMap;

    private void Awake()
    {
        icon = this;
    }

    private void Start()
    {
        StartCoroutine(Repeater());
    }

    IEnumerator Repeater()
    {
        while (true)
        {
            yield return null;
            yield return StartCoroutine(SmoothRotate(-angle, angle, seconds));
            yield return StartCoroutine(SmoothRotate(angle, -angle, seconds));
        }
    }

    IEnumerator SmoothRotate(float from, float to, float duration)
    {
        float time = 0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime / duration;
            rotation = Mathf.Lerp(from, to, Mathf.SmoothStep(0,1,time));
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            yield return null;
        }
        //Debug.Log("Finished");
    }

    public void MoveIcon(MapSelector map)
    {
        if (moving)
            return;

        curMap = map;

        StartCoroutine(SmoothMove(curMap.transform.position));

    }

    IEnumerator SmoothMove(Vector2 targetPos)
    {
        moving = true;

        float time = 0f;

        Vector2 startPos = (Vector2)transform.position;
        float dist = (startPos - targetPos).magnitude;
        float duration = dist / moveSpeed;

        while (time < 1.0f)
        {
            time += Time.deltaTime / duration;
            transform. position = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, time));
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        //ÀÌµ¿ ÈÄ Map Ui ¿ÀÇÂ.
        curMap.OpenMapUI();

        moving = false;
    }
}
