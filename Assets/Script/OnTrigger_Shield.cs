using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger_Shield : MonoBehaviour
{
    [SerializeField] private ParticleSystem shieldEffect;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            shieldEffect.Play();
            GameMGR.Inst.playerIsSheildNow = false;
            _rb.velocity = Vector2.zero;
            if (!player.isGround) _rb.AddForce(new Vector2(0, -1 * 300), ForceMode2D.Force);
        }
    }
}
