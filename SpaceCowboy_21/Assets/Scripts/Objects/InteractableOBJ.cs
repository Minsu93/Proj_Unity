using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableOBJ : MonoBehaviour
{
    public GameObject pressButton;
    SpriteRenderer spr;
    MaterialPropertyBlock block;


    private void Awake()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        block = new MaterialPropertyBlock();
        pressButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            block.SetFloat("_OutLine", 1.0f);
            spr.SetPropertyBlock(block);
            pressButton.SetActive(true);

            GameManager.Instance.curObj = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            block.SetFloat("_OutLine", 0.0f);
            spr.SetPropertyBlock(block);
            pressButton.SetActive(false);

            GameManager.Instance.curObj = null;
        }
    }


    //상호작용시 할 행동
    public abstract void InteractAction();


    public abstract void CancelAction();
}
