using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //플레이어 관련 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] GameObject lobbyShuttlePrefab;

    public Transform player { get; private set; }
    public bool playerIsAlive { get; private set; } //적들이 플레이어가 살았는지 죽었는지 참고

    //중력관련
    public float worldGravity = 900f;
    public float worldGravityRadius = 10.0f;

    //싱글톤
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance;
        }
    }

    //매니저들
    public PoolManager poolManager { get; private set; }
    public PopperManager popperManager { get; private set; }
    public DropManager dropManager { get; private set; }
    public AudioManager audioManager { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public CameraManager cameraManager { get; set; }
    public ArrowManager arrowManager { get; private set; }  
    
    //Dictionary들
    public MonsterDictionary monsterDictonary { get; private set; }
    public WeaponDictionary weaponDictionary { get; private set; }

    //UI
    [SerializeField] GameObject stageUI;
    Transform stageStartUi;
    Transform stageEndUi;
    

    [Space]
    public bool playBGM;

    public event System.Action PlayerDeadEvent;
    public event System.Action PlayerTeleportStart;
    public event System.Action PlayerTeleportEnd;


    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        poolManager = GetComponentInChildren<PoolManager>();
        popperManager = GetComponentInChildren<PopperManager>();
        dropManager = GetComponentInChildren<DropManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        particleManager = GetComponentInChildren<ParticleManager>();
        playerManager = GetComponentInChildren<PlayerManager>();
        cameraManager = GetComponentInChildren<CameraManager>();
        arrowManager = GetComponentInChildren<ArrowManager>();

        monsterDictonary = GetComponentInChildren<MonsterDictionary>();
        weaponDictionary = GetComponentInChildren<WeaponDictionary>();

        GameObject ui = Instantiate(stageUI, this.transform);
        stageEndUi = ui.transform.Find("StageEndUI");
        stageEndUi.gameObject.SetActive(false);

        weaponDictionary.LoadWeaponDictionary();

        //무기 정보 업데이트
        //popperManager.PopperReady();

        fadeCanvas = fadeoutAnimator.transform.parent.gameObject;

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

    Vector2 mapSize;
    public Vector2 MapSize
    {
        get
        {
            return mapSize;
        }
        set
        {
            mapSize = value;
        }
    }
    Vector2 mapCenter;
    public Vector2 MapCenter { get; set; }


    #region 캐릭터, 셔틀 스폰
    public GameObject SpawnPlayer(Vector2 pos, Quaternion rot)
    {
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //플레이어의 정보를 할당한다. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //플레이어생명 변수 업데이트
        playerIsAlive = true;
        
        return playerObj;
    }

    public void RespawnPlayer(Vector2 pos, Quaternion rot)
    {
        playerIsAlive = true;
        //cameraManager.StartCameraFollow();

        playerManager.playerBehavior.InitPlayer();


    }


    #endregion

    #region 플레이어 전역 이벤트(사망 시, 순간이동 시)
    // 플레이어가 죽으면 전역에 이벤트 발생
    public void PlayerIsDead()
    {
        //플레이어생명 변수 업데이트
        playerIsAlive = false;

        if (PlayerDeadEvent != null)
            PlayerDeadEvent();

        cameraManager.StopCameraFollow();

    }

    //플레이어가 위치 이동을 했을 때(맵 경계, 혹은 텔레포트 기계)
    public void PlayerIsTeleport(bool start )
    {
        if (start)
        {
            if (PlayerTeleportStart != null) PlayerTeleportStart();
        }
        else
        {
            if(PlayerTeleportEnd != null) PlayerTeleportEnd();  
        }
    }
    #endregion

    #region 씬 불러오기 관련 
    public int chapterIndex = 1;
    //상황에 따른 로비 진입 이벤트
    [SerializeField] private string lobbyName = "LobbyUI";

    public event System.Action<bool> StageStartEvent;
    //씬 로드
    //GameStart -> LobbyUI

    public void LoadsceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void StageStart(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
        playerManager.InitializeLife();
    }
    
    //플레이어 사망  > 로비 진입 시 
    public void ReturnToLobby()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }


    public void ChapterClear()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
        //챕터를 클리어 했으면 ChapterIndex ++
        chapterIndex++;
    }


    IEnumerator LoadSceneRoutine(string sceneName)
    {

        yield return new WaitForSeconds(0.6f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("씬 불러오기 완료");

        if (fadeoutAnimator.GetBool("fade"))
        {
            TransitionFadeOut(false);
        }

        StageStartEvent = null;
    }


    public IEnumerator ShowStageEndUI(float startDelay, float endDelay )
    {
        yield return new WaitForSeconds(startDelay);
        Debug.Log("UI오픈");

        stageEndUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageEndUi.gameObject.SetActive(false);
    }


    //씬 페이드 인,아웃
    [SerializeField] public Animator fadeoutAnimator;
    GameObject fadeCanvas;
    public void TransitionFadeOut(bool fadeOut)
    {
        if (fadeOut)
        {
            //FadeOut 시작 
            if (!fadeCanvas.activeSelf) fadeCanvas.SetActive(true);
            fadeoutAnimator.SetBool("fade", fadeOut);
        }
        else
        {
            //FadeOut 종료
            fadeoutAnimator.SetBool("fade", fadeOut);
        }

    }

    #endregion
}
