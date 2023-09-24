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

    private void OnValidate()
    {
        if(playerBehavior == null) { playerBehavior = GetComponent<PlayerBehavior>(); }
        if(playerWeapon == null) { playerWeapon = GetComponent<PlayerWeapon>(); }
    }

    // Update is called once per frame
    void Update()
    {
        // Jump
        if(Input.GetButtonDown(JumpButton))
        {
            playerBehavior.TryJump();
        }

        // Shoot
        if(Input.GetButtonDown(ShootButton))
        {
            playerBehavior.TryStartShoot();
        }

        if (Input.GetButtonUp(ShootButton))
        {
            playerBehavior.TryStopShoot();

        }

        // Run
        if (Input.GetButtonDown(RunButton))
        {
            playerBehavior.TryRun();
        }

        // Weapon Change
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Revolver
            playerWeapon.ChangeWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Watergun
            playerWeapon.ChangeWeapon(1);

        }
    }

    private void FixedUpdate()
    {
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
