using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulletdrop : MonoBehaviour
{
    [Range(1, 2)]
    public int gunIndex = 1;

    public float delayForDestroy = 2.0f;

    SpriteRenderer spr;
    Rigidbody2D rb;
    PlayerWeapon weapon;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GameManager.Instance.player.GetComponent<PlayerWeapon>();  
    }

    private void OnEnable()
    {
        //·£´ý È¸Àü°ª
        float rot = Random.Range(-0.3f, 0.3f);
        rb.AddTorque(rot, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ConsumeFuction();
        }

        if (collision.CompareTag("Planet"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
    }

    void ConsumeFuction()
    {
        weapon.FillArtifactGauge(gunIndex);

        StartCoroutine(DestroyFuction(delayForDestroy));

    }

    IEnumerator DestroyFuction(float delay)
    {
        spr.enabled = false;
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
