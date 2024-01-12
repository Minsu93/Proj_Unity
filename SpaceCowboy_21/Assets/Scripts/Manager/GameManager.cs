using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public PoolManager poolManager;
    public CameraPos cameraPos;
    public CinemachineVirtualCamera virtualCamera;

    //�߷°���
    public float worldGravity = 900f;

    //ī�޶� ����
    public float mapFOV = 120f;
    public float defaultFOV = 90f;
    float currFOV;
    float targetFOV;

    float approx = 0.05f;
    public float controlSpeed = 5.0f;

    public static GameManager Instance
    {
        get
        {
            // SingletonBehaviour�� �ʱ�ȭ �Ǳ� ���̶��
            if (_instance == null)
            {
                // �ش� ������Ʈ�� ã�� �Ҵ��Ѵ�.
                _instance = FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public Transform player;
    public PlayerWeapon weapon;
    //public float worldGravity;

    [Space]
    public bool playBGM;

    public event System.Action PlayerDeadEvent;


    void Awake()
    {
        // ������ �� ������ 2������ �û��ϰ� �ȴ�.
        if (_instance != null)
        {
            // (1) �ٸ� ���� ������Ʈ�� �ִٸ�
            if (_instance != this)
            {
                // �ϳ��� ���� ������Ʈ�� ������ �����Ѵ�.
                Destroy(gameObject);
            }


            // (2) Awake() ȣ�� �� �Ҵ�� �ν��Ͻ��� �ڱ� �ڽ��̶��
            // �ƹ��͵� ���� �ʴ´�.
            return;
        }

        // �� �Ʒ��� ���� SingletonBahaviour�� ���� ����
        // Instance ���� �� Awake()�� ����Ǵ� ����̴�.
        _instance = GetComponent<GameManager>();
        DontDestroyOnLoad(gameObject);


    }


    private void Start()
    {
        if(playBGM)
            AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;

        //�ʱ� FOV
        currFOV = defaultFOV;
        targetFOV = currFOV;
    }


    private void Update()
    {
        //���ø����̼� ����
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    private void FixedUpdate()
    {
        //ī�޶� ����

        if(Mathf.Abs(currFOV - targetFOV) > approx)
        {
            currFOV = Mathf.Lerp(currFOV, targetFOV, Time.deltaTime * controlSpeed);
            virtualCamera.m_Lens.FieldOfView = currFOV;
        }

    }


    // �÷��̾ ���̸� ������ �̺�Ʈ �߻�
    public void PlayerIsDead()
    {
        if (PlayerDeadEvent != null)
            PlayerDeadEvent();

        virtualCamera.Follow = null;
    }


    //ī�޶� �̺�Ʈ
    public void MapOpen()
    {
        targetFOV = mapFOV;

        cameraPos.StopCameraFollow();
        
    }

    public void MapClose()
    {
        targetFOV = defaultFOV;

        cameraPos.StartCameraFollow();
    }

    //�༺ �̵��� �⺻ FOV ����
    public void ChangeCamera(float orthoSize)
    {
        defaultFOV = orthoSize;
        targetFOV = defaultFOV;
    }
}
