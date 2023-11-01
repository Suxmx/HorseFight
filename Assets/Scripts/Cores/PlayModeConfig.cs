using System.Collections;
using System.Collections.Generic;
using AI;
using Services;
using UnityEngine;

public class PlayModeConfig : Service
{
    public bool IfAI
    {
        private set;
         get;
    }

    public AI.AIMode AIMode
    {
        private set;
        get;
    }

    public bool IfRandom
    {
        private set;
        get;
    }
    
    public void SetPlayModeConfig(bool ifAI,AIMode aiMode=AIMode.Easy,bool ifRandom=false)
    {
        IfRandom = ifRandom;
        IfAI = ifAI;
        AIMode = aiMode;
    }
}
