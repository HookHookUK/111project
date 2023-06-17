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

    [Header("대상")]
    [SerializeField] private GameObject player;

    [Header("재생 파티클")]
    [SerializeField] public ParticleSystem[] playerPtcls;
    [SerializeField] public ParticleSystem[] playerPtcls_stay;

    [Header("생성형 효과")]
    [SerializeField] public GameObject obj_JumpFever1;
    [SerializeField] public GameObject obj_JumpFever2;

    [Header("풀링")]
    [SerializeField] public Pool_PlayerAttack pool_attack;
    [SerializeField] public Pool_EnemyDieEffect pool_enemyDie;
    [SerializeField] public Pool_ComboText pool_comboText;
    [SerializeField] public Pool_DamageText pool_damageText;

    [SerializeField] private PlayerAttack effect_PlayerAttack; // 플레이어 공격 프리팹

    [Header("좌표 고정")]
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
