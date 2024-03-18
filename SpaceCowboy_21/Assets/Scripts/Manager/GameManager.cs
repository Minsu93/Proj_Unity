using Cinemachine;
using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public Transform player;
    PlayerInput playerInput;
    //Rigidbody2D _playerRb;
    //public Rigidbody2D playerRb
    //{
    //    get 
    //    { 
    //        if(_playerRb == null)
    //        {
    //            _playerRb = player.GetComponent<Rigidbody2D>(); 
    //        }
    //        return _playerRb; 
    //    }
    //}
    PlayerBehavior _playerBehavior;
    public PlayerBehavior PlayerBehavior
    {
        get
        {
            if(_playerBehavior == null)
            {
                _playerBehavior = player.GetComponent<PlayerBehavior>();
            }
            return _playerBehavior;
        }
    }

    public InteractableOBJ curObj;

    //중력관련
    public float worldGravity = 900f;



    public static GameManager Instance
    {
        get
        {
            // SingletonBehaviour가 초기화 되기 전이라면
            if (_instance == null)
            {
                // 해당 오브젝트를 찾아 할당한다.
                _instance = FindObjectOfType<GameManager>();
                //DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }


    //public float worldGravity;

    [Space]
    public bool playBGM;

    public event System.Action PlayerDeadEvent;


    void Awake()
    {
        //if (_instance != null)
        //{
        //    if (_instance != this)
        //    {
        //        Destroy(gameObject);
        //    }

        //    return;
        //}

        _instance = GetComponent<GameManager>();
        //DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        //if(playBGM) AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;

    }


    private void Update()
    {
        //어플리케이션 종료
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }


    // 플레이어가 죽으면 전역에 이벤트 발생
    public void PlayerIsDead()
    {
        if (PlayerDeadEvent != null)
            PlayerDeadEvent();
    }

    public void LoadScene(int num)
    {
        SceneManager.LoadScene(num);
    }
    public void Loadscene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    

    //플레이어 입력 관련 
    public void DisablePlayerInput()
    {
        if(playerInput == null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
        playerInput.inputDisabled = true;
    }
    public void EnablePlayerInput()
    {
        if (playerInput == null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
        playerInput.inputDisabled = false;
    }

    //상호작용 관련
    public void InteractSomething()
    {
        Debug.Log("oh");
        if (curObj == null)
            return;

        //플레이어 Cancel만 가능하도록 조작 변경.
        DisablePlayerInput();
        curObj.InteractAction();
    }

    public void InteractCancel()
    {
        if (curObj == null)
            return;

        EnablePlayerInput();
        curObj.CancelAction();
    }
}
