using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Events;
using MyTimer;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public enum GameState
{
    /// <summary>
    /// 购买马匹
    /// </summary>
    Shopping,

    /// <summary>
    /// 放置马匹
    /// </summary>
    Putting,

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

    private List<Road> roads;
    private PlayerInfo playerA, playerB;
    private TimerOnly gameTimer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        InitGame();
    }

    protected override void Start()
    {
        base.Start();
    }


    private void InitGame()
    {
        currentState = GameState.Shopping;
        playerA = new PlayerInfo(10);
        playerB = new PlayerInfo(10);
        roads = new List<Road>();
    }

    [Button("开始游戏")]
    public void StartFight()
    {
        gameTimer = new TimerOnly(true);
        gameTimer.OnTick += _ => { OnFightTick?.Invoke(gameTimer.Time); };
        OnFightStart?.Invoke();
        gameTimer.Restart();
    }

    public void RegisterRoad(Road road)
    {
        roads.Add(road);
        OnFightTick.AddListener(road.LogicUpdate);
        OnFightStart.AddListener(road.OnStart);
    }
    
}