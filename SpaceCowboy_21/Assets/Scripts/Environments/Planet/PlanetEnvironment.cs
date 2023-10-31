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
    //행성에 구름이 존재합니까?
    public bool isCloudOn = false;
    public GameObject cloudPrefab;
    public GameObject cloud;   //구름 오브젝트를 담을 변수


    public void GeneratePlanet()
    {
        //spriteshape를 교체한다
        if(spriteShapeController == null)
        {
            spriteShapeController = GetComponent<SpriteShapeController>();
        }

        spriteShapeController.spriteShape = this.spriteShape;

        //행성 대기 범위와 중력 표시 여부를 결정한다



        //행성에 구름을 표시할 것인지 결정한다.

        if (isCloudOn)
        {
            if (!cloud)
            {
                var script = GetComponentInChildren<CloudRotation>();
                if (script != null)
                {
                    cloud = script.gameObject;
                }
                else
                {

                    cloud = Instantiate(cloudPrefab, transform);
                    cloud.transform.localScale = Vector3.one * planet.gravityRadius;
                }
                
            }

        }
        else
        {
            var script = GetComponentInChildren<CloudRotation>();
            if (script != null)
            {
                cloud = script.gameObject;
                DestroyImmediate(cloud);
                cloud = null;
            }

        }
    }

}
