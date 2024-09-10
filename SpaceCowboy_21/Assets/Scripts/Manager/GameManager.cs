using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�÷��̾� ���� 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject shuttlePrefab;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] GameObject lobbyShuttlePrefab;

    public Transform player { get; private set; }
    public bool playerIsAlive { get; private set; } //������ �÷��̾ ��Ҵ��� �׾����� ����

    //�߷°���
    public float worldGravity = 900f;

    //�̱���
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

    //�Ŵ�����
    public PoolManager poolManager { get; private set; }
    public PopperManager popperManager { get; private set; }
    public AudioManager audioManager { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public CameraManager cameraManager { get; set; }
    public MaterialManager materialManager { get; set; }
    public TechDocument techDocument { get; private set; }
    
    //Dictionary��
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
        //���ø����̼� ����
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    #region ĳ����, ��Ʋ ����
    public GameObject SpawnPlayer(Vector2 pos, Quaternion rot)
    {
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //�÷��̾��� ������ �Ҵ��Ѵ�. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //�÷��̾���� ���� ������Ʈ
        playerIsAlive = true;

        //���� ���� ������Ʈ
        popperManager.PopperReady();

        //ī�޶� ���� ������Ʈ
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

        //ī�޶� ������Ʈ
        cameraManager.SetVirtualCam();
        //cameraManager.InitLobbyCam(playerObj.transform);
        //DoCamEventByLobbyState();

        ////��Ʋ ��ȯ
        //GameObject shuttleObj =  Instantiate(lobbyShuttlePrefab, pos, rot);
        //shuttleObj.GetComponent<LobbyDrone>().SetTarget(playerObj.transform);
    }

    #endregion

    #region �÷��̾� ���� �̺�Ʈ(��� ��, �����̵� ��)
    // �÷��̾ ������ ������ �̺�Ʈ �߻�
    public void PlayerIsDead()
    {
        //�÷��̾���� ���� ������Ʈ
        playerIsAlive = false;

        if (PlayerDeadEvent != null)
            PlayerDeadEvent();

        if(cameraManager != null)
            cameraManager.StopCameraFollow();
    }

    //�÷��̾ ��ġ �̵��� ���� ��(�� ���, Ȥ�� �ڷ���Ʈ ���)
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

    #region �� �ҷ����� ���� 

    //�� �ε�
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

    
    //��Ȳ�� ���� �κ� ���� �̺�Ʈ
    [SerializeField] private string lobbyName = "LobbyUI";
    public bool fromStageUI { get; set; }
    //���� ���� > �κ� ���� �� 
    public void LoadGameStart()
    {
        curState = LobbyEnterState.GameStart;
        fromStageUI = false;
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }
    
    //�÷��̾� ���  > �κ� ���� �� 
    public void LoadPlayerDie()
    {
        curState = LobbyEnterState.Die;
        fromStageUI = true;
        StartCoroutine(LoadSceneRoutine(lobbyName));
        poolManager.ResetPools();
    }
    //�������� Ŭ���� > �κ� ���� �� 
    public void LoadStageClear()
    {
        curState = LobbyEnterState.StageClear;
        fromStageUI = true;
        StartCoroutine(LoadSceneRoutine(lobbyName));
        poolManager.ResetPools();
    }


    IEnumerator LoadSceneRoutine(string sceneName)
    {
        //ī�޶� �ʱ�ȭ..(�ٸ��͵� �ʱ�ȭ �ʿ��Ѱ� ������ ���⼭)
        cameraManager.ResetCam();


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("�� �ҷ����� �Ϸ�");

        TransitionFadeOut(false);


        ////���� ���� �̸��� �κ��� ��� �κ� ĳ���͸� �����Ѵ�. 
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

        //�κ�� ���� �� 
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
        Debug.Log("UI����");

        stageEndUi.gameObject.SetActive(true);
        yield return new WaitForSeconds(endDelay);
        stageEndUi.gameObject.SetActive(false);
    }

    //�� ���̵� ��,�ƿ�
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
