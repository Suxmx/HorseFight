using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public List<EHorse> ownHorses;
    public PlayerInfo(int coins,Team team,Transform trans)
    {
        this.team = team;
        this.coins = coins;
        this.trans = trans;
        ownHorses = new List<EHorse>();
    }
    public int coins;
    public int scores;
    public Team team;
    public Transform trans;
}
