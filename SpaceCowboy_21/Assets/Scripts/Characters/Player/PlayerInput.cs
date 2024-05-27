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

    //방어기 test
    //public GrabLasso grabLasso;
    //public CounterShield counterShield;

    bool moveON = false;
    public bool inputDisabled = false;


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior == null)
            return;

        //if (Input.GetButtonDown(CancelButton))
        //{
        //    GameManager.Instance.InteractCancel();
        //}


        if (inputDisabled)
            return;

        ////Map
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    GameManager.Instance.MapOpen();

        //}
        //if (Input.GetKeyUp(KeyCode.Tab))
        //{
        //    GameManager.Instance.MapClose();
        //}

        // Jump
        if (Input.GetButtonDown(JumpButton))
        {
            //playerBehavior.PrepareJump();
            playerBehavior.TrySpeedJump();
        }
        //if(Input.GetButtonUp(JumpButton))
        //{
        //    playerBehavior.TryJump();
        //}


        // Shoot
        if (Input.GetButtonDown(ShootButton))
        {
            playerBehavior.StartShoot();

        }

        if (Input.GetButtonUp(ShootButton))
        {
            playerBehavior.StopShoot();

        }

        //// MagicShoot
        //if (Input.GetMouseButtonDown(1))
        //{
        //    playerBehavior.TryMagicShoot();
        //}

        // Slide
        if (Input.GetButtonDown(SlideButton))
        {
            //playerBehavior.ToggleSlide();

            playerBehavior.TrySlide();
        }

        if (Input.GetButtonUp(SlideButton))
        {
            playerBehavior.TryStopSlide();
        }

        //무기 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerBehavior.TryChangeWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerBehavior.TryChangeWeapon(1);
        }
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    playerBehavior.TryChangeWeapon(2);
        //}
        //Lasso
        //if (Input.GetMouseButtonDown(1))
        //{
        //    grabLasso.TryThrowLasso();
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    CameraManager.instance.ChangeCameraThreshold(12f, 8f);
        //}
        //if(Input.GetMouseButtonUp(1))
        //{
        //    CameraManager.instance.ResetCameraThreshold();
        //}

        //스킬 사용
        //if (Input.GetMouseButtonDown(1))
        //{
        //    playerBehavior.TryUseSkill();
        //}

        //투척 위성 사용
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerBehavior.TryThrowSatellite();
        }

        //weaponWheel 사용
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.playerManager.OpenWeaponWheel();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GameManager.Instance.playerManager.CloseWeaponWheel();

        }
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.playerManager.CloseWeaponWheel();
        }


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
