using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[SelectionBase]
public class PlayerInput : MonoBehaviour
{
    public string HorizontalAxis = "Horizontal";
    public string VerticalAxis = "Vertical";
    public string JumpButton = "Jump";
    public string ShootButton = "Fire1";
    public string SlideButton = "Dash";
    public string InteractButton = "Interact";
    public string CancelButton = "Cancel";
    Vector2 moveDir = Vector2.zero;

    PlayerBehavior playerBehavior;

    bool moveON = false;
    bool shootInputOn = false;
    public bool inputDisabled = false;
    public bool shootDisabled = false;


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior == null)
            return;
        if (!playerBehavior.activate) return;


        //Interact
        if (Input.GetButtonDown(InteractButton))
        {
            GameManager.Instance.playerManager.InteractSomething();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.playerManager.StopInteractSomething();
        }



        if (inputDisabled)
        {
            if (moveON)
            {
                moveON = false;
                playerBehavior.TryPause();
            }
            return;
        }
        

        // Jump & Dash
        if (Input.GetButtonDown(JumpButton))
        {
            playerBehavior.TryJump();
        }
        if (Input.GetButtonUp(JumpButton))
        {
            playerBehavior.TryStopBoost();
        }

        // Dash
        if (Input.GetMouseButtonDown(1))
        {
            playerBehavior.TryDash();
        }

        // Shoot
        if (!shootDisabled && Input.GetButton(ShootButton))
        {
            if (!shootInputOn) shootInputOn = true;
            playerBehavior.TryShoot();
        }
        if (!shootDisabled && Input.GetButtonUp(ShootButton))
        {
            if(shootInputOn)
                playerBehavior.TryResetShoot();
            if (shootInputOn) shootInputOn = false;
        }

        // Slide
        if (Input.GetButtonDown(SlideButton))
        {
            playerBehavior.TrySlide();
        }

        if (Input.GetButtonUp(SlideButton))
        {
            playerBehavior.StopSlide();
        }


        //드론 사용
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Instance.playerManager.UseDrone(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.Instance.playerManager.UseDrone(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.Instance.playerManager.UseDrone(2);
        }

        //움직임
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        //moveDir.x = Input.GetAxisRaw(HorizontalAxis);
        //moveDir.y = Input.GetAxisRaw(VerticalAxis);
        float move = Input.GetAxisRaw(HorizontalAxis);
        if (move > 0)
        {
            playerBehavior.TryMove(true);
            if (!moveON) moveON = true;
        }
        else if (move < 0)
        {
            playerBehavior.TryMove(false);
            if (!moveON) moveON = true;
        }
        else
        {
            if (moveON)
            {
                moveON = false;
                playerBehavior.TryStop();
            }
        }

        //if (moveDir.x != 0 || moveDir.y != 0)
        //{
        //    playerBehavior.TryMove(moveDir);
        //    if(!moveON) moveON = true;
        //}
        //else
        //{
        //    if (moveON)
        //    {
        //        moveON = false;
        //        playerBehavior.TryStop();
        //    }
        //}

    }
}
