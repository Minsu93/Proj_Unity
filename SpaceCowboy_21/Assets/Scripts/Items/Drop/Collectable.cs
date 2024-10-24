using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public ParticleSystem consumeEffect;
    protected Gravity gravity;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        gravity = GetComponent<Gravity>();
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void OnEnable()
    {
        return;
    }
    protected virtual void OnDisable()
    {
        return;
    }
    protected abstract bool ConsumeEvent();

}
