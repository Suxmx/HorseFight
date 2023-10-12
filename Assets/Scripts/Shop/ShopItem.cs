using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public EHorse type;
    
    private ShopManager manager;
    private HorseFactory horseFactory;
    // Ù–‘œ‘ æ
    private Transform texts;
    private TextMeshProUGUI damageText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI nameText;
    //√Ë ˆ
    private Transform descRoot;
    private TextMeshProUGUI desctext;

    private void Awake()
    {
        texts = transform.Find("BottomTexts");
        damageText = texts.Find("Damage").GetComponent<TextMeshProUGUI>();
        speedText = texts.Find("Speed").GetComponent<TextMeshProUGUI>();
        nameText = texts.Find("Name").GetComponent<TextMeshProUGUI>();
        
        descRoot = transform.Find("DescriptionRoot");
        
        
    }

    private void Start()
    {
        manager = ServiceLocator.Get<ShopManager>();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        damageText.text = horseFactory.GetHorseDamage(type).ToString();
        speedText.text = horseFactory.GetHorseSpeed(type).ToString();
        nameText.text = type.ToString();
        manager.RegisterItem(this);
        
        descRoot.gameObject.SetActive(true);
        desctext = descRoot.Find("DescBg").Find("DescText").GetComponent<TextMeshProUGUI>();
        desctext.text = $"<size=135%>{type.ToString()}\n" + "<size=135%>\n" +
                        $"<size=100%>{horseFactory.GetHorseDesc(type)}";
        descRoot.gameObject.SetActive(false);

    }
}
