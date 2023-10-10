using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake instance { get; private set; }
    //���׸ӽ� ī�޶� ��������
    //�ű�� perline ��ġ�� �ش�. 
    //update���� ���� �ð��� ���� �� ��ġ�� 0���� �ٲ۴�. 


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
