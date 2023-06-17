using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Object_Interactive
{
    protected virtual void OnEnable()
    {
        StartCoroutine(CO_StartDisappear());
    }


    public void FlipX(bool isFlip = false)
    {
        _sr.flipX = isFlip;
    }

    protected virtual IEnumerator CO_StartDisappear()
    {
        yield return null;
        Disappear();
    }

    public override void DestroySelf()
    {
        Pool_PlayerAttack.Inst.DestroyPool(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
             collision.gameObject.GetComponent<Enemy>().Hit();
        }
    }

}
