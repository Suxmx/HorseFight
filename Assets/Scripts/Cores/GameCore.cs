using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Events;
using MyTimer;

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
    public UnityEvent<int> OnRoundChange;
    public GameState currentState;
    public int curRound;
    public List<Road> roads;
    
    private PlayerInfo playerA, playerB;
    private TimerOnly gameTimer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        
        RestartGame();
    }

    public void RestartGame()
    {
        gameTimer = new TimerOnly();
        gameTimer.Restart();
        currentState = GameState.Shopping;
        playerA = new PlayerInfo(10);
        playerB = new PlayerInfo(10);
        roads = new List<Road>();
        for (int i = 1; i <= 5; i++)
        {
            string roadName = "Road " + i;
            roads.Add(GameObject.Find(roadName).GetComponent<Road>());
        }
    }

    public void FightStart()
    {
        curRound = 1;
    }
    
}
