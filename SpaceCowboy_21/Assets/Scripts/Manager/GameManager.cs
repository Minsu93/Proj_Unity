using Cinemachine;
using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        //���ø����̼� ����
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    #region ĳ����, ��Ʋ ����
    public void SpawnPlayer()
    {
        //���� ����Ʈ�� �����´�. 
        StartPoint spawnPoint = GameObject.FindObjectOfType<StartPoint>();
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        //���� ����Ʈ�� ������ �� ���, ������ 0,0 �� �÷��̾ ��ȯ�Ѵ�. 
        if(spawnPoint != null)
        {
            pos = spawnPoint.transform.position;
            rot = spawnPoint.transform.rotation;
        }
        GameObject playerObj = Instantiate(playerPrefab, pos, rot);

        //�÷��̾��� ������ �Ҵ��Ѵ�. 
        player = playerObj.transform;
        playerManager.UpdatePlayerScripts(playerObj);
        
        //�÷��̾���� ���� ������Ʈ
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
        StartCoroutine(LoadSceneRoutine(sceneName, StageState.Lobby));
    }

    public void LoadSceneByStageState(string stageName, StageState stageState)
    {
        //Before Load
        switch (stageState)
        {
            case StageState.Lobby:
                //�κ� ȭ�� -> ��������
                StartCoroutine(LoadSceneRoutine(stageName, stageState));
                break;
            case StageState.Stage:
                //�������� -> ��������
                materialManager.SaveMoney();
                StartCoroutine(LoadSceneRoutine(stageName, stageState));
                break;
            case StageState.BossLevel:
                //���� ���� -> �κ�
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
        Debug.Log("�� �ҷ����� �Ϸ�");

        switch (stageState)
        {
            case StageState.Lobby:
                //�κ� ȭ�� -> ��������
                StartCoroutine(ShowStageStartUI(1f, 5f));
                break;
            case StageState.Stage:
                //�������� -> ��������
                break;
            case StageState.BossLevel:
                //���� ���� -> �κ�
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
