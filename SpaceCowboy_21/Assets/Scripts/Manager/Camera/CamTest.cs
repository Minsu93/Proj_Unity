using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] vCams;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeVCam(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeVCam(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeVCam(2);
        }
    }

    void ChangeVCam(int index)
    {
        foreach(var v in vCams)
        {
            v.Priority = 0;
        }

        vCams[index].Priority = 1;
    }
}
