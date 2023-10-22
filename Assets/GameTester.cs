using System;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Services;
using Sirenix.OdinInspector;

public class GameTester : MonoBehaviour
{
    protected RoadManager roadManager;
    protected ShopManager shop;
    protected HorseFactory horseFactory;
    private DifficultyConfig config;

    private void Awake()
    {
        AIConfig configSO = Resources.Load<AIConfig>("AIConfigs");
        config = configSO.configDic[AIMode.Easy];
    }

    private void Start()
    {
        roadManager = ServiceLocator.Get<RoadManager>();
        horseFactory = ServiceLocator.Get<HorseFactory>();
    }

    [Button("测试")]
    public void TestRandom()
    {
        Dictionary<EHorse, int> tester = new Dictionary<EHorse, int>();
        foreach (EHorse type in Enum.GetValues(typeof(EHorse)))
        {
            if (type != EHorse.None) tester.Add(type, 0);
        }

        List<int> prices = new List<int>() { 1, 2, 3, 4 };
        List<float> odds = new List<float>();
        foreach (var price in prices)
            odds.Add(config.randomBuyPossibility[price]);
        for (int i = 1; i <= 10000; i++)
        {
            int choose = Utilities.RandomChoose(odds);
            //根据攻击随机
            var horseList = horseFactory.GetHorsesByPrice(3);
            List<int> damageList = new List<int>();
            foreach (var horse in horseList)
            {
                int damage = horseFactory.GetHorseDamage(horse);
                damageList.Add(damage == 0 ? 1 : damage);
            }

            choose = Utilities.RandomChoose(damageList) - 1;
            EHorse chooseHorse = horseList[choose];
            tester[chooseHorse]++;
        }

        int add = 0;
        foreach (EHorse type in horseFactory.GetHorsesByPrice(3))
        {
            int damage = horseFactory.GetHorseDamage(type);
            damage = damage == 0 ? 1 : damage;
            add += damage;
        }
        foreach (EHorse type in horseFactory.GetHorsesByPrice(3))
        {
            int damage = horseFactory.GetHorseDamage(type);
            damage = damage == 0 ? 1 : damage;
            if (type != EHorse.None)
                Debug.Log($"{type}:{tester[type] / 100}% 价格占比:{1.0f*damage/add*100f}%");
        }
    }
}