using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public Dictionary<EHorse, int> horses;
    public PlayerInfo(int coins)
    {
        this.coins = coins;
        horses = new Dictionary<EHorse, int>();
        foreach (EHorse horse in Enum.GetValues(typeof(EHorse)))
        {
            if(horse==EHorse.None) continue;
            horses.Add(horse, 0);
        }
    }
    public int coins;
    public int scores;
}
