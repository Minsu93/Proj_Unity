using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.SpawnPlayer();
        Vector2 spawnPos = transform.position + (transform.up * 2f);
        GameManager.Instance.SpawnShuttle(spawnPos, Quaternion.identity) ;
    }
}
