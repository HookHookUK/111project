using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect
{
    atkUp,
    hpUp,
    restoreHp,
    coolDown_Shield,
    gainUp_Jump,
    gainUp_Shield,
    gainUp_Attack,
    getGoldUp,
    gainUp_All,
    criPerUp,
    cridamUp,
    knockBackUp,
}

public class Item : MonoBehaviour
{
    [SerializeField] public int ownNum; // 고유번호 및 아이템 이미지 식별
    [SerializeField] public int grade;
    [SerializeField] public int count; // 중첩가능 횟수
    [SerializeField] public string itemName;
    [SerializeField] public string description;
    [SerializeField] public Effect effect;
    [SerializeField] public int amount;

    public Item(int num, int grade, int count,  string name, string desc, Effect effect, int amount)
    {
        ownNum = num;
        this.grade = grade;
        this.count = count;
        itemName = name;
        description = desc;
        this.effect = effect;
        this.amount = amount;
    }

}
