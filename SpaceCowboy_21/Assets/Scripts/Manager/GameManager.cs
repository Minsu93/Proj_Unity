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
    public float mapLens = 16.0f;
    public float defaultLens = 8.0f;
    float currLens;
    float targetLens;
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
    //public float worldGravity;

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
        //AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;

        //�ʱ� ī�޶� 
        currLens = defaultLens;
        targetLens = currLens;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();


    }

    private void FixedUpdate()
    {

        if (Mathf.Abs(currLens - targetLens) > approx)
        {
            currLens = Mathf.Lerp(currLens, targetLens, Time.deltaTime * controlSpeed);
            virtualCamera.m_Lens.OrthographicSize = currLens;
        }
        
    }



    // �÷��̾ ���̸� ������ �̺�Ʈ �߻�
    public void PlayerIsDead()
    {
        if (PlayerDeadEvent != null)
            PlayerDeadEvent();
    }

    public void MapOpen()
    {
        //cameraLens.LensExpand();
        //virtualCamera.m_Lens.OrthographicSize = lensExpand;
        targetLens = mapLens;

        cameraPos.StopCameraFollow();
        
    }

    public void MapClose()
    {
        //cameraLens.LensReduce();
        //virtualCamera.m_Lens.OrthographicSize = lensReduce;
        targetLens = defaultLens;

        cameraPos.StartCameraFollow();
    }

    public void ChangeCamera(float orthoSize)
    {
        defaultLens = orthoSize;
        targetLens = defaultLens;
    }
}
