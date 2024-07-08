using SpaceCowboy;
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

        if (inputDisabled)
            return;

        // Jump
        if (Input.GetButtonDown(JumpButton))
        {
            playerBehavior.TrySpeedJump();
        }

        // Shoot
        //if (Input.GetButtonDown(ShootButton))
        //{
        //    playerBehavior.StartShoot();
        //}

        //if (Input.GetButtonUp(ShootButton))
        //{
        //    playerBehavior.StopShoot();
        //}

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
            playerBehavior.TryStopSlide();
        }

        //투척 위성 사용
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    playerBehavior.TryThrowSatellite();
        //}

        ////weaponWheel 사용
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    GameManager.Instance.playerManager.OpenWeaponWheel();
        //}
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    GameManager.Instance.playerManager.CloseWeaponWheel();

        //}
        //if (Input.GetMouseButtonDown(1))    //우클릭 시 취소
        //{
        //    GameManager.Instance.playerManager.CloseWeaponWheel();
        //}



        //Interact
        if (Input.GetButtonDown(InteractButton))
        {
            GameManager.Instance.playerManager.InteractSomething();
        }

    }

    private void FixedUpdate()
    {
        if (playerBehavior == null)
            return;

        if (inputDisabled)
        {
            if (moveON)
            {
                moveON = false;
                playerBehavior.TryStop();
            }
            return;
        }
        //Vector2 moveRaw = Vector2.zero;

        moveDir.x = Input.GetAxisRaw(HorizontalAxis);
        moveDir.y = Input.GetAxisRaw(VerticalAxis);
        
        if (moveDir.x != 0 || moveDir.y != 0)
        {
            playerBehavior.TryMove(moveDir);
            if(!moveON) moveON = true;
        }
        else
        {
            if(moveON)
            {
                moveON = false;
                playerBehavior.TryStop();
            }
        }
    }
}
