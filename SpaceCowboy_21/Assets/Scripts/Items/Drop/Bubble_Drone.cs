using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble_Drone : SelfCollectable
{
    [SerializeField] GameObject dronePrefab;
    //public event System.Action ConsumeAction;
    SpriteRenderer spr;
    protected override void Awake()
    {
        base.Awake();
        spr = GetComponentInChildren<SpriteRenderer>();
        //커스텀
        if (dronePrefab != null) SetDrone(dronePrefab);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //action 초기화.
        GameManager.Instance.arrowManager.CreateArrow(this.gameObject, 1);

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.Instance.arrowManager.RemoveArrow(this.gameObject, 1);
    }


    protected override bool ConsumeEvent()
    {
        return GameManager.Instance.playerManager.AddDrone(dronePrefab);
    }

    public void SetDrone(GameObject dronePrefab)
    {
        this.dronePrefab = dronePrefab;
        spr.sprite = dronePrefab.GetComponent<DroneItem>().sprite;
    }
}
