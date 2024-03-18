using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMagnify : MonoBehaviour
{
    public float magnifyFOV;
    public float defaultFOV;
    public float changeSpd = 1f;
    public BoxCollider2D boxCollider;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CameraManager.instance.ChangeCamera(magnifyFOV, changeSpd);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CameraManager.instance.ChangeCamera(defaultFOV, changeSpd);
    }

    private void OnDrawGizmos()
    {
        if(boxCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);

        }
    }
}
