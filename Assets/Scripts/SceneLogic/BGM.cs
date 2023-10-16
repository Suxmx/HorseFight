using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class BGM : Service
{
    protected override void Awake()
    {
        BGM instance = ServiceLocator.Get<BGM>();
        if (instance != null&& instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
