using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerInput : MonoBehaviour
{
    public string HorizontalAxis = "Horizontal";
    public string VerticalAxis = "Vertical";
    public string JumpButton = "Jump";
    public string ShootButton = "Fire1";
    public string DashButton = "Dash";
    public string InteractButton = "Interact";
    public string CancelButton = "Cancel";
    Vector2 moveDir = Vector2.zero;

    PlayerBehavior playerBehavior;

    //¹æ¾î±â test
    public GrabLasso grabLasso;
    public CounterShield counterShield;

    bool moveON = false;
    public bool inputDisabled = false;


    private void Awake()
    {
        if (playerBehavior == null) { playerBehavior = GetComponent<PlayerBehavior>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior == null)
            return;

        if (Input.GetButtonDown(CancelButton))
        {
            GameManager.Instance.InteractCancel();
        }


        if (inputDisabled)
            return;

        //Map
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //CameraManager.instance.MapOpen();
            //¿ùµå¸Ê ¿ÀÇÂ

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            //CameraManager.instance.MapClose();
            //¿ùµå¸Ê Å¬·ÎÁî
        }

        // Jump
        if (Input.GetButtonDown(JumpButton))
        {
            playerBehavior.PrepareJump();
        }
        if(Input.GetButtonUp(JumpButton))
        {
            playerBehavior.TryJump();
        }


        // Shoot
        if (Input.GetButtonDown(ShootButton))
        {
            playerBehavior.StartShoot();

        }

        if (Input.GetButtonUp(ShootButton))
        {
            playerBehavior.StopShoot();

        }

        //Dash
        //if(Input.GetButtonDown(DashButton))
        //{
        //    playerBehavior.TryDash();
        //}

        //CounterShield
        if (Input.GetMouseButtonDown(1))
        {
            counterShield.TryShieldOn();
        }

        //Lasso
        //if (Input.GetMouseButtonDown(1))
        //{
        //    grabLasso.TryThrowLasso();
        //}

        //Interact
        if (Input.GetButtonDown(InteractButton))
        {
            GameManager.Instance.InteractSomething();
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
