using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //플레이어 관련 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject shuttlePrefab;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] GameObject lobbyShuttlePrefab;

    public Transform player { get; private set; }
    public bool playerIsAlive { get; private set; } //적들이 플레이어가 살았는지 죽었는지 참고

    //중력관련
    public float worldGravity = 900f;

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
    public AudioManager audioManager { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public CameraManager cameraManager { get; set; }
    public MaterialManager materialManager { get; set; }
    public TechDocument techDocument { get; private set; }
    
    //Dictionary들
    public MonsterDictionary monsterDictonary { get; private set; }
    public SkillDictionary skillDictionary { get; private set; }
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
        audioManager = GetComponentInChildren<AudioManager>();
        particleManager = GetComponentInChildren<ParticleManager>();
        playerManager = GetComponentInChildren<PlayerManager>();
        techDocument = GetComponentInChildren<TechDocument>();
        materialManager= GetComponentInChildren<MaterialManager>();
        cameraManager = GetComponentInChildren<CameraManager>();

        monsterDictonary = GetComponentInChildren<MonsterDictionary>();
        skillDictionary = GetComponentInChildren<SkillDictionary>();
        weaponDictionary = GetComponentInChildren<WeaponDictionary>();

        GameObject ui = Instantiate(stageUI, this.transform);
        stageStartUi = ui.transform.Find("StageStartUI");
        stageEndUi = ui.transform.Find("StageEndUI");
        stageStartUi.gameObject.SetActive(false);
        stageEndUi.gameObject.SetActive(false);


        weaponDictionary.LoadWeaponDictionary();
        weaponDictionary.LoadEquippedWeapons();
        skillDictionary.LoadSkillDictionary();

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

    #region 캐릭터, 셔틀 스폰
    public GameObject SpawnPlayer(Vector2 pos, Quaternion rot)
    {
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //플레이어의 정보를 할당한다. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //플레이어생명 변수 업데이트
        playerIsAlive = true;

        //무기 정보 업데이트
        popperManager.PopperReady();

        //카메라 정보 업데이트
        cameraManager.SetVirtualCam();
        cameraManager.InitCam();
        //DoCamEventByLobbyState();

        playerObj.SetActive(false);
        return playerObj;
    }

    public GameObject SpawnShuttle(Vector2 pos, Quaternion rot)
    {
        GameObject shuttleObj = Instantiate(shuttlePrefab, pos, rot);
        if(shuttleObj.TryGetComponent<FollowingShuttle>(out FollowingShuttle shuttle))
        {
            shuttle.InitializeShuttle();
        }

        shuttleObj.SetActive(false);
        return shuttleObj;
    }

    public void SpawnLobbyPlayer(Vector2 pos, Quaternion rot)
    {
        //GameObject playerObj = Instantiate(lobbyPlayerPrefab, pos, rot);
        //player = playerObj.transform;

        //카메라 업데이트
        cameraManager.SetVirtualCam();
        //cameraManager.InitLobbyCam(playerObj.transform);
        //DoCamEventByLobbyState();

        ////셔틀 소환
        //GameObject shuttleObj =  Instantiate(lobbyShuttlePrefab, pos, rot);
        //shuttleObj.GetComponent<LobbyDrone>().SetTarget(playerObj.transform);
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

        if(cameraManager != null)
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

    //씬 로드
    //GameStart -> LobbyUI
    public void LoadsceneByName(string sceneName)
    {
        curState = LobbyEnterState.None;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void LoadSceneByStageState(string sceneName, LobbyEnterState state)
    {
        curState = state;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    
    //상황에 따른 로비 진입 이벤트
    [SerializeField] private string lobbyName = "LobbyUI";
    public bool fromStageUI { get; set; }
    //게임 시작 > 로비 진입 시 
    public void LoadGameStart()
    {
        curState = LobbyEnterState.GameStart;
        fromStageUI = false;
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }
    
    //플레이어 사망  > 로비 진입 시 
    public void LoadPlayerDie()
    {
        curState = LobbyEnterState.Die;
        fromStageUI = true;
        StartCoroutine(LoadSceneRoutine(lobbyName));
        poolManager.ResetPools();
    }
    //스테이지 클리어 > 로비 진입 시 
    public void LoadStageClear()
    {
        curState = LobbyEnterState.StageClear;
        fromStageUI = true;
        StartCoroutine(LoadSceneRoutine(lobbyName));
        poolManager.ResetPools();
    }


    IEnumerator LoadSceneRoutine(string sceneName)
    {
        //카메라 초기화..(다른것도 초기화 필요한게 있으면 여기서)
        cameraManager.ResetCam();


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("씬 불러오기 완료");

        TransitionFadeOut(false);


        ////현재 씬의 이름이 로비일 경우 로비 캐릭터를 스폰한다. 
        //Scene scene = SceneManager.GetActiveScene();
        //if (scene.name == lobbyName && player == null)
        //{
        //    SpawnLobbyPlayer(Vector2.zero, Quaternion.identity);
        //}
    }

    LobbyEnterState curState = LobbyEnterState.None;

    void DoCamEventByLobbyState()
    {
        TransitionFadeOut(false);

        //로비로 들어올 때 
        //switch (curState)
        //{
        //    case LobbyEnterState.GameStart:

        //        TransitionFadeOut(false);
        //        cameraManager.SetStartCamera(CamDist.Back);
        //        cameraManager.ZoomCamera(CamDist.Middle, ZoomSpeed.Fast);
        //        break;
        //    case LobbyEnterState.Die:
        //        TransitionFadeOut(false);
        //        cameraManager.SetStartCamera(CamDist.Fore);
        //        cameraManager.ZoomCamera(CamDist.Middle, ZoomSpeed.Fast);
        //        break;
        //    case LobbyEnterState.StageClear:
        //        TransitionFadeOut(false);
        //        cameraManager.SetStartCamera(CamDist.Fore);
        //        cameraManager.ZoomCamera(CamDist.Middle, ZoomSpeed.Fast);
        //        break;
        //}
    }


    public IEnumerator ShowStageStartUI(float startDelay, float endDelay)
    {
        yield return new WaitForSeconds(startDelay);
        stageStartUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageStartUi.gameObject.SetActive(false);
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
        if (!fadeCanvas.activeSelf) fadeCanvas.SetActive(true);
        fadeoutAnimator.SetBool("fade", fadeOut);
    }

    #endregion
}
public enum LobbyEnterState { None, GameStart, StageClear, Die }
