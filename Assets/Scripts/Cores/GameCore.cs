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
    public GameState currentState;
    public float curTime=>gameTimer.Time;
    
    private List<Road> roads;
    private PlayerInfo playerA, playerB;
    private TimerOnly gameTimer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        
        
    }

    protected override void Start()
    {
        base.Start();
        RestartGame();
    }

    public void RestartGame()
    {
        gameTimer = new TimerOnly(true);
        gameTimer.OnTick += _ => { OnFightTick?.Invoke(gameTimer.Time);};

        currentState = GameState.Shopping;
        playerA = new PlayerInfo(10);
        playerB = new PlayerInfo(10);
        roads = new List<Road>();
        for (int i = 1; i <= 5; i++)
        {
            string roadName = "Road " + i;
            roads.Add(GameObject.Find(roadName).GetComponent<Road>());
            OnFightTick.AddListener(roads[i-1].LogicUpdate);
        }
    }
    [Button("开始游戏")]
    public void FightStart()
    {
        gameTimer.Restart();
    }

}
