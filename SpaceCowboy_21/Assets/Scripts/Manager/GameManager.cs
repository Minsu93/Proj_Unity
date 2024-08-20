using Cinemachine;
using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //플레이어 관련 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject shuttlePrefab;
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

        monsterDictonary = GetComponentInChildren<MonsterDictionary>();
        skillDictionary = GetComponentInChildren<SkillDictionary>();
        weaponDictionary = GetComponentInChildren<WeaponDictionary>();

        GameObject ui = Instantiate(stageUI, this.transform);
        stageStartUi = ui.transform.Find("StageStartUI");
        stageEndUi = ui.transform.Find("StageEndUI");
        stageStartUi.gameObject.SetActive(false);
        stageEndUi.gameObject.SetActive(false);


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
    public void SpawnPlayer()
    {
        //스폰 포인트를 가져온다. 
        StartPoint spawnPoint = GameObject.FindObjectOfType<StartPoint>();
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        //스폰 포인트가 있으면 그 장소, 없으면 0,0 에 플레이어를 소환한다. 
        if(spawnPoint != null)
        {
            pos = spawnPoint.transform.position;
            rot = spawnPoint.transform.rotation;
        }
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //플레이어의 정보를 할당한다. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //플레이어생명 변수 업데이트
        playerIsAlive = true;

    }

    public void SpawnShuttle(Vector2 pos, Quaternion rot)
    {
        GameObject shuttleObj = Instantiate(shuttlePrefab, pos, rot);
        if(shuttleObj.TryGetComponent<FollowingShuttle>(out FollowingShuttle shuttle))
        {
            shuttle.InitializeShuttle();
        }
    }

    #endregion

    #region 전역 이벤트
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

    #region 씬 관련 
    //씬 로드
    public void LoadsceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, StageState.Lobby));
    }

    public void LoadSceneByStageState(string stageName, StageState stageState)
    {
        //Before Load
        switch (stageState)
        {
            case StageState.Lobby:
                //로비 화면 -> 스테이지
                StartCoroutine(LoadSceneRoutine(stageName, stageState));
                break;
            case StageState.Stage:
                //스테이지 -> 스테이지
                materialManager.SaveMoney();
                StartCoroutine(LoadSceneRoutine(stageName, stageState));
                break;
            case StageState.BossLevel:
                //보스 레벨 -> 로비
                materialManager.ResetMoney();
                StartCoroutine(ShowStageEndUI(1f, 5f, stageName, stageState));
                break;
        }

    }

    IEnumerator LoadSceneRoutine(string sceneName, StageState stageState)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("씬 불러오기 완료");

        switch (stageState)
        {
            case StageState.Lobby:
                //로비 화면 -> 스테이지
                StartCoroutine(ShowStageStartUI(1f, 5f));
                break;
            case StageState.Stage:
                //스테이지 -> 스테이지
                break;
            case StageState.BossLevel:
                //보스 레벨 -> 로비
                break;
        }
    }

    IEnumerator ShowStageStartUI(float startDelay, float endDelay)
    {
        yield return new WaitForSeconds(startDelay);
        stageStartUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageStartUi.gameObject.SetActive(false);

    }

    IEnumerator ShowStageEndUI(float startDelay, float endDelay , string stageName, StageState stageState)
    {
        yield return new WaitForSeconds(startDelay);
        stageEndUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageEndUi.gameObject.SetActive(false);
        StartCoroutine(LoadSceneRoutine(stageName, stageState));

    }


    #endregion
}
