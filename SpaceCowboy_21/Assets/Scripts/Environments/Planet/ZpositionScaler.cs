
using UnityEngine;

public class ZpositionScaler : MonoBehaviour
{
    public float zPos = 0f;

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos * 10f);
        transform.localScale = Vector3.one * (zPos);


    }
}
