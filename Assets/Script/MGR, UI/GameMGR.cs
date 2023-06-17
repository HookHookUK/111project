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

    [Header("���� ����")]
    [SerializeField] public float gameSpeed = 1f; 

    [Header("�÷��̾� ����")]
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

    [SerializeField] public float shieldCool; // �ǵ� ��Ÿ��

    public int finalDamage = 0; // �ܺο��� ������ ������ ������ �� �ջ��Ͽ� ��ȯ�ϴ� ����������
    private int randCri = 0;
    public bool isCritical = false;

    [Header("�÷��̾� ����")]
    [SerializeField] public bool playerIsSheildNow;
    [SerializeField] public bool playerIsGroundNow;

    [SerializeField] public bool canShieldNow = true;

    [Header("�÷��̾� ��ų ������")]
    [SerializeField] public int jumpGage = 0; // ������ ������ ����
    [SerializeField] public int shieldGage = 0; // ���� ��밡�� ���¿��� �ð��� �������� ����
    [SerializeField] public int attackGage = 0; // ������ ������ ����


    private int maxGage = 2000; // ������ �ִ밪
    private int halfMaxGage = 1000;

    private float curShieldCool;

    [Header("����Ʈ ����")]

    [SerializeField] private Sprite[] playerMotions;

    [SerializeField] public EffectMGR effect;

    [Header("UI ����")]
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

    [Header("��ų �����̴�")]
    [SerializeField] private Slider[] slider;
    [SerializeField] private Image[] slider_Background; //�����̴� ���� ������
    [SerializeField] private Image[] slider_Fill; // �����̴� ���� ������
    [SerializeField] private Image[] slider_MaxIconColor; // �����̴� ������ ������ Ȱ��ȭ�Ǵ� �巡�� ���̵� ������ ����
    [SerializeField] private Image[] slider_MaxIcon; // �����̴� ������ ������ Ȱ��ȭ�Ǵ� �巡�� ���̵� ������

    [SerializeField] private Image uiSheild;
    [SerializeField] private Image feverShield1;
    [SerializeField] private Image feverShield2;

    [SerializeField] private Image feverJump1;
    [SerializeField] private Image feverJump2;

    [SerializeField] private Image feverAttack1;
    [SerializeField] private Image feverAttack2;

    [Header("�ΰ� ����")]
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

    #region ���� ���� ����
    public void StartSetting() // �������� ���� �� �� �ʱ�ȭ
    {
        jumpGage = 0;
        shieldGage = 0;
        attackGage = 0;
        shieldCool = 0;

    }
    #endregion

    #region ���� ��ȣ�ۿ�

    // �ð� ����
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

    // ����
    public int GetDamage() // �÷��̾� ������ ���� ������ �� ȣ��
    {
        randCri = Random.Range(0, 100);
        if(randCri < criticalPer)
        {
            // ũ���� �ջ�
            finalDamage = playerDamage + criticalDamage;
            isCritical = true;
            Debug.Log($"ũ���� {finalDamage}");
        }
        else
        {
            finalDamage = playerDamage;
            isCritical = false;
            Debug.Log($"��Ÿ {finalDamage}");
        }

        return finalDamage;
    }

    #region �÷��̾� ü�� ����
    public void ChangeMaxLife(int amount) // �ִ� ü�°� ���� ������ ���� �� 
    {
        if (amount > 0)
        {
            if (playerMaxHp == 5) return; // ���Ѽ�
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
    public void LifeUpdate(int amount, bool isPlus = false) // ü�� ������ ȣ�� �Լ�
    {
        if(isPlus)
        {
            // ȸ��
            if (playerCurHp == playerMaxHp) return;
            playerCurHp += amount;
            if (playerCurHp >= playerMaxHp) playerCurHp = playerMaxHp;
        }
        else
        {
            // ����
            playerCurHp -= amount;
            if(playerCurHp <= 0)
            {
                playerCurHp = 0;
                Debug.Log("���");
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

    #region �÷��̾� ��ų ����

    public void GetGage(int num, int amount = 0) // �������� ���� �� ȣ��Ǵ� �Լ�. num = ���� ����, amount = ��
    {
        Debug.Log($"{num} �������� �����");
        switch(num)
        {
            case 0: // ���� ������
                if (jumpGage >= maxGage) break;
                if (amount != 0) jumpGage += amount;
                else jumpGage += gain_Jump;
                if (jumpGage > maxGage) jumpGage = maxGage;
                break;
            case 1: // �ǵ� ������
                if (shieldGage >= maxGage) break;
                shieldGage += gain_Shield;
                if (shieldGage > maxGage) shieldGage = maxGage;
                break;
            case 2: // ���� ������
                if (attackGage >= maxGage) break;
                attackGage += gain_Attack;
                if (attackGage > maxGage) attackGage = maxGage;
                break;
        }
        GageUpdate(num);
    }

    public void GageUpdate(int num) // �ش� ������ �������� ���� ������ ȣ��Ǵ� �Լ�
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
    /// �������� �ٲ� ��ȣ, ������ ��
    /// </summary>
    /// <param name="num"></param>
    /// <param name="amount"></param>
    public void IconChange(int num, int amount)
    {
        switch(num) 
        {
            case 0: // ���� ������
                if (amount < halfMaxGage) buttonIcon[0].sprite = iconData[0];
                else if (amount < maxGage) buttonIcon[0].sprite = iconData[1];
                else buttonIcon[0].sprite = iconData[2];
                break;
            case 1: // �ǵ� ������
                if (amount < halfMaxGage) buttonIcon[1].sprite = iconData[3];
                else if (amount < maxGage) buttonIcon[1].sprite = iconData[4];
                else buttonIcon[1].sprite = iconData[5];
                break;
            case 2: // ���� ������
                if (amount < halfMaxGage) buttonIcon[2].sprite = iconData[6];
                else if (amount < maxGage) buttonIcon[2].sprite = iconData[7];
                else buttonIcon[2].sprite = iconData[8];
                break;
        }
    }

    public void SliderChange(int num) // ��ų �����̴��� ���� ���� ( ���� )
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

    public void SetMaxIcon(int num, int level) // ��ų ��� ���ɽ� ǥ��Ǵ� ������ ó�� �Լ�. num�� ��ȣ. level�� ������ ��ġ.
    {
        switch(level)
        {
            case 0: //��Ȱ��ȭ
                slider_MaxIconColor[num].enabled = false;
                slider_MaxIcon[num].enabled = false;
                break;
            case 1: //1�ܰ� Ȱ��ȭ
                slider_MaxIconColor[num].enabled = true;
                slider_MaxIcon[num].enabled = true;
                slider_MaxIconColor[num].color = fever1_1;
                slider_MaxIcon[num].sprite = buttonIcon[num].sprite; // ��ư ������ ���� �ٲٰ� �� �Լ��� ���´ٸ� �̰ɷ� �� ��.
                break;
            case 2: //2�ܰ� Ȱ��ȭ
                slider_MaxIconColor[num].enabled = true;
                slider_MaxIcon[num].enabled = true;
                slider_MaxIconColor[num].color = fever2_1;
                slider_MaxIcon[num].sprite = buttonIcon[num].sprite;
                break;
        }
    }

    #region ���� ��ų
    // ��ų ���
    public void ActSkill(int type, int level) // type = ���� / �ǵ� / ����, level = ��ų ���
    {
        switch(type)
        {
            case 0: //������ų
                if(level == 1)
                {
                    Debug.Log("���� ��ų1 ���");
                    StartCoroutine(CO_Skill_Jump1());
                    jumpGage -= halfMaxGage;
                    GageUpdate(0);
                }
                else
                {
                    Debug.Log("���� ��ų2 ���");
                    StartCoroutine(CO_Skill_Jump2());
                    jumpGage = 0;
                    GageUpdate(0);
                }    
                break;
            case 1:
                if (level == 1)
                {
                    Debug.Log("�ǵ� ��ų1 ���");
                    skill_shield1On = true;
                    player.OnClick_Shield();
                    shieldGage -= halfMaxGage;
                    GageUpdate(1);

                }
                else
                {
                    Debug.Log("�ǵ� ��ų2 ���");
                    skill_shield2On = true;
                    player.OnClick_Shield();
                    shieldGage = 0;
                    GageUpdate(1);
                }
                break;
            case 2:
                if (level == 1)
                {
                    Debug.Log("���� ��ų1 ���");
                    StartCoroutine(CO_Skill_Attack1());
                    attackGage -= halfMaxGage;
                    GageUpdate(2);
                }
                else
                {
                    Debug.Log("���� ��ų2 ���");
                    StartCoroutine(CO_Skill_Attack2());
                    attackGage = 0;
                    GageUpdate(2);
                }
                break;
        }
        slider[type].value = 0;
    }

    // ���� ��ų
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

    // ��� ��ų


    // ���� ��ų

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

    // �ǵ� ����
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
