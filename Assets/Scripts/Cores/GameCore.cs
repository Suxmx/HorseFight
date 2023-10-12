using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Events;
using MyTimer;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;

public enum GameState
{
    /// <summary>
    /// 购买马匹
    /// </summary>
    Shopping,

    /// <summary>
    /// 进行对战
    /// </summary>
    Fighting
}

public class GameCore : Service
{
    public UnityEvent<float> OnFightTick;
    public UnityEvent OnFightStart;
    public GameState currentState;
    public float curTime => gameTimer.Time;

    private List<Road> roads= new List<Road>();
    private PlayerInfo playerA, playerB;
    private TimerOnly gameTimer;
    private ShopManager shop;
    private Dictionary<Team, PlayerInfo> playerDic;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    protected override void Start()
    {
        base.Start();
        InitGame();
    }


    private void InitGame()
    {
        playerDic = new Dictionary<Team, PlayerInfo>();
        shop = ServiceLocator.Get<ShopManager>();
        currentState = GameState.Shopping;
        Transform scoreUI = GameObject.Find("UICanvas").transform.Find("ScoreUI");
        TextMeshProUGUI aScoreText = scoreUI.Find("TeamABg/ScoreText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bScoreText = scoreUI.Find("TeamBBg/ScoreText").GetComponent<TextMeshProUGUI>();
        playerA = new PlayerInfo(10, Team.A, new GameObject("PlayerA").transform, shop.coinTextA, aScoreText);
        playerB = new PlayerInfo(10, Team.B, new GameObject("PlayerB").transform, shop.coinTextB, bScoreText);
        playerDic.Add(Team.A, playerA);
        playerDic.Add(Team.B, playerB);
        roads = new List<Road>();

        shop.SetPlayerInfo(playerDic);
    }

    public void FightReady()
    {
        currentState = GameState.Fighting;
    }

    [Button("开始游戏")]
    public void StartFight()
    {
        gameTimer = new TimerOnly(true);
        OnFightStart?.Invoke();
        gameTimer.Restart();
    }

    public void RegisterRoad(Road road)
    {
        roads.Add(road);
        OnFightStart.AddListener(road.OnStart);
    }

    public void AddScore(Team team, int score)
    {
        playerDic[team].Scores++;
    }
}