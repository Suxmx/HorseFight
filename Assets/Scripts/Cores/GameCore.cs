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
    public GameState currentState;
    public float curTime => gameTimer.Time;

    private List<Road> roads= new List<Road>();
    private PlayerInfo playerA, playerB;
    private TimerOnly gameTimer;
    private ShopManager shop;
    private RoadManager roadManager;
    private Dictionary<Team, PlayerInfo> playerDic;
    private Button startButton;
    private SceneController sceneController;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    protected override void Start()
    {
        base.Start();
        sceneController = ServiceLocator.Get<SceneController>();
        InitGame();
    }


    private void InitGame()
    {
        playerDic = new Dictionary<Team, PlayerInfo>();
        shop = ServiceLocator.Get<ShopManager>();
        roadManager = ServiceLocator.Get<RoadManager>();
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
        Debug.LogWarning("进入对战阶段");
        startButton.gameObject.SetActive(false);
        gameTimer = new TimerOnly(true);
        gameTimer.Restart();
        roadManager.Init(roads);
        roadManager.OnStart();
    }

    public void RegisterRoad(Road road)
    {
        roads.Add(road);
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))OnGameEnd();
    }

    public void OnGameEnd()
    {
        GetComponent<AudioSource>().Play();
        sceneController.LoadNextScene();
    }

    public int GetScore(Team team)
    {
        return playerDic[team].Scores;
    }
}