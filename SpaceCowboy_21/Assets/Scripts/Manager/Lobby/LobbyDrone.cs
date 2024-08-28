using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyDrone : MonoBehaviour
{
    [SerializeField] Transform targetTr;
    [SerializeField] Transform viewTr;
    
    [SerializeField] float speed = 3f;
    Vector2 vel;
    bool isRight = true;

    private void Update()
    {
        //플레이어를 따라다닌다. 
        Vector2 targetPos = targetTr.position;
        Vector2 movePos = Vector2.SmoothDamp(transform.position, targetPos, ref vel, speed);
        float dir = movePos.x - transform.position.x;
        transform.position = movePos;

        if(dir > 0 && !isRight)
        {
            isRight = true;
            viewTr.localScale = new Vector3(1, 1, 1);
        }

        else if (dir < 0 && isRight) 
        {
            isRight = false;
            viewTr.localScale = new Vector3(-1, 1, 1);
        }
        
    }
}
