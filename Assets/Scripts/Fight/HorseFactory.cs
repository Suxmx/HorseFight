using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class HorseFactory : Service
{
    private Dictionary<EHorse, GameObject> factoryDic;
    private Dictionary<EHorse, horseUnit> horseAttriDic;
    private HorseAttriSO so;

    protected override void Awake()
    {
        base.Awake();
        so = Resources.Load<HorseAttriSO>("HorseSO");
        factoryDic = new Dictionary<EHorse, GameObject>();
        horseAttriDic = new Dictionary<EHorse, horseUnit>();
        foreach (EHorse type in Enum.GetValues(typeof(EHorse)))
        {
            if (type == EHorse.None) continue;
            factoryDic.Add(type, Resources.Load<GameObject>(type.ToString()));
        }
        foreach (var unit in so.horses)
            horseAttriDic.Add(unit.type, unit);
    }

    public GameObject GetHorseObj(EHorse type)
    {
        return factoryDic[type];
    }

    public int GetHorseDamage(EHorse type)
    {
        return horseAttriDic[type].damage;
    }

    public int GetHorseSpeed(EHorse type)
    {
        return horseAttriDic[type].speed;
    }

    public string GetHorseDesc(EHorse type)
    {
        return horseAttriDic[type].description;
    }

    public int GetHorsePrice(EHorse type)
    {
        return horseAttriDic[type].price;
    }
}