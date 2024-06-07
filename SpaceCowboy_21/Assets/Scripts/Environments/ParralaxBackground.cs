using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    //속도의 반대 방향으로 이동한다. 

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
        
        ////렌즈에 따른 베이스 위치, 스케일 
        //float curFOV = GameManager.Instance.cameraManager.currFOV;
        //float baseScaler;

        //baseScaler = defaultFOV / curFOV;
        //float a = 1 / baseScaler;      //a는 1~2의 값을 갖는다. 
        //transform.localScale = startScale * a;
    }
}
