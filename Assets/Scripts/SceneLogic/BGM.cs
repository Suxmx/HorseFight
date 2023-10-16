using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class BGM : Service
{
    protected override void Awake()
    {
        if (ServiceLocator.Get<BGM>() != null) Destroy(gameObject);
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
