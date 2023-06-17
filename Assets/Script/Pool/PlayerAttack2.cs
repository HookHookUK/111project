using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack2 : Object_Interactive
{
    [SerializeField] private int upSpeed;

    private Color ownColor = new Color(0.3f, 1, 0.9f, 1);
    protected virtual void OnEnable()
    {
        _sr.color = ownColor;
        StartCoroutine(CO_StartDisappear());
        StartCoroutine(CO_MoveUp());
    }

    protected virtual IEnumerator CO_StartDisappear()
    {
        yield return sc05;
        Disappear();
    }

    protected override IEnumerator CO_Disappear()
    {
        for (int i = 100; i > 0; i -= disapearSpeed)
        {
            _sr.color = new Color(0.3f, 1, 0.9f, i * 0.01f);
            yield return null;
        }
        DestroySelf();
    }

    protected IEnumerator CO_MoveUp()
    {
        while(true)
        {
            transform.Translate(0, upSpeed * 0.01f, 0);
            yield return null;
        }
    }

    public override void DestroySelf()
    {
        Pool_PlayerAttack2.Inst.DestroyPool(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().Hit();
        }
    }

}
