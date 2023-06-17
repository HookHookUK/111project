using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundMGR : MonoBehaviour
{
    [SerializeField] private ItemMGR itemMGR;

    [SerializeField] private Sprite[] itemIcons;

    private int curRound = 0;
    public bool isRoundStart = false;
    

    [SerializeField] private EnemyWave[] _wavePrefab;
    public EnemyWave curWave;
    

    [SerializeField] private GameObject roundClearUI;
    [SerializeField] private GameObject buttonsUI;

    [Header("선택창 아이템 정보")]
    [SerializeField] private Image[] itemImage;
    [SerializeField] private TextMeshProUGUI[] itemName;
    [SerializeField] private TextMeshProUGUI[] itemDesc;

    private Item[] tempItem = new Item[3]; // 아이템 매니저에서 임시로 가져온 아이템 정보

    private bool randChooseDone = false;

    private void Awake()
    {
        StartCoroutine(CO_WaitLoadDone());
    }

    IEnumerator CO_WaitLoadDone()
    {
        yield return new WaitUntil(() => itemMGR.isLoadItemList == true);
        RoundEnd();
    }

    public void RoundStart()
    {
        Debug.Log($"{curRound} 라운드 시작");
        switch(curRound)
        {
            case 0:
                curWave = Instantiate(_wavePrefab[0], transform.position, Quaternion.identity);
                break;
            case 1:
                curWave = Instantiate(_wavePrefab[1], transform.position, Quaternion.identity);
                break;
            case 2:
                curWave = Instantiate(_wavePrefab[2], transform.position, Quaternion.identity);
                break;
            default:
                curWave = Instantiate(_wavePrefab[2], transform.position, Quaternion.identity);
                break;
        }
    }

    public void RoundEnd()
    {
        SetButton(false);
        SetChooseItemInfo(); // 포브스 선정 템 3개
        roundClearUI.SetActive(true); // 내부적으로 템 선정이 완료된 이후에 UI 출력
    }


    public void SetButton(bool isOn) // 플레이 버튼 UI 활성화 여부 함수
    {
        buttonsUI.SetActive(isOn);
    }

    public void SetChooseItemInfo() // 선택창에 뜨는 아이템 정보 불러오기 
    {
        for(int i = 0; i < 3; i++)
        {
            tempItem[i] = itemMGR.DropItem();
            itemImage[i].sprite = itemIcons[tempItem[i].ownNum];
            itemName[i].text = tempItem[i].itemName;
            itemDesc[i].text = tempItem[i].description;
        }
        randChooseDone = true;
    }

    public void ChoiceButton(int num) // 버튼 눌렀을 때 호출
    {
        itemMGR.DiscountItem(tempItem[num]);
        itemMGR.ResetChooseList();
        GetItemBuff(tempItem[num]);

        roundClearUI.SetActive(false);
        SetButton(true);

        curRound++;
        RoundStart();
    }

    private void GetItemBuff(Item item)
    {
        switch(item.effect)
        {
            case Effect.atkUp:
                GameMGR.Inst.playerDamage += item.amount;
                break;
            case Effect.cridamUp:
                GameMGR.Inst.criticalDamage += item.amount;
                break;
            case Effect.criPerUp:
                GameMGR.Inst.criticalPer += item.amount;
                break;
            case Effect.getGoldUp:
                GameMGR.Inst.getGoldAmount += item.amount;
                break;
            case Effect.gainUp_Attack:
                GameMGR.Inst.gain_Attack += item.amount;
                break;
            case Effect.gainUp_Jump:
                GameMGR.Inst.gain_Jump += item.amount;
                break;
            case Effect.gainUp_Shield:
                GameMGR.Inst.gain_Shield += item.amount;
                break;
            case Effect.knockBackUp:
                GameMGR.Inst.knockBackPower += item.amount;
                break;
            case Effect.restoreHp:
                GameMGR.Inst.LifeUpdate(5, true);
                break;
            case Effect.hpUp:
                GameMGR.Inst.ChangeMaxLife(1);
                break;
            case Effect.coolDown_Shield:
                if (GameMGR.Inst.shieldCool <= 0.5f) GameMGR.Inst.shieldCool = 0.5f;
                else GameMGR.Inst.shieldCool -= Mathf.Floor(item.amount * 0.03f);
                break;
        }
    }
}
