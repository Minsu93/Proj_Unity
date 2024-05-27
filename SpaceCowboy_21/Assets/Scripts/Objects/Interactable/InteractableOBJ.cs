using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableOBJ : MonoBehaviour
{
    public bool interactable; //���ͷ��� ���ΰ���
    protected bool turnOnStart = false;   //���� ����, �������� �������� ������.

    public int interactCost;  //Ȱ��ȭ ����
    public float interactTime;  //Ȱ��ȭ�ϱ���� �ɸ��� �ð�
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
        //Ȱ��ȭ ����
        turnOnTimer = 0f;

        //��� �ð�
        while (turnOnTimer < activateTime)
        {
            turnOnTimer += Time.deltaTime;
            yield return null;
        }

        //Ȱ��ȭ �Ϸ�
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

    //��ȣ�ۿ�� �� �ൿ
    public abstract void InteractAction();

    public abstract void turnOnAction();


}
