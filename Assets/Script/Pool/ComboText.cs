using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboText : DamageText
{
    protected override void OnEnable()
    {
        _tmp.text = $"{GameMGR.Inst.comboCount} Combo";
        if (GameMGR.Inst.isCritical)
        {
            _tmp.color = Color.red;
            _tmp.fontSize = 5;
        }
        else
        {
            _tmp.color = Color.white;
            _tmp.fontSize = 4;
        }

        StartCoroutine(CO_MoveUp());
    }

    protected override IEnumerator CO_MoveUp()
    {
        for (int i = 0; i < durTime * 100; i++)
        {
            transform.Translate(0, moveSpeed, 0);
            _tmp.color = new Color(1, 1, 1, 1 - (i * 0.01f));
            yield return null;
        }
        //durTime = 1;
        Pool_ComboText.Inst.DestroyPool(this);
    }

}
