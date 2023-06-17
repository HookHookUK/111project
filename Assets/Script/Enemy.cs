using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyWave wave;

    [SerializeField] private int resetHp;
    [SerializeField] private int hp;

    private bool isHitDelay = false;

    private Vector3 setPos1 = new Vector3(0,-0.5f, 0);

    public void OnEnable()
    {
        hp = resetHp;
    }


    public void Hit(int AttackType = 2) // �ǰݽ� (�⺻������ �ǰ� �޴� ����� �����̹Ƿ� 2
    {
        Debug.Log("ȣ�� Ƚ��");
        isHitDelay = true;
        hp -= GameMGR.Inst.GetDamage();
        Pool_DamageText.Inst.InstPool(transform.position);
        if (hp <= 0)
        {
            //����� ��
            wave.CheckClear(); // óġ ī��Ʈ++
            GameMGR.Inst.comboCount++;
            GameMGR.Inst.GetGage(2);
            Pool_EnemyDieEffect.Inst.InstPool(transform.position, EffectMGR.Inst.pool_enemyDie.gameObject);
            Pool_ComboText.Inst.InstPool(transform.position + setPos1, EffectMGR.Inst.pool_comboText.gameObject);
            isHitDelay = false;
            gameObject.SetActive(false);
        }
        isHitDelay = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameMGR.Inst.playerInvincible) Hit();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            wave.Start_MoveUp(0.4f, 5f);
            GameMGR.Inst.GetGage(1);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stun"))
        {
            Debug.Log("���� ����");
            wave.Start_MoveUp(0.4f, 5f);
            GameMGR.Inst.StopWave();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameMGR.Inst.playerIsSheildNow)
            {
                Debug.Log("�� : ����� �ǵ�");
                wave.Start_MoveUp(0.4f, 5f);
            }
        }
    }
}
