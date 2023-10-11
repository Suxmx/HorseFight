using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum EHorse//因为用英文还要另外弄一堆中文名，干脆用中文枚举一劳永逸了
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
    [Header("属性")] 
    public string horseName;
    public int speed;
    public int damage;
    public int price;

    private Transform attributeTransform;
    private TextMeshPro damageText;
    private TextMeshPro speedText;
    private TextMeshPro nameText;

    private void Awake()
    {
        attributeTransform = transform.Find("Attributes");
        damageText = attributeTransform.Find("DamageText").GetComponent<TextMeshPro>();
        speedText = attributeTransform.Find("SpeedText").GetComponent<TextMeshPro>();
        nameText = attributeTransform.Find("Name").GetComponent<TextMeshPro>();

        damageText.text = damage.ToString();
        speedText.text = damage.ToString();
        nameText.text = horseName;
    }

    private void Start()
    {
        
    }
}
