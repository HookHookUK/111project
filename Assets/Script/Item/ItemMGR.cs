using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMGR : MonoBehaviour
{
    private List<Item> itemList = new List<Item>(); // ��ü ������ ����Ʈ


    private List<Item> itemList0 = new List<Item>(); // 0Ƽ�� ������ ����Ʈ
    private List<Item> itemList1 = new List<Item>(); // 1Ƽ�� ������ ����Ʈ
    private List<Item> itemList2 = new List<Item>(); // 2Ƽ�� ������ ����Ʈ

    public bool isLoadItemList = false;

    private List<Item> choosList = new List<Item>();

    private int rand = 0;
    private int rand2 = 0;
    private int minNum = 0;

    private void Awake()
    {
        InstList();
    }


    private void InstList() // ������ ����Ʈ ����
    {
        itemList.Add(new Item(0, 0, 3, "��� ����", "��� ȹ�淮 10% ����", Effect.getGoldUp, 10));
        itemList.Add(new Item(1, 0, 3, "�߻���", "�ǵ� ��Ÿ�� 10% ����", Effect.coolDown_Shield, 10));
        itemList.Add(new Item(2, 0, 3, "���޶���", "���ݷ� 10% ����", Effect.atkUp, 10));
        itemList.Add(new Item(3, 1, 3, "����", "�ִ� ü�� +1 ����", Effect.hpUp, 1));
        itemList.Add(new Item(4, 1, 3, "������ ���", "���� ������ ȹ�淮 10% ����", Effect.gainUp_Attack, 10));
        itemList.Add(new Item(5, 2, 1, "���� ���", "�ǵ� ������ ȹ�淮 50% ����", Effect.gainUp_Shield, 50));
        itemList.Add(new Item(6, 1, 3, "�Ͼ� ����", "�ǵ� ������ ȹ�淮 10% ����", Effect.gainUp_Shield, 10));
        itemList.Add(new Item(7, 1, 3, "����", "���� ������ ȹ�淮 10% ����", Effect.gainUp_Jump, 10));
        itemList.Add(new Item(8, 1, 3, "����", "���ݷ� 30% ����", Effect.atkUp, 30));
        itemList.Add(new Item(9, 0, 999, "����", "��� ü�� ȸ��", Effect.restoreHp, 5));
        itemList.Add(new Item(10, 2, 1, "��� ����", "���� ������ ȹ�淮 50% ����", Effect.gainUp_Jump, 50));
        itemList.Add(new Item(11, 0, 5, "ġ��", "ġ��Ÿ Ȯ�� 10% ����", Effect.criPerUp, 10));
        itemList.Add(new Item(12, 0, 5, "�����", "ġ��Ÿ ������ 10% ����", Effect.cridamUp, 10));
        itemList.Add(new Item(13, 0, 3, "������", "�ǵ� �˹� ȿ�� 10% ����", Effect.knockBackUp, 10));
        itemList.Add(new Item(14, 1, 3, "ȣ��", "�ǵ� �˹� ȿ�� 30% ����", Effect.knockBackUp, 30));
        itemList.Add(new Item(15, 2, 1, "�����", "���� ������ ȹ�淮 50% ����", Effect.gainUp_Attack, 50));

        SeperateGrade();
        isLoadItemList = true;
    }

    public void SeperateGrade() // ��޿� ���� �����۾�
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

    public Item DropItem() // ��ӵǴ� �������� ��ȯ�ϴ� �Լ�
    {
        if (itemList2.Count == 0) minNum = 1;
        if (itemList1.Count == 0) minNum = 4;
        rand = Random.Range(minNum, 10);
        switch(rand)
        {
            case 0: // ����
                rand2 = Random.Range(0, itemList2.Count);
                //if (choosList.Contains(itemList2[rand2])) DropItem();
                //else choosList.Add(itemList2[rand2]);
                return itemList2[rand2];
            case 1: // ����
            case 2:
            case 3:
                rand2 = Random.Range(0, itemList1.Count);
                //if (choosList.Contains(itemList1[rand2])) DropItem();
                //else choosList.Add(itemList1[rand2]);
                return itemList1[rand2];
            default: // �븻
                rand2 = Random.Range(0, itemList0.Count);
                //if (choosList.Contains(itemList0[rand2])) DropItem();
                //else choosList.Add(itemList0[rand2]);
                return itemList0[rand2];
        }
    }

    public void DiscountItem(Item item) // �÷��̾ �������� ȹ������ �� �ش� �������� ȹ�氡�� ����Ʈ���� ����.
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
    public void ResetChooseList() // �÷��̾ ������ ��ġ�� �ӽ� ���ø���Ʈ �ʱ�ȭ
    {
        choosList.Clear();
    }

    private bool CheckDuplicate(Item item) // �������� �������� �ߺ����� �ߴ��� �˻� ( ���� )
    {
        if (choosList.Contains(item)) return true;
        else
        {
            choosList.Add(item);
            return false;
        }  
    }

}
