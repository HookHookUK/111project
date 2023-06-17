using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] private float downSpeed;
    [SerializeField] private float upSpeed;
    [SerializeField] private float upHeight;

    [SerializeField] private int deadCount = 0;

    private float upCount = 0;

    private bool isKnockback = false;

    private Coroutine co_Down;
    private Coroutine co_Up;

    private WaitForSeconds sc01 = new WaitForSeconds(0.1f);
    private WaitForSeconds sc02 = new WaitForSeconds(0.2f);

    private void Awake()
    {
        Start_MoveDown();
    }

    public void CheckClear()
    {
        ++deadCount;
        if(deadCount == 30)
        {
            deadCount = 0;
            StartCoroutine(CO_PlayerMustOnGround());
        }
    }

    IEnumerator CO_PlayerMustOnGround()
    {
        yield return new WaitUntil(() => GameMGR.Inst.player.isGround == true);
        GameMGR.Inst.roundMGR.RoundEnd();
    }

    public void Start_MoveDown()
    {
        if (co_Down != null) StopCoroutine(co_Down);
        co_Down = StartCoroutine(CO_MoveDown());
    }

    IEnumerator CO_MoveDown()
    {
        while (!isKnockback)
        {
            transform.Translate(0, -GameMGR.Inst.gameSpeed * downSpeed, 0);
            yield return null;
        }
    }

    public void Start_MoveUp(float speed, float height)
    {
        //upSpeed = speed;
        //upHeight = height;
        upCount = 0;
        StopCoroutine(co_Down);
        co_Up = StartCoroutine(CO_MoveUp());
    }

    IEnumerator CO_MoveUp()
    {
        while(upCount < upHeight)
        {
            transform.Translate(0, GameMGR.Inst.gameSpeed * upSpeed, 0);
            yield return null;
            upCount++;
        }
        upCount = 0;

        // ªÛΩ¬ºº ∞®º“
        for(int i = 100; i > 0; i--)
        {
            transform.Translate(0, GameMGR.Inst.gameSpeed * (upSpeed * (i * 0.01f)), 0);
            yield return null;
        }
        // ∏ÿƒ©
        yield return sc01;
        // «œ∞≠ Ω√¿€
        for (int i = 0; i < 100; i++)
        {
            transform.Translate(0, -GameMGR.Inst.gameSpeed * (downSpeed * (i * 0.01f)), 0);
            yield return null;
        }
        Start_MoveDown();
    }


}
