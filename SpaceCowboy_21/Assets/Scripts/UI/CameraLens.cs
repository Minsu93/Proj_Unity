using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLens : MonoBehaviour
{
    CinemachineVirtualCamera virtualCam;

    public float expandLens = 16.0f;
    public float reduceLens = 8.0f;
    public float controlSpeed = 5.0f;

    float currLens;
    float targetLens;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        currLens = reduceLens;
    }
    private void LateUpdate()
    {
        
        if(currLens != targetLens)
        {
           

            float lens = Mathf.Lerp(currLens, targetLens, Time.deltaTime * controlSpeed);
            currLens = lens;
            virtualCam.m_Lens.OrthographicSize = currLens;
        }

    }

    public void LensReduce()
    {
        //시야가 좁아진다
        //virtualCam.m_Lens.OrthographicSize = reduceLens;
        targetLens = reduceLens;

    }
    public void LensExpand()
    {
        //시야가 넓어진다
        //virtualCam.m_Lens.OrthographicSize = expandLens;
        targetLens = expandLens;

    }
}
