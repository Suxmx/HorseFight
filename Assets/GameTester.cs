using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Services;
using Sirenix.OdinInspector;

public class GameTester : MonoBehaviour
{
    private ShopManager shop;
    private RoadManager roadManager;
    private EventSystem eventSystem;

    private GameAI gameAI;

    private void Start()
    {
        shop = ServiceLocator.Get<ShopManager>();
        roadManager=ServiceLocator.Get<RoadManager>();
        eventSystem=ServiceLocator.Get<EventSystem>();
        
    }

    [Button("≤‚ ‘")]
    public void TestAIShop()
    {
        List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };
        list.Disturb();
        foreach (var a in list)
        {
            Debug.Log(a);
        }
    }
}