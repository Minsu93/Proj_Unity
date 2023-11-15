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
    public string RunButton = "Run";
    Vector2 moveDir = Vector2.zero;

    PlayerBehavior playerBehavior;
    PlayerWeapon playerWeapon;



    private void Awake()
    {
        if (playerBehavior == null) { playerBehavior = GetComponent<PlayerBehavior>(); }
        if (playerWeapon == null) { playerWeapon = GetComponent<PlayerWeapon>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior == null)
            return;
        if (playerWeapon == null)
            return;

        //Map
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.Instance.MapOpen();

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            GameManager.Instance.MapClose();
        }

        // Jump
        if (Input.GetButtonDown(JumpButton))
        {
            playerBehavior.TryJump();
        }
        if(Input.GetButtonUp(JumpButton))
        {
            playerBehavior.StopJump();
        }


        // Run
        if (Input.GetButtonDown(RunButton))
        {
            playerBehavior.TryRun();
        }
        if (Input.GetButtonUp(RunButton))
        {
            playerBehavior.StopRun();
        }


        // Shoot
        if (Input.GetButtonDown(ShootButton))
        {
            playerWeapon.TryStartShoot();
        }

        if (Input.GetButtonUp(ShootButton))
        {
            playerWeapon.TryStopShoot();

        }

        // Weapon Change
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Revolver
            playerWeapon.ChangeWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // bubbleGun
            playerWeapon.ChangeWeapon(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // TripleGun
            playerWeapon.ChangeWeapon(3);

        }


        // Weapon Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerWeapon.TryReload();
        }

      

    }

    private void FixedUpdate()
    {
        if (playerBehavior == null)
            return;
        if (playerWeapon == null)
            return;

        //Vector2 moveRaw = Vector2.zero;

        moveDir.x = Input.GetAxisRaw(HorizontalAxis);
        moveDir.y = Input.GetAxisRaw(VerticalAxis);
        
        if (moveDir.x != 0 || moveDir.y != 0)
        {
            //moveRaw.x = Mathf.Sign(moveDir.x);
            //moveRaw.y = Mathf.Sign(moveDir.y);
            playerBehavior.TryMove(moveDir);
        }
        else
        {
            playerBehavior.TryStop();
        }
    }
}
