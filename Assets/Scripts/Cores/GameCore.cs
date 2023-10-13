using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;
using UnityEngine.Events;
using MyTimer;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private Button startButton;

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
        Transform UICanvasTrans = GameObject.Find("UICanvas").transform;
        Transform scoreUI = UICanvasTrans.Find("ScoreUI");
        TextMeshProUGUI aScoreText = scoreUI.Find("TeamABg/ScoreText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bScoreText = scoreUI.Find("TeamBBg/ScoreText").GetComponent<TextMeshProUGUI>();
        
        playerA = new PlayerInfo(10, Team.A, new GameObject("PlayerA").transform, shop.coinTextA, aScoreText);
        playerB = new PlayerInfo(10, Team.B, new GameObject("PlayerB").transform, shop.coinTextB, bScoreText);
        playerDic.Add(Team.A, playerA);
        playerDic.Add(Team.B, playerB);
        
        startButton = UICanvasTrans.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartFight);
        startButton.gameObject.SetActive(false);

        shop.SetPlayerInfo(playerDic);
    }

    private void ResetGame()
    {
        roads = new List<Road>();
        InitGame();
    }

    public void FightReady()
    {
        currentState = GameState.Fighting;
        roads = roads.OrderBy(road => road.gameObject.name).ToList();
        startButton.gameObject.SetActive(true);
    }

    [Button("开始游戏")]//测试按钮
    public void StartFight()
    {
        startButton.gameObject.SetActive(false);
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
        playerDic[team].Scores+=score;
    }

    public void ShowAllHorses()
    {
        foreach (var road in roads)
        {
            road.ShowHorses();
        }
    }

    public List<Road> GetRoadList()
    {
        return roads;
    }
}