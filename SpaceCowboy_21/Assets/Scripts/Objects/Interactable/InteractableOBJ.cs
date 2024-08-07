using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableOBJ : MonoBehaviour
{
    //public bool interactOn { get; set; }
    protected bool interactActive = true;

    [SerializeField] private GameObject outlineObject;
   // protected SpriteRenderer spr;




    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (interactActive && collision.CompareTag("Player"))
        {
            DrawOutline();
            GameManager.Instance.playerManager.SetInteractableObj(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RemoveOutLine();
            GameManager.Instance.playerManager.RemoveInteractableObj(this);
        }
    }

    public virtual void StopInteract()
    {
        RemoveOutLine();
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
