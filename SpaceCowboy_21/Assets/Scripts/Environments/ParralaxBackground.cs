using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [SerializeField] private float parralaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    float defaultFOV;
    Vector3 startScale;



    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        transform.position = new Vector2(lastCameraPosition.x, lastCameraPosition.y);

        startScale = transform.localScale;
        defaultFOV = GameManager.Instance.cameraManager.defaultFOV;

        //���� ��ġ ��¦ ����ֱ�

    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        transform.position += deltaMovement * parralaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;



        ////��� ���� ���̽� ��ġ, ������ 
        //float curFOV = GameManager.Instance.cameraManager.currFOV;
        //float baseScaler;

        //baseScaler = defaultFOV / curFOV;
        //float a = 1 / baseScaler;      //a�� 1~2�� ���� ���´�. 
        //transform.localScale = startScale * a;


    }
}
