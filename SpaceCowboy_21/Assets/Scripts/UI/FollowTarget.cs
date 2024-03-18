using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    /// <summary>
    /// 오브젝트의 위치를 타겟 위치와 동일하게 만든다. 
    /// </summary>
    ///

    public GameObject target;
    public Vector3 localPos;


    void LateUpdate()
    {
        if(target != null)
        {
            transform.position = target.transform.position + localPos;
        }

    }
}
