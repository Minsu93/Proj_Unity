using System;
using System.Collections;
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
        //weaponDictionary.LoadEquippedWeapons();
        skillDictionary.LoadSkillDictionary();

        //���� ���� ������Ʈ
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
        //���ø����̼� ����
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

    #region ĳ����, ��Ʋ ����
    public GameObject SpawnPlayer(Vector2 pos, Quaternion rot)
    {
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //�÷��̾��� ������ �Ҵ��Ѵ�. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //�÷��̾���� ���� ������Ʈ
        playerIsAlive = true;


        //ī�޶� ���� ������Ʈ
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

    #region �÷��̾� ���� �̺�Ʈ(��� ��, �����̵� ��)
    // �÷��̾ ������ ������ �̺�Ʈ �߻�
    public void PlayerIsDead()
    {
        //�÷��̾���� ���� ������Ʈ
        playerIsAlive = false;

        if (PlayerDeadEvent != null)
            PlayerDeadEvent();

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
    public int chapterIndex = 1;
    public int stageIndex = 1;
    int maxStage = 5;
    //��Ȳ�� ���� �κ� ���� �̺�Ʈ
    [SerializeField] private string lobbyName = "LobbyUI";
    delegate void AfterSceneLoadEvent();
    AfterSceneLoadEvent sceneDel;

    public event System.Action StageStartEvent;
    public event System.Action StageEndEvent;   
    //�� �ε�
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
    
    //�÷��̾� ���  > �κ� ���� �� 
    public void ReturnToLobby()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }

    public void MoveToNextStage()
    {
        stageIndex++;
        //���� ���������� �̵�
        string sceneStr = "Stage_" + chapterIndex.ToString() + "_" + stageIndex.ToString();
        StartCoroutine(LoadSceneRoutine(sceneStr));

    }

    //�������� Ŭ���� �� �����ϴ� ���� �̺�Ʈ. 
    // StartPortal ���� OpenPortal �̺�Ʈ�� ����ȴ�. 
    public void StageClear()
    {
        if (StageEndEvent != null) StageEndEvent();
        
        StageEndEvent = null;
    }
    public void ChapterClear()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
        //poolManager.ResetPools();
        //é�͸� Ŭ���� ������ ChapterIndex ++
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

        //ī�޶� �ʱ�ȭ..(�ٸ��͵� �ʱ�ȭ �ʿ��Ѱ� ������ ���⼭)
        cameraManager.ResetCam();


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("�� �ҷ����� �Ϸ�");

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
