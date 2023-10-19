using PlanetSpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;



public class PlanetEnvironment : MonoBehaviour
{

    [Header("Planet")]
    public Planet planet;
    SpriteShapeController spriteShapeController;
    public SpriteShape spriteShape;

    [Header("Cloud")]
    //�༺�� ������ �����մϱ�?
    public bool isCloudOn = false;
    public GameObject cloudPrefab;
    public GameObject cloud;   //���� ������Ʈ�� ���� ����



    public void GeneratePlanet()
    {
        //spriteshape�� ��ü�Ѵ�
        if(spriteShapeController == null)
        {
            spriteShapeController = GetComponent<SpriteShapeController>();
        }

        spriteShapeController.spriteShape = this.spriteShape;

        //�༺ ��� ������ �߷� ǥ�� ���θ� �����Ѵ�



        //�༺�� ������ ǥ���� ������ �����Ѵ�.

        if (isCloudOn)
        {
            if (!cloud)
            {
                cloud = Instantiate(cloudPrefab, transform);
                cloud.transform.localScale = Vector3.one * planet.gravityRadius;
            }

        }
        else
        {
            DestroyImmediate(cloud);
        }
    }

}
