using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo
{
    public List<EHorse> ownHorses;
    public PlayerInfo(int coins,Team team,Transform trans,TextMeshProUGUI coinText,TextMeshProUGUI scoreText)
    {
        this.team = team;
        this.coins = coins;
        this.trans = trans;
        this.coinText = coinText;
        this.scoreText = scoreText;
        scores = 0;
        ownHorses = new List<EHorse>();
    }

    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            string tmp = coinText.text;
            coinText.text = tmp.Split(':')[0] + ":" + coins;
        }
    }

    public int Scores
    {
        get => scores;
        set
        {
            scores = value;
            scoreText.text = scores.ToString();
        }
    }
    private int scores;
    public Team team;
    public Transform trans;
    
    private int coins;
    private TextMeshProUGUI coinText;
    private TextMeshProUGUI scoreText;

}
