using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public EHorse type;
    public int price;
    
    private ShopManager manager;
    private HorseFactory horseFactory;
    //属性显示
    private Transform texts;
    private TextMeshProUGUI damageText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI nameText;
    //描述
    private Transform descRoot;
    private TextMeshProUGUI desctext;
    private Horse tmpBoughtItem;

    private void Awake()
    {
        texts = transform.Find("BottomTexts");
        damageText = texts.Find("Damage/Text").GetComponent<TextMeshProUGUI>();
        speedText = texts.Find("Speed/Text").GetComponent<TextMeshProUGUI>();
        // nameText = texts.Find("Name").GetComponent<TextMeshProUGUI>();
        
        descRoot = transform.Find("DescriptionRoot");
        
        
    }

    private void Start()
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
        desctext.text = $"<size=135%>{type.ToString()}\n" + "<size=30%>\n" +
                        $"<size=100%>{desc}";
        descRoot.gameObject.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descRoot.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descRoot.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.ShopRequest(this);
    }

    
}
