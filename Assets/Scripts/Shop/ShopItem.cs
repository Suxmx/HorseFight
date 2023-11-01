using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EHorse type;
    public int price;

    protected ShopManager manager;

    protected HorseFactory horseFactory;

    //属性显示
    protected Transform texts;
    protected TextMeshProUGUI damageText;
    protected TextMeshProUGUI speedText;

    protected TextMeshProUGUI nameText;

    //描述
    protected Transform descRoot;
    protected TextMeshProUGUI desctext;
    protected Horse tmpBoughtItem;
    protected Image icon;

    protected void Awake()
    {
        texts = transform.Find("BottomTexts");
        damageText = texts.Find("Damage/Text").GetComponent<TextMeshProUGUI>();
        speedText = texts.Find("Speed/Text").GetComponent<TextMeshProUGUI>();
        icon = transform.Find("Icon").GetComponent<Image>();
        descRoot = transform.Find("DescriptionRoot");
    }

    protected virtual void Start()
    {
        manager = ServiceLocator.Get<ShopManager>();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        damageText.text = horseFactory.GetHorseDamage(type).ToString();
        speedText.text = horseFactory.GetHorseSpeed(type).ToString();
        // nameText.text = type.ToString();
        price = horseFactory.GetHorsePrice(type);
        manager.RegisterItem(this);

        string desc = horseFactory.GetHorseDesc(type);
        desc = string.IsNullOrEmpty(desc) ? "无特殊技能" : desc;
        descRoot.gameObject.SetActive(true);
        desctext = descRoot.Find("DescBg").Find("DescText").GetComponent<TextMeshProUGUI>();
        desctext.text = $"<align=center><size=135%><color=orange>{type.ToString()}</color></align>\n" + "<size=30%>\n" +
                        $"<size=100%>{desc}";
        descRoot.gameObject.SetActive(false);
        icon.sprite = Resources.Load<Sprite>(type.ToString());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descRoot.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descRoot.gameObject.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        manager.ShopRequest(this);
    }
}