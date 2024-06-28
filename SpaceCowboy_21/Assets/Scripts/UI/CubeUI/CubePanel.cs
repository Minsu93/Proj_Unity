using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePanel : MonoBehaviour
{
    //큐브가 담길 슬롯
    [SerializeField] private GameObject cubeSlot;
    //큐브
    [SerializeField] private GameObject cube;
    //캔버스
    [SerializeField] private Canvas canvas;

    //큐브 안에 들어갈 내용
    [SerializeField] private CubeData[] cubeData;



    private void Awake()
    {
        SpawnCube(0);
        SpawnCube(1);
        SpawnCube(2);
    }

    //생성
    void SpawnCube(int index)
    {
        GameObject slot = Instantiate(cubeSlot, this.transform);
        GameObject obj = Instantiate(cube, slot.transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        obj.GetComponent<CubeUI>().InitializeCube(canvas, rect, cubeData[index].cube);
    }

    //제거
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
