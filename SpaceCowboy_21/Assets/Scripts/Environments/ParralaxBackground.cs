using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    //�ӵ��� �ݴ� �������� �̵��Ѵ�. 

    [SerializeField] private float speed;
    private Vector3 lastCameraPosition;

    //float defaultFOV;
    //Vector3 startScale;


    private void LateUpdate()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 deltaMovement = cameraPos - lastCameraPosition;
        lastCameraPosition = cameraPos;

        transform.Translate(new Vector3(deltaMovement.x, deltaMovement.y, 0) * -speed * Time.deltaTime, Space.World);
        
        ////��� ���� ���̽� ��ġ, ������ 
        //float curFOV = GameManager.Instance.cameraManager.currFOV;
        //float baseScaler;

        //baseScaler = defaultFOV / curFOV;
        //float a = 1 / baseScaler;      //a�� 1~2�� ���� ���´�. 
        //transform.localScale = startScale * a;
    }
}
