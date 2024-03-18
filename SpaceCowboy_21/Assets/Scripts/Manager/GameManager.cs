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

    //�߷°���
    public float worldGravity = 900f;



    public static GameManager Instance
    {
        get
        {
            // SingletonBehaviour�� �ʱ�ȭ �Ǳ� ���̶��
            if (_instance == null)
            {
                // �ش� ������Ʈ�� ã�� �Ҵ��Ѵ�.
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
        //���ø����̼� ����
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }


    // �÷��̾ ������ ������ �̺�Ʈ �߻�
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

    

    //�÷��̾� �Է� ���� 
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

    //��ȣ�ۿ� ����
    public void InteractSomething()
    {
        Debug.Log("oh");
        if (curObj == null)
            return;

        //�÷��̾� Cancel�� �����ϵ��� ���� ����.
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
