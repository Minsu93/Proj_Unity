using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{
    List<GameObject> objs = new List<GameObject>();
    public Vector2 cubeSize = new Vector2 (1,1);

    BoxCollider2D boxColl;

    private void Awake()
    {
        boxColl = GetComponent<BoxCollider2D>();
        boxColl.size = cubeSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.TryGetComponent<GravityFinder>(out GravityFinder finder))
        {
            finder.FixedGravity(transform.up * -1f);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<GravityFinder>(out GravityFinder finder))
        {
            finder.FixedGravity(Vector2.zero);
        }
    }


    private void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position + transform.up * 0.5f, transform.up * -1f, Color.green);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, (Vector3)cubeSize);
    }
}
