using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public EHorse type;
    
    private ShopManager manager;
    private void Start()
    {
        manager = ServiceLocator.Get<ShopManager>();
        manager.RegisterItem(this);
        
    }
}
