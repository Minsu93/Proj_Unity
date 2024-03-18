using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    /// <summary>
    /// ������Ʈ�� ��ġ�� Ÿ�� ��ġ�� �����ϰ� �����. 
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
