using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

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
    public GameState currentState;
    
    private PlayerInfo playerA, playerB;

    protected override void Awake()
    {
        base.Awake();
        playerA = new PlayerInfo(10);
        playerB = new PlayerInfo(10);
    }

    public void RestartGame()
    {
        currentState = GameState.Shopping;
        
    }
}
