using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public PoolManager poolManager;

    public static GameManager Instance
    {
        get
        {
            // SingletonBehaviour�� �ʱ�ȭ �Ǳ� ���̶��
            if (_instance == null)
            {
                // �ش� ������Ʈ�� ã�� �Ҵ��Ѵ�.
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
        // ������ �� ������ 2������ �û��ϰ� �ȴ�.
        if (_instance != null)
        {
            // (1) �ٸ� ���� ������Ʈ�� �ִٸ�
            if (_instance != this)
            {
                // �ϳ��� ���� ������Ʈ�� ������ �����Ѵ�.
                Destroy(gameObject);
            }


            // (2) Awake() ȣ�� �� �Ҵ�� �ν��Ͻ��� �ڱ� �ڽ��̶��
            // �ƹ��͵� ���� �ʴ´�.
            return;
        }

        // �� �Ʒ��� ���� SingletonBahaviour�� ���� ����
        // Instance ���� �� Awake()�� ����Ǵ� ����̴�.
        _instance = GetComponent<GameManager>();
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        //AudioManager.instance.PlayBgm(true);

        //Cursor.visible = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    // �÷��̾ ���̸� ������ �̺�Ʈ �߻�
    public void PlayerIsDead()
    {
        if (PlayerDeadEvent != null)
            PlayerDeadEvent();
    }
}
