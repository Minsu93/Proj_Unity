using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBorder : MonoBehaviour
{
    public float width;
    public float height;
    PolygonCollider2D polyColl;


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.PlayerIsTeleport();
            GameManager.Instance.cameraManager.ActiveVirtualCam(false);

            Vector2 playerPos = collision.transform.position;
            float x = playerPos.x;
            float y = playerPos.y;

            if (x > width * 0.5f)
            {
                x = -width * 0.5f;
            }
            else if (x < -width * 0.5f)
            {
                x = width * 0.5f;
            }

            if (y > height * 0.5f)
            {
                y = -height * 0.5f;
            }
            else if (y < -height * 0.5f)
            {
                y = height * 0.5f;
            }

            Vector2 movePos = new Vector2(x, y);
            StartCoroutine(NextFrameRoutine(movePos));

        }
    }

    IEnumerator NextFrameRoutine(Vector2 movePos)
    {
        yield return null;

        GameManager.Instance.player.position = movePos;

        Vector2 limitVec = new Vector2(width * 0.5f, height * 0.5f);
        GameManager.Instance.cameraManager.MoveCamera(movePos, limitVec);
        GameManager.Instance.PlayerIsTeleport();
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
