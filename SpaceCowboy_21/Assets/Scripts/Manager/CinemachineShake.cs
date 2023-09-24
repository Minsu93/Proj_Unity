using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake instance { get; private set; }
    //씨네머신 카메라를 가져오고
    //거기다 perline 수치를 준다. 
    //update에서 일정 시간이 지난 후 수치를 0으로 바꾼다. 


    private float shakeTimer;
    float startingIntensity;
    float shakeTimerTotal;

    CinemachineVirtualCamera cinemachineVirtualCamera;
    private void Awake()
    {
        instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlin.m_AmplitudeGain = intensity;
        
        //shakeTimerTotal = time;
        shakeTimer = time;

    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            /*
            CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1-(shakeTimer/shakeTimerTotal));
            */

            
            if (shakeTimer <= 0)
            {
                CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_AmplitudeGain = 0f;
            }
            
        }
    }
}
