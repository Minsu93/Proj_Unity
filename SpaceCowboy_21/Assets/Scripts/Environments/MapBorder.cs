using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBorder : MonoBehaviour
{
    public float width;
    public float height;
    PolygonCollider2D polyColl;


    //플레이어가 오브젝트를 빠져나가면
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //이동 위치 계산
            Vector2 playerPos = collision.transform.position;
            float x = playerPos.x;
            float y = playerPos.y;

            

            if (x > transform.position.x + width * 0.5f)
            {
                x = transform.position.x - width * 0.5f;
            }
            else if (x < transform.position.x - width * 0.5f)
            {
                x = transform.position.x + width * 0.5f;
            }

            if (y > transform.position.y + height * 0.5f)
            {
                y = transform.position.y - height * 0.5f;
            }
            else if (y < transform.position.y - height * 0.5f)
            {
                y = transform.position.y + height * 0.5f;
            }

            Vector2 movePos = new Vector2(x, y);

            TeleportPlayer(movePos);
        }
    }

    void TeleportPlayer(Vector2 movePos)
    {
        StartCoroutine(NextFrameRoutine(movePos));
    }

    IEnumerator NextFrameRoutine(Vector2 movePos)
    {
        //TeleportStart 이벤트
        GameManager.Instance.PlayerIsTeleport(true);
        //카메라 off
        GameManager.Instance.cameraManager.SetActiveVirtualCam(false);

        yield return new WaitForFixedUpdate();

        GameManager.Instance.player.position = movePos;
        Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
        GameManager.Instance.cameraManager.MoveCamera(movePos, halfSize, transform.position);

        yield return new WaitForFixedUpdate();

        //카메라 on
        GameManager.Instance.cameraManager.SetActiveVirtualCam(true);
        //TeleportEnd이벤트
        GameManager.Instance.PlayerIsTeleport(false);
    }


    [ContextMenu("Set Border")]
    void SetBorder()
    {
        polyColl = GetComponent<PolygonCollider2D>();
        if (polyColl == null) return;

        var myPoints = polyColl.points;

        myPoints[0] = new Vector2(width * 0.5f, height * 0.5f);
        myPoints[1] = new Vector2(-width * 0.5f, height * 0.5f);
        myPoints[2] = new Vector2(-width * 0.5f, -height * 0.5f);
        myPoints[3] = new Vector2(width * 0.5f, -height * 0.5f);
        polyColl.points = myPoints;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }
}
