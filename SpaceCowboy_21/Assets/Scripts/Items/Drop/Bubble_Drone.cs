using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble_Drone : SelfCollectable
{
    [SerializeField] GameObject dronePrefab;
    public event System.Action ConsumeAction;
    SpriteRenderer spr;
    protected override void Awake()
    {
        base.Awake();
        spr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ConsumeAction = null;
    }

    protected override bool ConsumeEvent()
    {
        if (ConsumeAction != null) ConsumeAction();

        return GameManager.Instance.playerManager.AddDrone(dronePrefab);
    }

    public void SetDrone(GameObject dronePrefab)
    {
        this.dronePrefab = dronePrefab;
        spr.sprite = dronePrefab.GetComponent<DroneItem>().icon;
    }
}
