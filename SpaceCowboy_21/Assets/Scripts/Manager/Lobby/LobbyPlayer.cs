using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] Transform viewObj;
    
    float minX;
    float maxX;
    Rigidbody2D rb;

    private void Start()
    {
        GameManager.Instance.SpawnLobbyPlayer(Vector2.zero, Quaternion.identity);

        //rb = GetComponent<Rigidbody2D>();

        //BoxCollider2D movementArea = GameObject.FindGameObjectWithTag("MovableArea").GetComponent<BoxCollider2D>();
        //minX = movementArea.bounds.min.x + movementArea.transform.position.x;
        //maxX = movementArea.bounds.max.x + movementArea.transform.position.x;


    }

    private void Update()
    {
        //if (Input.GetButtonDown("Interact"))
        //{
        //    GameManager.Instance.playerManager.InteractSomething();
        //}

        //테스트 용도
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.cameraManager.ZoomCamera(CamDist.Back, ZoomSpeed.Fast);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.cameraManager.ZoomCamera(CamDist.Fore, ZoomSpeed.Fast);
        }

    }

    //bool isRight = true;
    //private void FixedUpdate()
    //{
    //    float AxisInput = Input.GetAxisRaw("Horizontal");
    //    if (Mathf.Abs(AxisInput) != 0)
    //    {
    //        Vector2 curPos = transform.position;
    //        float moveX = moveSpeed * Time.deltaTime * AxisInput;
    //        curPos.x += moveX;
    //        curPos.x = Mathf.Clamp(curPos.x, minX, maxX);


    //        rb.MovePosition(curPos);
    //    }

    //    if(!isRight && AxisInput > 0)
    //    {
    //        isRight = true;
    //        viewObj.localScale = new Vector3(1, 1, 1);
    //    }
    //    else if(isRight && AxisInput < 0)
    //    {
    //        isRight = false;
    //        viewObj.localScale = new Vector3(-1, 1, 1);
    //    }
    //}

}
