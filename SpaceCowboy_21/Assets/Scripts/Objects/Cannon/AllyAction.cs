using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AllyAction : MonoBehaviour
{
    protected bool _onAttackCool = false;
    public bool OnAttackCool { get { return _onAttackCool; } }

    public float attackCoolTime = 5f;
    float atTimer;

    public GameObject projectilePrefab;

    private void Update()
    {
        if (_onAttackCool)
        {
            atTimer += Time.deltaTime;
            if(atTimer > attackCoolTime )
            {
                atTimer = 0;
                _onAttackCool = false;
            }
        }
    }
    public abstract void Attack(Vector3 pos, Quaternion rot);
}
