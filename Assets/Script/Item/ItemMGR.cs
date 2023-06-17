using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMGR : MonoBehaviour
{
    private List<Item> itemList = new List<Item>(); // 전체 아이템 리스트


    private List<Item> itemList0 = new List<Item>(); // 0티어 아이템 리스트
    private List<Item> itemList1 = new List<Item>(); // 1티어 아이템 리스트
    private List<Item> itemList2 = new List<Item>(); // 2티어 아이템 리스트

    public bool isLoadItemList = false;

    private List<Item> choosList = new List<Item>();

    private int rand = 0;
    private int rand2 = 0;
    private int minNum = 0;

    private void Awake()
    {
        InstList();
    }


    private void InstList() // 아이템 리스트 생성
    {
        itemList.Add(new Item(0, 0, 3, "골드 더미", "골드 획득량 10% 증가", Effect.getGoldUp, 10));
        itemList.Add(new Item(1, 0, 3, "야생초", "실드 쿨타임 10% 감소", Effect.coolDown_Shield, 10));
        itemList.Add(new Item(2, 0, 3, "에메랄드", "공격력 10% 증가", Effect.atkUp, 10));
        itemList.Add(new Item(3, 1, 3, "돌담", "최대 체력 +1 증가", Effect.hpUp, 1));
        itemList.Add(new Item(4, 1, 3, "찢어진 깃발", "공격 게이지 획득량 10% 증가", Effect.gainUp_Attack, 10));
        itemList.Add(new Item(5, 2, 1, "용의 비늘", "실드 게이지 획득량 50% 증가", Effect.gainUp_Shield, 50));
        itemList.Add(new Item(6, 1, 3, "하얀 열쇠", "실드 게이지 획득량 10% 증가", Effect.gainUp_Shield, 10));
        itemList.Add(new Item(7, 1, 3, "버섯", "점프 게이지 획득량 10% 증가", Effect.gainUp_Jump, 10));
        itemList.Add(new Item(8, 1, 3, "도끼", "공격력 30% 증가", Effect.atkUp, 30));
        itemList.Add(new Item(9, 0, 999, "물약", "즉시 체력 회복", Effect.restoreHp, 5));
        itemList.Add(new Item(10, 2, 1, "까마귀 깃털", "점프 게이지 획득량 50% 증가", Effect.gainUp_Jump, 50));
        itemList.Add(new Item(11, 0, 5, "치즈", "치명타 확률 10% 증가", Effect.criPerUp, 10));
        itemList.Add(new Item(12, 0, 5, "목걸이", "치명타 데미지 10% 증가", Effect.cridamUp, 10));
        itemList.Add(new Item(13, 0, 3, "돌조각", "실드 넉백 효과 10% 증가", Effect.knockBackUp, 10));
        itemList.Add(new Item(14, 1, 3, "호박", "실드 넉백 효과 30% 증가", Effect.knockBackUp, 30));
        itemList.Add(new Item(15, 2, 1, "원기옥", "공격 게이지 획득량 50% 증가", Effect.gainUp_Attack, 50));

        SeperateGrade();
        isLoadItemList = true;
    }

    public void SeperateGrade() // 등급에 따라 선별작업
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            switch(itemList[i].grade)
            {
                case 0:
                    itemList0.Add(itemList[i]);
                    break;
                case 1:
                    itemList1.Add(itemList[i]);
                    break;
                case 2:
                    itemList2.Add(itemList[i]);
                    break;
            }
        }
    }

    public Item DropItem() // 드롭되는 아이템을 반환하는 함수
    {
        if (itemList2.Count == 0) minNum = 1;
        if (itemList1.Count == 0) minNum = 4;
        rand = Random.Range(minNum, 10);
        switch(rand)
        {
            case 0: // 레어
                rand2 = Random.Range(0, itemList2.Count);
                //if (choosList.Contains(itemList2[rand2])) DropItem();
                //else choosList.Add(itemList2[rand2]);
                return itemList2[rand2];
            case 1: // 매직
            case 2:
            case 3:
                rand2 = Random.Range(0, itemList1.Count);
                //if (choosList.Contains(itemList1[rand2])) DropItem();
                //else choosList.Add(itemList1[rand2]);
                return itemList1[rand2];
            default: // 노말
                rand2 = Random.Range(0, itemList0.Count);
                //if (choosList.Contains(itemList0[rand2])) DropItem();
                //else choosList.Add(itemList0[rand2]);
                return itemList0[rand2];
        }
    }

    public void DiscountItem(Item item) // 플레이어가 아이템을 획득했을 떄 해당 아이템을 획득가능 리스트에서 차감.
    {
        switch(item.grade)
        {
            case 0:
                itemList[item.ownNum].count--;
                if (itemList[item.ownNum].count == 0) itemList0.Remove(item);
                break;
            case 1:
                itemList[item.ownNum].count--;
                if (itemList[item.ownNum].count == 0) itemList1.Remove(item);
                break;
            case 2:
                itemList[item.ownNum].count--;
                if (itemList[item.ownNum].count == 0) itemList2.Remove(item);
                break;
        }
    }
    public void ResetChooseList() // 플레이어가 선택을 마치면 임시 선택리스트 초기화
    {
        choosList.Clear();
    }

    private bool CheckDuplicate(Item item) // 선택지에 아이템이 중복으로 뜨는지 검사 ( 내부 )
    {
        if (choosList.Contains(item)) return true;
        else
        {
            choosList.Add(item);
            return false;
        }  
    }

}
