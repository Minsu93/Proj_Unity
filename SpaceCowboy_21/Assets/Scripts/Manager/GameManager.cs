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
            // SingletonBehaviour가 초기화 되기 전이라면
            if (_instance == null)
            {
                // 해당 오브젝트를 찾아 할당한다.
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
        // 이제는 이 조건이 2가지를 시사하게 된다.
        if (_instance != null)
        {
            // (1) 다른 게임 오브젝트가 있다면
            if (_instance != this)
            {
                // 하나의 게임 오브젝트만 남도록 삭제한다.
                Destroy(gameObject);
            }


            // (2) Awake() 호출 전 할당된 인스턴스가 자기 자신이라면
            // 아무것도 하지 않는다.
            return;
        }

        // 이 아래의 경우는 SingletonBahaviour가 운이 좋게
        // Instance 참조 전 Awake()가 실행되는 경우이다.
        _instance = GetComponent<GameManager>();
        DontDestroyOnLoad(gameObject);


    }


    private void Start()
    {
        //AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;

        //초기 카메라 
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



    // 플레이어가 죽이면 전역에 이벤트 발생
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
