using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableOBJ : MonoBehaviour
{
    public bool interactOn { get; set; }

    [SerializeField] private GameObject outlineObject;
    protected SpriteRenderer spr;
    //protected CircleCollider2D circleColl;

    //Material sprMat;


    protected virtual void Awake()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        //circleColl = GetComponent<CircleCollider2D>();
        //sprMat = spr.material;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DrawOutline();
            interactOn = true;
            GameManager.Instance.playerManager.SetInteractableObj(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RemoveOutLine();
            interactOn = false;
            GameManager.Instance.playerManager.RemoveInteractableObj(this);
        }
    }

    public void StopInteract()
    {
        RemoveOutLine();
        interactOn = false;
    }

    protected void DrawOutline()
    {
        //sprMat.SetFloat("_OutlineAlpha", 1.0f);
        outlineObject.SetActive(true);
    }

    protected void RemoveOutLine()
    {
        //sprMat.SetFloat("_OutlineAlpha", 0f);
        outlineObject.SetActive(false);
    }

    //상호작용시 할 행동
    public abstract void InteractAction();


}
