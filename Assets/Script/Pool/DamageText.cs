using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    protected float durTime = 1f;
    protected float moveSpeed = 0.005f;
    [SerializeField] protected TextMeshPro _tmp;

    protected virtual void OnEnable()
    {
        _tmp.text = GameMGR.Inst.finalDamage.ToString();
        if (GameMGR.Inst.isCritical)
        {
            _tmp.color = Color.red;
            _tmp.fontSize = 6;
        }
        else
        {
            _tmp.color = Color.white;
            _tmp.fontSize = 5;
        }

        StartCoroutine(CO_MoveUp());
    }

    protected virtual IEnumerator CO_MoveUp()
    {
        for(int i = 0; i < durTime * 100; i++)
        {
            transform.Translate(0, moveSpeed, 0);
            _tmp.color = new Color(1, 1, 1, 1 - (i * 0.01f));
            yield return null;
        }
        //durTime = 1;
        Pool_DamageText.Inst.DestroyPool(this);
    }
}
