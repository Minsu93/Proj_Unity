using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePanel : MonoBehaviour
{
    //ť�갡 ��� ����
    [SerializeField] private GameObject cubeSlot;
    //ť��
    [SerializeField] private GameObject cube;
    //ĵ����
    [SerializeField] private Canvas canvas;

    //ť�� �ȿ� �� ����
    [SerializeField] private CubeData[] cubeData;



    private void Awake()
    {
        SpawnCube(0);
        SpawnCube(1);
        SpawnCube(2);
    }

    //����
    void SpawnCube(int index)
    {
        GameObject slot = Instantiate(cubeSlot, this.transform);
        GameObject obj = Instantiate(cube, slot.transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        obj.GetComponent<CubeUI>().InitializeCube(canvas, rect, cubeData[index].cube);
    }

    //����
    void DeleteCube()
    {

    }


}

[System.Serializable]
public class CubeData
{
    public AlienTechCube cube;
    public float chance; 
}
