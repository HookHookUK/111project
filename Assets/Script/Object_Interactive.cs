using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  오브젝트의 기본적인 상호작용. 점차 사라지거나, 깜빡임 등 공통적으로 사용될 수 있는 부분들이 포함되어 있습니다.
/// </summary>
public class Object_Interactive : MonoBehaviour
{
    [Header("Object Interactive")]
    [SerializeField] protected SpriteRenderer _sr = null;

    protected WaitForSeconds sc1 = new WaitForSeconds(1f);
    protected WaitForSeconds sc05 = new WaitForSeconds(0.5f);
    protected WaitForSeconds sc01 = new WaitForSeconds(0.1f);

    protected Coroutine co_Disappear;
    [SerializeField] protected int disapearSpeed = 10;

    public void Disappear(bool isOn = true)
    {
        if (!isOn) StopCoroutine(co_Disappear);
        else
            co_Disappear = StartCoroutine(CO_Disappear());
    }

    protected virtual IEnumerator CO_Disappear()
    {
        for(int i = 100; i > 0; i-= disapearSpeed)
        {
            _sr.color = new Color(1, 1, 1, i * 0.01f);
            yield return null;
        }
        DestroySelf();
    }

    public virtual void DestroySelf()
    {
        gameObject.SetActive(false);
    }
}
