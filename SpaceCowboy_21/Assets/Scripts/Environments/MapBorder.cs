using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBorder : MonoBehaviour
{
    public float width;
    public float height;


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Out of Border");
            
            Vector2 playerPos = collision.transform.position;
            float x = playerPos.x;
            float y = playerPos.y;

            Debug.Log("Player Position: " + collision.transform.position);
            Debug.Log("X: " + x + ", Y: " + y);
            Debug.Log("Width: " + width + ", Height: " + height);

            if (x > width * 0.5f )
            {
                x = - width * 0.5f;
            }
            else if(x < -width * 0.5f)
            {
                x = width * 0.5f;
            }

            if(y > height *0.5f )
            {
                y = - height * 0.5f;
            }
            else if (y < -height * 0.5f)
            {
                y = height * 0.5f;
            }

            Debug.Log("X: " + x + ", Y: " + y);
            collision.transform.position = new Vector2(x, y);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }
}
