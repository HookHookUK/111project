using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_EnemyDie : MonoBehaviour
{
    private WaitForSeconds sc2 = new WaitForSeconds(2f);
    private void OnEnable()
    {
        StartCoroutine(CO_SelfDestroy());
    }

    IEnumerator CO_SelfDestroy()
    {
        yield return sc2;
        Pool_EnemyDieEffect.Inst.DestroyPool(this);
    }
}
