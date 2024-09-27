using System;
using System.Collections;
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
        //weaponDictionary.LoadEquippedWeapons();
        skillDictionary.LoadSkillDictionary();

        //무기 정보 업데이트
        popperManager.PopperReady();

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
            if(mapSize == default)
            {
                MapBorder border = GameObject.FindGameObjectWithTag("SpaceBorder").GetComponent<MapBorder>();
                mapSize = new Vector2(border.width / 2, border.height / 2);
                Debug.Log("mapSize is : " + mapSize.ToString());
            }

            return mapSize;
        }
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


        //카메라 정보 업데이트
        cameraManager.SetVirtualCam();
        cameraManager.InitCam();

        
        playerObj.SetActive(false);
        return playerObj;
    }

    public void RespawnPlayer(Vector2 pos, Quaternion rot)
    {
        playerIsAlive = true;
        cameraManager.StartCameraFollow();

        playerManager.playerBehavior.InitPlayer();


    }

    //public GameObject SpawnShuttle(Vector2 pos, Quaternion rot)
    //{
    //    GameObject shuttleObj = Instantiate(shuttlePrefab, pos, rot);
    //    if(shuttleObj.TryGetComponent<FollowingShuttle>(out FollowingShuttle shuttle))
    //    {
    //        shuttle.InitializeShuttle();
    //    }

    //    shuttleObj.SetActive(false);
    //    return shuttleObj;
    //}


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
    public int stageIndex = 1;
    int maxStage = 5;
    //상황에 따른 로비 진입 이벤트
    [SerializeField] private string lobbyName = "LobbyUI";
    delegate void AfterSceneLoadEvent();
    AfterSceneLoadEvent sceneDel;

    public event System.Action StageStartEvent;
    public event System.Action StageEndEvent;   
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
        //sceneDel = ShowStartUI;
    }
    
    //플레이어 사망  > 로비 진입 시 
    public void ReturnToLobby()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }

    public void MoveToNextStage()
    {
        stageIndex++;
        //다음 스테이지로 이동
        string sceneStr = "Stage_" + chapterIndex.ToString() + "_" + stageIndex.ToString();
        StartCoroutine(LoadSceneRoutine(sceneStr));

    }

    //스테이지 클리어 시 실행하는 전역 이벤트. 
    // StartPortal 에서 OpenPortal 이벤트가 실행된다. 
    public void StageClear()
    {
        if (StageEndEvent != null) StageEndEvent();
        
        StageEndEvent = null;
    }
    public void ChapterClear()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
        //poolManager.ResetPools();
        //챕터를 클리어 했으면 ChapterIndex ++
        chapterIndex++;
        stageIndex = 1;
    }

    public bool IsNextStageAvailable()
    {
        if (stageIndex + 1 <= maxStage)
        {
            return true;
        }
        else return false;
    }


    IEnumerator LoadSceneRoutine(string sceneName)
    {
        //TransitionFadeOut(true);

        yield return new WaitForSeconds(0.6f);

        //카메라 초기화..(다른것도 초기화 필요한게 있으면 여기서)
        cameraManager.ResetCam();


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("씬 불러오기 완료");

        //TransitionFadeOut(false);
        if (sceneDel != null) sceneDel();
        if(StageStartEvent != null) StageStartEvent();

        StageStartEvent = null;
    }


    void ShowStartUI()
    {
        StartCoroutine(ShowStageStartUIRoutine(1f, 1f));
    }
    IEnumerator ShowStageStartUIRoutine(float startDelay, float endDelay)
    {
        yield return new WaitForSeconds(startDelay);
        stageStartUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageStartUi.gameObject.SetActive(false);
        sceneDel = null;

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
