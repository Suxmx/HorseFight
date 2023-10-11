using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public enum EHorse //因为用英文还要另外弄一堆中文名，干脆用中文枚举一劳永逸了
{
    None,
    慢马,
    小马,
    风暴,
    爆弹魔,
    沉默者,
    成长,
    肥马,
    教官,
    救星,
    狂马,
    幽灵,
    装甲马,
    冠军,
    后勤,
    大法师,
    冲锋,
    无面,
    超级兵,
    飞驰,
    烧马,
    双面,
    黑暗双面,
    巨人
}

public class Horse : MonoBehaviour
{
    public string horseName => type.ToString();
    
    [Header("属性"),OnValueChanged(nameof(ResetText))] public EHorse type;
    [OnValueChanged(nameof(ResetText))]
    public int speed;
    [OnValueChanged(nameof(ResetText))]
    public int damage;
    [OnValueChanged(nameof(ResetText))]
    public int price;

    [NonSerialized]public Skill skill;
    

    private Transform attributeTransform;
    private TextMeshPro damageText;
    private TextMeshPro speedText;
    private TextMeshPro nameText;

    private void Awake()
    {
        ResetText();
        skill = transform.Find("Skill").GetComponent<Skill>();
    }
    
    private void ResetText()
    {
        attributeTransform = transform.Find("Texts");
        damageText = attributeTransform.Find("DamageText").GetComponent<TextMeshPro>();
        speedText = attributeTransform.Find("SpeedText").GetComponent<TextMeshPro>();
        nameText = attributeTransform.Find("Name").GetComponent<TextMeshPro>();

        damageText.text = damage.ToString();
        speedText.text = speed.ToString();
        nameText.text = horseName;
        
    }
}