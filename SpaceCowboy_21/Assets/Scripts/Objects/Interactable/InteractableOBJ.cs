using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableOBJ : MonoBehaviour
{
    public bool interactable; //인터렉션 중인가요
    protected bool turnOnStart = false;   //돈을 내고, 게이지가 차오르는 중인지.

    public int interactCost;  //활성화 가격
    public float interactTime;  //활성화하기까지 걸리는 시간
    public float turnOnTimer { get; set; }
    public bool interactOn { get; set; }

    protected ObjectUI objUI;
    protected SpriteRenderer spr;
    protected CircleCollider2D circleColl;

    Material sprMat;


    protected virtual void Awake()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        circleColl = GetComponent<CircleCollider2D>();
        objUI = GetComponentInChildren<ObjectUI>();
        sprMat = spr.material;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactable) return;


        if (collision.CompareTag("Player"))
        {
            if (turnOnStart)
            {
                interactOn = true;
                //objUI.GaugeOn(true);

            }
            else
            {
                DrawOutline();
                objUI.TextOn(true);
                interactOn = true;
                GameManager.Instance.playerManager.curObj = this;
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!interactable) return;

        if (collision.CompareTag("Player"))
        {
            if(turnOnStart)
            {
                interactOn = false;
                //objUI.GaugeOn(false);


            }
            else
            {
                RemoveOutLine();
                objUI.TextOn(false);
                interactOn = false;
                GameManager.Instance.playerManager.curObj = null;
            }
        }
    }

    private void Update()
    {
        if (turnOnStart && interactOn)
        {
            turnOnTimer += Time.deltaTime;
        }
        else
        {
            if(turnOnTimer>0)
                turnOnTimer -= Time.deltaTime;
        }

        if (turnOnTimer > interactTime)
        {
            turnOnAction();
            objUI.TextOn(false);
            objUI.GaugeOn(false);
            turnOnStart = false;
            interactOn = false;
            interactable = false;
        }
    }

    protected IEnumerator TurnOnRoutine(float activateTime)
    {
        //활성화 시작
        turnOnTimer = 0f;

        //대기 시간
        while (turnOnTimer < activateTime)
        {
            turnOnTimer += Time.deltaTime;
            yield return null;
        }

        //활성화 완료
        turnOnTimer = activateTime;

        turnOnAction();
    }

    protected void DrawOutline()
    {
        sprMat.SetFloat("_OutlineAlpha", 1.0f);
    }

    protected void RemoveOutLine()
    {
        sprMat.SetFloat("_OutlineAlpha", 0f);
    }

    //상호작용시 할 행동
    public abstract void InteractAction();

    public abstract void turnOnAction();


}
