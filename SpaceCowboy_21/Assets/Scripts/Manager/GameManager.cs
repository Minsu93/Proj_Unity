using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�÷��̾� ���� 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] GameObject lobbyShuttlePrefab;

    public Transform player { get; private set; }
    public bool playerIsAlive { get; private set; } //������ �÷��̾ ��Ҵ��� �׾����� ����

    //�߷°���
    public float worldGravity = 900f;
    public float worldGravityRadius = 10.0f;

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
    public DropManager dropManager { get; private set; }
    public AudioManager audioManager { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public CameraManager cameraManager { get; set; }
    public ArrowManager arrowManager { get; private set; }  
    
    //Dictionary��
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

        //���� ���� ������Ʈ
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
        //���ø����̼� ����
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


    #region ĳ����, ��Ʋ ����
    public GameObject SpawnPlayer(Vector2 pos, Quaternion rot)
    {
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //�÷��̾��� ������ �Ҵ��Ѵ�. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //�÷��̾���� ���� ������Ʈ
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
    //��Ȳ�� ���� �κ� ���� �̺�Ʈ
    [SerializeField] private string lobbyName = "LobbyUI";

    public event System.Action<bool> StageStartEvent;
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
    }
    
    //�÷��̾� ���  > �κ� ���� �� 
    public void ReturnToLobby()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
    }


    public void ChapterClear()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName));
        //é�͸� Ŭ���� ������ ChapterIndex ++
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

        Debug.Log("�� �ҷ����� �Ϸ�");

        if (fadeoutAnimator.GetBool("fade"))
        {
            TransitionFadeOut(false);
        }

        StageStartEvent = null;
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
        if (fadeOut)
        {
            //FadeOut ���� 
            if (!fadeCanvas.activeSelf) fadeCanvas.SetActive(true);
            fadeoutAnimator.SetBool("fade", fadeOut);
        }
        else
        {
            //FadeOut ����
            fadeoutAnimator.SetBool("fade", fadeOut);
        }

    }

    #endregion
}
