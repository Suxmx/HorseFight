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
    private Transform texts;
    private TextMeshProUGUI damageText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI nameText;

    private void Awake()
    {
        texts = transform.Find("BottomTexts");
        damageText = texts.Find("Damage").GetComponent<TextMeshProUGUI>();
        speedText = texts.Find("Speed").GetComponent<TextMeshProUGUI>();
        nameText = texts.Find("Name").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        manager = ServiceLocator.Get<ShopManager>();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        damageText.text = horseFactory.GetHorseDamage(type).ToString();
        speedText.text = horseFactory.GetHorseSpeed(type).ToString();
        nameText.text = type.ToString();
        manager.RegisterItem(this);
        
    }
}
