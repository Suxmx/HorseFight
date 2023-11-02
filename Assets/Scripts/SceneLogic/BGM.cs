using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class BGM : Service
{
    public AudioSource win;
    private bool hasInit=false;

    protected override void Awake()
    {
        singleInAllScene = true;
        if(!hasInit)
        {
            hasInit = true;
            base.Awake();
            DontDestroyOnLoad(this);
        }
    }

    public void PlayWinSound()
    {
        win.Play();
    }
}