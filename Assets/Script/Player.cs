using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [HideInInspector] public Rigidbody2D rb { get { return _rb; } }
    [SerializeField] public BoxCollider2D _col;
    [SerializeField] public Animator _anim;
    [SerializeField] private SpriteRenderer _sr;

    [SerializeField] public bool isGround = false;

    [SerializeField] public float jumpForce; //점프력
    [SerializeField] public float jumpMaxForce; //최대점프력
    [SerializeField] public float jumpGageSpeed; // 점프게이지 초당회복량 증가
    [SerializeField] private bool isInputJump = false;

    [SerializeField] private GameObject shieldTrigger;
    [SerializeField] private GameObject stunTrigger;
    [SerializeField] private GameObject barrierTrigger;

    [SerializeField] private ParticleSystem shieldEffect;
    [SerializeField] private GameObject jumpFever1;

    [SerializeField] private bool isJumpNow;
    private bool isHitWhenFly = false;
    public bool canControl = true; // false 일 경우 조작 불가


    [SerializeField] private Vector2 groundPos;
    private WaitForSeconds sc05 = new WaitForSeconds(0.5f);
    private WaitForSeconds sc1 = new WaitForSeconds(1f);
    private WaitForSeconds sc2 = new WaitForSeconds(2f);
    private WaitForSeconds sc5 = new WaitForSeconds(5f);

    #region 버튼 함수

    public void OnClick_Attack()
    {
        if (!canControl) return;
        if (GameMGR.Inst.skill_attack2On) Pool_PlayerAttack2.Inst.InstPool(transform.position);
        else GameMGR.Inst.effect.Effect_PlayerAttack(transform.position);
        _anim.SetTrigger("doAttack");

        GameMGR.Inst.ResetSliderPos(2);
    }

    public void OnClick_Jump(int amount = 0)
    {
        if (!canControl) return;
        if (isGround)
        {
            _rb.AddForce(new Vector2(0, 1 * jumpForce), ForceMode2D.Force);
            GameMGR.Inst.GetGage(0, amount);
        }
        //if (isGround) StartCoroutine(CO_Jump());    
    }

    public void PointerDown_Jump()
    {
        jumpForce = 0;
        isInputJump = true;
    }

    public void PointerUp_Jump()
    {
        isInputJump = false;
        OnClick_Jump();
    }

    public void OnClick_Shield()
    {
        if (!canControl) return;
        if (!GameMGR.Inst.canShieldNow) return;
        GameMGR.Inst.playerIsSheildNow = true;
        _anim.SetBool("isShield", true);
        if (GameMGR.Inst.skill_shield1On) stunTrigger.SetActive(true);
        else if (GameMGR.Inst.skill_shield2On) barrierTrigger.SetActive(true);
        else shieldTrigger.SetActive(true);
        //shieldEffect.SetActive(true);
        GameMGR.Inst.ShieldCoolStart();
        StartCoroutine(CO_ShieldDuration());
    }


    #endregion

    #region 점프 스킬 관련

    public void JumpFever1()
    {
        canControl = false;
        jumpFever1.SetActive(true);
        //gameObject.layer = LayerMask.NameToLayer("PlayerAttack");
        _rb.velocity = Vector2.zero;
        _rb.AddForce(new Vector2(0, 500));
        StartCoroutine(CO_JumpFever1());
    }

    IEnumerator CO_JumpFever1()
    {
        yield return sc2;
        canControl = true;
        jumpFever1.SetActive(false);
    }

    #endregion

    IEnumerator CO_ShieldDuration()
    {
        if (GameMGR.Inst.skill_shield1On)
        {
            GameMGR.Inst.effect.playerPtcls[0].Play();
            yield return sc1;
        }
        else if (GameMGR.Inst.skill_shield2On)
        {
            GameMGR.Inst.effect.playerPtcls[0].Play();
            _anim.SetTrigger("doSwing");
            yield return sc5;
        }
        else yield return sc05;
        //shieldEffect.SetActive(false);
        GameMGR.Inst.playerIsSheildNow = false;
        _anim.SetBool("isShield", false);
        shieldTrigger.SetActive(false);
        stunTrigger.SetActive(false);
        barrierTrigger.SetActive(false);
        GameMGR.Inst.skill_shield1On = false;
        GameMGR.Inst.skill_shield2On = false;
    }
    public void Hit(int damage = 1)
    {
        StartCoroutine(CO_HitRed());
        GameMGR.Inst.LifeUpdate(damage);
        GameMGR.Inst.comboCount = 0;

        if (!isGround)
        {
            ResetForce(-300);
        }
    }

    IEnumerator CO_HitRed()
    {
        _sr.color = Color.red;
        yield return sc05;
        _sr.color = Color.white;
    }

    public void ResetForce(int amount)
    {
        //_rb.velocity = Vector2.zero;
        _rb.AddForce(new Vector2(0, 1 * amount), ForceMode2D.Force);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            _anim.SetBool("isJump", false);
        }

        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Hit();
            if (isGround)
            {
                Pool_EnemyDieEffect.Inst.InstPool(collision.transform.position);
                collision.gameObject.GetComponent<Enemy>().wave.CheckClear();
                collision.gameObject.SetActive(false);
                GameMGR.Inst.comboCount = 0;
            }
            else
            {
                Debug.Log("점프 중에 부딪혔다");
            }

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            _anim.SetBool("isJump", true);
        }

    }
}
