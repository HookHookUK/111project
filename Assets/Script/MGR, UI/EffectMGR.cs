using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMGR : MonoBehaviour
{
    #region Singleton
    private static EffectMGR instance;
    public static EffectMGR Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EffectMGR>();
                if (instance == null)
                {
                    instance = new GameObject("EffectMGR").AddComponent<EffectMGR>();
                }
            }
            return instance;
        }
    }
    #endregion

    [Header("���")]
    [SerializeField] private GameObject player;

    [Header("��� ��ƼŬ")]
    [SerializeField] public ParticleSystem[] playerPtcls;
    [SerializeField] public ParticleSystem[] playerPtcls_stay;

    [Header("������ ȿ��")]
    [SerializeField] public GameObject obj_JumpFever1;
    [SerializeField] public GameObject obj_JumpFever2;

    [Header("Ǯ��")]
    [SerializeField] public Pool_PlayerAttack pool_attack;
    [SerializeField] public Pool_EnemyDieEffect pool_enemyDie;
    [SerializeField] public Pool_ComboText pool_comboText;
    [SerializeField] public Pool_DamageText pool_damageText;

    [SerializeField] private PlayerAttack effect_PlayerAttack; // �÷��̾� ���� ������

    [Header("��ǥ ����")]
    [SerializeField] private Vector2 attackPos = new Vector2(0, 2);

    private GameObject _tempObj;
    public void Effect_PlayerAttack(Vector2 pos)
    {
        pool_attack.InstPool(pos + attackPos, player);
        /*_tempObj = Instantiate(effect_PlayerAttack.gameObject, pos, Quaternion.identity);
        _tempObj.transform.SetParent(player.transform);
        _tempObj.transform.Translate(0, 1, 0);*/
    }
}
