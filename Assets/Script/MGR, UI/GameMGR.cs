using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMGR : MonoBehaviour
{
    #region Singleton
    private static GameMGR instance;
    public static GameMGR Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameMGR>();
                if (instance == null)
                {
                    instance = new GameObject("GameMGR").AddComponent<GameMGR>();
                }
            }
            return instance;
        }
    }
    #endregion

    [Header("게임 관리")]
    [SerializeField] public float gameSpeed = 1f; 

    [Header("플레이어 정보")]
    [SerializeField] public Player player;
    [SerializeField] public int playerDamage = 100;
    [SerializeField] public int playerMaxHp = 3;
    [SerializeField] public int playerCurHp = 3;
    [SerializeField] public int gain_Jump = 100;
    [SerializeField] public int gain_Shield = 100;
    [SerializeField] public int gain_Attack = 100;
    [SerializeField] public int criticalPer = 5;
    [SerializeField] public int criticalDamage = 100;
    [SerializeField] public int knockBackPower = 100;
    [SerializeField] public int getGoldAmount = 100;

    [SerializeField] public float shieldCool; // 실드 쿨타임

    public int finalDamage = 0; // 외부에서 데미지 정보를 가져올 때 합산하여 반환하는 최종데미지
    private int randCri = 0;
    public bool isCritical = false;

    [Header("플레이어 상태")]
    [SerializeField] public bool playerIsSheildNow;
    [SerializeField] public bool playerIsGroundNow;

    [SerializeField] public bool canShieldNow = true;

    [Header("플레이어 스킬 게이지")]
    [SerializeField] public int jumpGage = 0; // 점프할 때마다 찬다
    [SerializeField] public int shieldGage = 0; // 쉴드 사용가능 상태에서 시간이 지날수록 찬다
    [SerializeField] public int attackGage = 0; // 공격할 때마다 찬다


    private int maxGage = 2000; // 게이지 최대값
    private int halfMaxGage = 1000;

    private float curShieldCool;

    [Header("이펙트 관리")]

    [SerializeField] private Sprite[] playerMotions;

    [SerializeField] public EffectMGR effect;

    [Header("UI 관리")]
    [SerializeField] private Sprite[] iconData;
    [SerializeField] private Image[] buttonIcon;
    [SerializeField] private Image[] life;

    private Color alpha0 = new Color(1, 1, 1, 0);
    private Color alpha05 = new Color(1, 1, 1, 0.5f);
    private Color alpha1 = new Color(1, 1, 1, 1f);

    private Color fever1_05 = new Color(0, 0.7f, 1, 0.5f);
    private Color fever2_05 = new Color(0.7f, 0, 1, 0.5f);

    private Color fever1_1 = new Color(0, 0.7f, 1, 1);
    private Color fever2_1 = new Color(0.7f, 0, 1, 1);

    private Color stunColor = new Color(0.7f, 0.7f, 0.7f, 1);

    [Header("스킬 슬라이더")]
    [SerializeField] private Slider[] slider;
    [SerializeField] private Image[] slider_Background; //슬라이더 투명도 관리용
    [SerializeField] private Image[] slider_Fill; // 슬라이더 투명도 관리용
    [SerializeField] private Image[] slider_MaxIconColor; // 슬라이더 게이지 충전시 활성화되는 드래그 가이드 아이콘 배경색
    [SerializeField] private Image[] slider_MaxIcon; // 슬라이더 게이지 충전시 활성화되는 드래그 가이드 아이콘

    [SerializeField] private Image uiSheild;
    [SerializeField] private Image feverShield1;
    [SerializeField] private Image feverShield2;

    [SerializeField] private Image feverJump1;
    [SerializeField] private Image feverJump2;

    [SerializeField] private Image feverAttack1;
    [SerializeField] private Image feverAttack2;

    [Header("부가 관리")]
    [SerializeField] public RoundMGR roundMGR;
    [SerializeField] public GameObject ui;

    private WaitForSeconds sc1 = new WaitForSeconds(1f);
    private WaitForSeconds sc2 = new WaitForSeconds(2f);
    private WaitForSeconds sc3 = new WaitForSeconds(3f);
    private WaitForSeconds sc5 = new WaitForSeconds(5f);
    private WaitForSeconds sc05 = new WaitForSeconds(0.5f);
    private WaitForSeconds sc01 = new WaitForSeconds(0.1f);

    public bool skill_attack2On = false;
    public bool skill_shield1On = false;
    public bool skill_shield2On = false;


    public bool playerInvincible = false;
    public int comboCount = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            attackGage += 1000;
            GageUpdate(2);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            jumpGage += 1000;
            GageUpdate(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            shieldGage += 1000;
            GageUpdate(1);
        }
    }

    #region 게임 진행 관련
    public void StartSetting() // 스테이지 시작 전 값 초기화
    {
        jumpGage = 0;
        shieldGage = 0;
        attackGage = 0;
        shieldCool = 0;

    }
    #endregion

    #region 게임 상호작용

    // 시간 조절
    public void SetSpeed(float num = 1f)
    {
        gameSpeed = num;
        player.rb.gravityScale = num;
        if(num == 0)
        {
            player.rb.velocity = Vector2.zero;
        }
    }
    public void StopWave()
    {
        gameSpeed = 0;
        StartCoroutine(CO_StopWave());
    }

    IEnumerator CO_StopWave()
    {
        yield return sc3;
        gameSpeed = 1;
    }

    // 공격
    public int GetDamage() // 플레이어 데미지 값을 가져올 때 호출
    {
        randCri = Random.Range(0, 100);
        if(randCri < criticalPer)
        {
            // 크리뎀 합산
            finalDamage = playerDamage + criticalDamage;
            isCritical = true;
            Debug.Log($"크리뎀 {finalDamage}");
        }
        else
        {
            finalDamage = playerDamage;
            isCritical = false;
            Debug.Log($"평타 {finalDamage}");
        }

        return finalDamage;
    }

    #region 플레이어 체력 관련
    public void ChangeMaxLife(int amount) // 최대 체력값 변동 사항이 있을 때 
    {
        if (amount > 0)
        {
            if (playerMaxHp == 5) return; // 상한선
            playerMaxHp++;
            playerCurHp++;
        }
        else
        {
            if (playerMaxHp == 1) return;
            playerMaxHp--;
            playerCurHp--;
        }
        SetLifeUI();
    }
    public void LifeUpdate(int amount, bool isPlus = false) // 체력 변동시 호출 함수
    {
        if(isPlus)
        {
            // 회복
            if (playerCurHp == playerMaxHp) return;
            playerCurHp += amount;
            if (playerCurHp >= playerMaxHp) playerCurHp = playerMaxHp;
        }
        else
        {
            // 차감
            playerCurHp -= amount;
            if(playerCurHp <= 0)
            {
                playerCurHp = 0;
                Debug.Log("사망");
            }
        }
        SetLifeUI();
    }
    private void SetLifeUI()
    {
        for(int i = 0; i < playerCurHp; i++)
        {
            life[i].enabled = true;
        }
        for(int i = playerMaxHp - 1; i >= playerCurHp; i--)
        {
            life[i].enabled = false;
        }
    }
    #endregion

    #endregion

    #region 플레이어 스킬 관련

    public void GetGage(int num, int amount = 0) // 게이지를 얻을 때 호출되는 함수. num = 얻은 종류, amount = 양
    {
        Debug.Log($"{num} 게이지를 얻었다");
        switch(num)
        {
            case 0: // 점프 게이지
                if (jumpGage >= maxGage) break;
                if (amount != 0) jumpGage += amount;
                else jumpGage += gain_Jump;
                if (jumpGage > maxGage) jumpGage = maxGage;
                break;
            case 1: // 실드 게이지
                if (shieldGage >= maxGage) break;
                shieldGage += gain_Shield;
                if (shieldGage > maxGage) shieldGage = maxGage;
                break;
            case 2: // 공격 게이지
                if (attackGage >= maxGage) break;
                attackGage += gain_Attack;
                if (attackGage > maxGage) attackGage = maxGage;
                break;
        }
        GageUpdate(num);
    }

    public void GageUpdate(int num) // 해당 게이지 변동사항 있을 때마다 호출되는 함수
    {
        switch(num)
        {
            case 0:
                if (jumpGage >= halfMaxGage) feverJump1.fillAmount = 1;
                else feverJump1.fillAmount = jumpGage * 0.001f;
                if (jumpGage >= maxGage) feverJump2.fillAmount = 1;
                else feverJump2.fillAmount = (jumpGage - halfMaxGage) * 0.001f;
                IconChange(num, jumpGage);
                SliderChange(num);
                break;
            case 1:
                if (shieldGage >= halfMaxGage) feverShield1.fillAmount = 1;
                else feverShield1.fillAmount = shieldGage * 0.001f;
                if (shieldGage >= maxGage) feverShield2.fillAmount = 1;
                else feverShield2.fillAmount = (shieldGage - halfMaxGage) * 0.001f;
                IconChange(num, shieldGage);
                SliderChange(num);
                break;
            case 2:
                if (attackGage >= halfMaxGage) feverAttack1.fillAmount = 1;
                else feverAttack1.fillAmount = attackGage * 0.001f;
                if (attackGage >= maxGage) feverAttack2.fillAmount = 1;
                else feverAttack2.fillAmount = (attackGage - halfMaxGage) * 0.001f;
                IconChange(num, attackGage);
                SliderChange(num);
                break;

        }
    }

    /// <summary>
    /// 아이콘을 바꿀 번호, 게이지 양
    /// </summary>
    /// <param name="num"></param>
    /// <param name="amount"></param>
    public void IconChange(int num, int amount)
    {
        switch(num) 
        {
            case 0: // 점프 아이콘
                if (amount < halfMaxGage) buttonIcon[0].sprite = iconData[0];
                else if (amount < maxGage) buttonIcon[0].sprite = iconData[1];
                else buttonIcon[0].sprite = iconData[2];
                break;
            case 1: // 실드 아이콘
                if (amount < halfMaxGage) buttonIcon[1].sprite = iconData[3];
                else if (amount < maxGage) buttonIcon[1].sprite = iconData[4];
                else buttonIcon[1].sprite = iconData[5];
                break;
            case 2: // 공격 아이콘
                if (amount < halfMaxGage) buttonIcon[2].sprite = iconData[6];
                else if (amount < maxGage) buttonIcon[2].sprite = iconData[7];
                else buttonIcon[2].sprite = iconData[8];
                break;
        }
    }

    public void SliderChange(int num) // 스킬 슬라이더의 상태 변경 ( 투명도 )
    {
        switch (num)
        {
            case 0:
                if (jumpGage < halfMaxGage)
                {
                    slider[num].value = 0;
                    slider[num].enabled = false;
                    slider_Background[num].color = alpha0;
                    SetMaxIcon(0, 0);
                }
                else if (jumpGage < maxGage)
                {
                    slider[num].enabled = true;
                    slider_Background[num].color = fever1_05;
                    SetMaxIcon(0, 1);
                    if (slider[num].value > 0.4f)
                    {
                        ActSkill(0, 1);
                    }
                    else slider[num].value = 0;
                }
                else
                {
                    slider_Background[num].color = fever2_05;
                    SetMaxIcon(0, 2);
                    if (slider[num].value > 0.7f) ActSkill(0, 2) ;
                    else slider[num].value = 0;
                }
                break;
            case 1:
                if (shieldGage < halfMaxGage)
                {
                    slider[num].value = 0;
                    slider[num].enabled = false;
                    slider_Background[num].color = alpha0;
                    SetMaxIcon(1, 0);
                }
                else if (shieldGage < maxGage)
                {
                    slider[num].enabled = true;
                    slider_Background[num].color = fever1_05;
                    SetMaxIcon(1, 1);
                    if (slider[num].value > 0.7f) { ActSkill(1, 1); }
                    else slider[num].value = 0;
                }
                else
                {
                    slider_Background[num].color = fever2_05;
                    SetMaxIcon(1, 2);
                    if (slider[num].value > 0.7f) ActSkill(1, 2); 
                    else slider[num].value = 0;
                }
                break;
            case 2:
                if (attackGage < halfMaxGage)
                {
                    slider[num].value = 0;
                    slider[num].enabled = false;
                    slider_Background[num].color = alpha0;
                    SetMaxIcon(2, 0);
                }
                else if (attackGage < maxGage)
                {
                    slider[num].enabled = true;
                    slider_Background[num].color = fever1_05;
                    SetMaxIcon(2, 1);
                    if (slider[num].value > 0.7f)
                    {
                        ActSkill(2, 1);
                        //HandlerMoveSelf(2, 1);
                    }
                    else slider[num].value = 0;
                }
                else
                {
                    slider_Background[num].color = fever2_05;
                    SetMaxIcon(2, 2);
                    if (slider[num].value > 0.7f)
                    {
                        ActSkill(2, 2);
                        //HandlerMoveSelf(2, 2);
                    }
                    else slider[num].value = 0;
                }
                break;
        }
    }

    public void ResetSliderPos(int num)
    {
        slider[num].value = 0;
    }

    public void SetMaxIcon(int num, int level) // 스킬 사용 가능시 표기되는 아이콘 처리 함수. num은 번호. level은 게이지 수치.
    {
        switch(level)
        {
            case 0: //비활성화
                slider_MaxIconColor[num].enabled = false;
                slider_MaxIcon[num].enabled = false;
                break;
            case 1: //1단계 활성화
                slider_MaxIconColor[num].enabled = true;
                slider_MaxIcon[num].enabled = true;
                slider_MaxIconColor[num].color = fever1_1;
                slider_MaxIcon[num].sprite = buttonIcon[num].sprite; // 버튼 아이콘 먼저 바꾸고 이 함수로 들어온다면 이걸로 될 것.
                break;
            case 2: //2단계 활성화
                slider_MaxIconColor[num].enabled = true;
                slider_MaxIcon[num].enabled = true;
                slider_MaxIconColor[num].color = fever2_1;
                slider_MaxIcon[num].sprite = buttonIcon[num].sprite;
                break;
        }
    }

    #region 세부 스킬
    // 스킬 사용
    public void ActSkill(int type, int level) // type = 점프 / 실드 / 공격, level = 스킬 등급
    {
        switch(type)
        {
            case 0: //점프스킬
                if(level == 1)
                {
                    Debug.Log("점프 스킬1 사용");
                    StartCoroutine(CO_Skill_Jump1());
                    jumpGage -= halfMaxGage;
                    GageUpdate(0);
                }
                else
                {
                    Debug.Log("점프 스킬2 사용");
                    StartCoroutine(CO_Skill_Jump2());
                    jumpGage = 0;
                    GageUpdate(0);
                }    
                break;
            case 1:
                if (level == 1)
                {
                    Debug.Log("실드 스킬1 사용");
                    skill_shield1On = true;
                    player.OnClick_Shield();
                    shieldGage -= halfMaxGage;
                    GageUpdate(1);

                }
                else
                {
                    Debug.Log("실드 스킬2 사용");
                    skill_shield2On = true;
                    player.OnClick_Shield();
                    shieldGage = 0;
                    GageUpdate(1);
                }
                break;
            case 2:
                if (level == 1)
                {
                    Debug.Log("공격 스킬1 사용");
                    StartCoroutine(CO_Skill_Attack1());
                    attackGage -= halfMaxGage;
                    GageUpdate(2);
                }
                else
                {
                    Debug.Log("공격 스킬2 사용");
                    StartCoroutine(CO_Skill_Attack2());
                    attackGage = 0;
                    GageUpdate(2);
                }
                break;
        }
        slider[type].value = 0;
    }

    // 점프 스킬
    IEnumerator CO_Skill_Jump1()
    {
        player.canControl = false;
        player._anim.SetTrigger("doCharging");
        SetSpeed(0);
        effect.playerPtcls[0].Play();
        yield return sc05;
        player._anim.SetTrigger("doJump");
        yield return sc01;
        SetSpeed(1);
        player.ResetForce(800);
        effect.obj_JumpFever1.SetActive(true);
        playerInvincible = true;
        player._col.isTrigger = true;
        yield return sc1;
        player.canControl = true;
        effect.obj_JumpFever1.SetActive(false);
        playerInvincible = false;
        player._col.isTrigger = false;
    }

    IEnumerator CO_Skill_Jump2()
    {
        player.canControl = false;
        player._anim.SetTrigger("doCharging");
        SetSpeed(0);
        effect.playerPtcls[0].Play();
        yield return sc05;
        effect.obj_JumpFever2.SetActive(true);
        player._anim.SetTrigger("doJump");
        playerInvincible = true;
        player._col.isTrigger = true;
        yield return sc01;
        SetSpeed(1);
        player.ResetForce(1500);
        yield return sc3;
        player.canControl = true;
        effect.obj_JumpFever2.SetActive(false);
        playerInvincible = false;
        player._col.isTrigger = false;
    }

    // 방어 스킬


    // 공격 스킬

    IEnumerator CO_Skill_Attack1()
    {
        player.canControl = false;
        player._anim.SetTrigger("doCharging");
        SetSpeed(0);
        effect.playerPtcls[0].Play();
        yield return sc05;
        player._anim.SetTrigger("doSwing");
        Pool_PlayerAttack2.Inst.InstPool(player.transform.position);
        yield return sc01;
        SetSpeed(1);
        player.canControl = true;
    }

    IEnumerator CO_Skill_Attack2()
    {
        player.canControl = false;
        player._anim.SetTrigger("doCharging");
        SetSpeed(0);
        effect.playerPtcls[1].Play();
        yield return sc05;
        player._anim.SetTrigger("doSwing");
        player.canControl = true;
        skill_attack2On = true;
        effect.playerPtcls_stay[0].Play();
        SetSpeed(1);
        yield return sc5;
        skill_attack2On = false;
        effect.playerPtcls_stay[0].Stop();
    }


    #endregion

    // 실드 관련
    public void ShieldCoolStart()
    {
        canShieldNow = false;
        StartCoroutine(CO_ShieldCool());
    }

    IEnumerator CO_ShieldCool()
    {
        for(curShieldCool = 0; curShieldCool < shieldCool; curShieldCool += 0.01f)
        {
            uiSheild.fillAmount = (Mathf.Lerp(0, 100, curShieldCool / shieldCool) / 100);
            yield return null;
        }
        canShieldNow = true;
        curShieldCool = 0;
    }

    #endregion
}
