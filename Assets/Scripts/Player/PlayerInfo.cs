using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo
{
    public List<EHorse> ownHorses;
    public PlayerInfo(int coins,Team team,Transform trans,TextMeshProUGUI text)
    {
        this.team = team;
        this.coins = coins;
        this.trans = trans;
        this.text = text;
        ownHorses = new List<EHorse>();
    }

    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            string tmp = text.text;
            text.text = tmp.Split(':')[0] + ":" + coins;
        }
    }
    public int scores;
    public Team team;
    public Transform trans;
    
    private int coins;
    private TextMeshProUGUI text;
}
