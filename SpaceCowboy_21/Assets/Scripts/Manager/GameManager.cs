using Cinemachine;
using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�÷��̾� ���� 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject shuttlePrefab;
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

    }


    private void Start()
    {
        //if(playBGM) AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;

        fadeCanvas = fadeoutAnimator.transform.parent.gameObject;

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
        cameraManager.InitCam();

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

    #endregion

    #region ���� �̺�Ʈ
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

    #region �� ���� 
    //�� �ε�
    public void LoadsceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, StageState.None));
    }

    public void LoadSceneByStageState(string sceneName, StageState state)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, state));
    }

    [SerializeField] private string lobbyName = "LobbyUI";
    public void StageClear()
    {
        StartCoroutine(LoadSceneRoutine(lobbyName, StageState.BossLevel));
    }

    IEnumerator LoadSceneRoutine(string sceneName, StageState stageState)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("�� �ҷ����� �Ϸ�");


        if(stageState == StageState.Stage)
        {
            StartCoroutine(ShowStageStartUI(1f, 5f));

        }
        switch(stageState)
        {
            case StageState.Lobby:
                cameraManager.StageStartCameraZoomin();
                break;
            case StageState.Stage:
                cameraManager.StageStartCameraZoomin();
                StartCoroutine(ShowStageStartUI(1f, 5f));

                break;
        }
        TransitionFadeOut(false);
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
public enum StageState { None, Lobby, Stage, BossLevel }
