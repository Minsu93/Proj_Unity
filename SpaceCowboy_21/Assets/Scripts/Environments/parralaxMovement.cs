using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parralaxMovement : MonoBehaviour
{
    public Vector2 moveSpeed = new Vector2(1f, 0f);
    ParralaxBackground parralax;
    Vector2 move;

    //test
    //[SerializeField] float neg = 0f;

    private void OnEnable()
    {
        parralax = GetComponent<ParralaxBackground>();
        if(parralax != null)
            parralax.pMoveAction += ParralaxMove;
    }

    private void OnDisable()
    {
        if(parralax != null)
            parralax.pMoveAction -= ParralaxMove;

    }

    Vector2 ParralaxMove()
    {
        move -= Time.fixedDeltaTime * moveSpeed;
        return move;
    }
}
