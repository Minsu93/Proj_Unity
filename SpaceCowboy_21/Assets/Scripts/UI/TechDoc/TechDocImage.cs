using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechDocImage : MonoBehaviour
{
    [SerializeField] int itemID;
    [SerializeField] private UIController controller;

    private void Awake()
    {
        if(controller != null)
        {
            controller.ImageUpdate += UpdateItemData;
        }
    }
    public void UpdateItemData()
    {
        Item item = GameManager.Instance.techDocument.GetItem(itemID);
        Debug.Log(itemID + " : is updated");
    }
}
