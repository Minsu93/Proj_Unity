using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetArrowRotater : MonoBehaviour
{
    [SerializeField] GameObject arm;
    private void Awake()
    {
        Planet planet = GetComponentInParent<Planet>();
        float radius = planet.planetRadius;
        arm.transform.localPosition = new Vector2(0, radius * 0.9f);
    }
    private void Update()
    {
        Transform playerTr = GameManager.Instance.player;
        if(playerTr!= null)
        {
            //방향 벡터 계산 
            Vector3 dirVec = playerTr.transform.position - this.transform.position;
            float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
            transform.rotation = rotation;
        }
    }
}
