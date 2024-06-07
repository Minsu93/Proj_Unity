using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOBJ : MonoBehaviour
{
    public GameObject followingObj;

    private void LateUpdate()
    {
        if (followingObj == null)
            return;

        this.transform.position = Camera.main.WorldToScreenPoint(followingObj.transform.position);
    }
}
