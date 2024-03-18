using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Randomize : MonoBehaviour
{

    
    private void Awake()
    {
        //�������� ����ȭ
        float scale = Random.Range(0.3f, 1f);
        transform.localScale = Vector3.one * scale;

        //������ ����ȭ
        int degree = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, 0, degree);

    }
}
