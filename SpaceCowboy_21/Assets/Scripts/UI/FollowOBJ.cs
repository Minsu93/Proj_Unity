using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOBJ : MonoBehaviour
{
    public GameObject followingObj;

    private void FixedUpdate()
    {
        if (followingObj == null)
            return;

        this.transform.position = Camera.main.WorldToScreenPoint(followingObj.transform.position);
    }
}
