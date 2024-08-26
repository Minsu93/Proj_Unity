using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] BoxCollider2D movementArea;
    float minX;
    float maxX;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameManager.Instance.cameraManager.InitLobbyCam(this.transform);
        minX = movementArea.bounds.min.x + movementArea.transform.position.x;
        maxX = movementArea.bounds.max.x + movementArea.transform.position.x;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            GameManager.Instance.playerManager.InteractSomething();
        }

        //테스트 용도
        if(Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.cameraManager.ZoomCamera(CameraManager.CamDist.Back, CameraManager.ZoomSpeed.Fast);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.cameraManager.ZoomCamera(CameraManager.CamDist.Fore, CameraManager.ZoomSpeed.Slow);
        }


        
    }

    private void FixedUpdate()
    {
        float AxisInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(AxisInput) != 0)
        {
            Vector2 curPos = transform.position;
            float moveX = moveSpeed * Time.deltaTime * AxisInput;
            curPos.x += moveX;
            curPos.x = Mathf.Clamp(curPos.x, minX, maxX);
            

            rb.MovePosition(curPos);
        }
    }

}
