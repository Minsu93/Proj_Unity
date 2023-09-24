using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [SerializeField] private float parralaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        Sprite spr = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = spr.texture;
        textureUnitSizeX = texture.width / spr.pixelsPerUnit;
        textureUnitSizeY = texture.height / spr.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        transform.position -= deltaMovement * parralaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;


        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }

        if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
        {
            float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
        }
    }
}
