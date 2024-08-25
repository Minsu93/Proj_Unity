using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] CameraPos campos;
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] BoxCollider2D movementArea;
    float minX;
    float maxX;

    private void Awake()
    {
        campos.CamPosInitLobby(this.transform);
        minX = movementArea.bounds.min.x + movementArea.transform.position.x;
        maxX = movementArea.bounds.max.x + movementArea.transform.position.x;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            GameManager.Instance.playerManager.InteractSomething();
        }


        float AxisInput = Input.GetAxisRaw("Horizontal");
        if ( Mathf.Abs(AxisInput) != 0 )
        {
            Vector2 curPos = transform.position;
            float moveX = moveSpeed * Time.deltaTime * AxisInput;
            curPos.x += moveX;
            curPos.x = Mathf.Clamp(curPos.x, minX, maxX);
            transform.position = curPos;
            
        }
    }

}
