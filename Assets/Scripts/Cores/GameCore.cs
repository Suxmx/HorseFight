using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
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
    [Header("AI"), LabelText("是否开启AI")] public bool ifAI;
    [LabelText("AI难度")] public AI.AIMode aiMode;
    public GameState currentState;
    private PlayerInfo playerA, playerB;
    private ShopManager shop;
    private RoadManager roadManager;
    private Dictionary<Team, PlayerInfo> playerDic;
    private Button startButton;
    private SceneController sceneController;
    private EventSystem eventSystem;
    private HorseFactory horseFactory;
    private GameAI ai;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    protected override void Start()
    {
        base.Start();
        sceneController = ServiceLocator.Get<SceneController>();
        shop = ServiceLocator.Get<ShopManager>();
        roadManager = ServiceLocator.Get<RoadManager>();
        eventSystem = ServiceLocator.Get<EventSystem>();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        InitGame();
    }


    private void InitGame()
    {
        playerDic = new Dictionary<Team, PlayerInfo>();
        
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
        ai = new GameAI(playerB, AIMode.Easy, eventSystem, roadManager, shop, horseFactory);
    }

    private void ResetGame()
    {
        InitGame();
    }

    public void FightReady()
    {
        currentState = GameState.Fighting;
        startButton.gameObject.SetActive(true);
    }

    [Button("开始游戏")] //测试按钮
    public void StartFight()
    {
        Debug.LogWarning("进入对战阶段");
        startButton.gameObject.SetActive(false);
        roadManager.OnStart();
    }

    public void AddScore(Team team, int score)
    {
        playerDic[team].Scores += score;
    }

    public void OnGameEnd()
    {
        Invoke(nameof(LoadEndScene), 0.7f);
    }

    private void LoadEndScene()
    {
        GetComponent<AudioSource>().Play();
        sceneController.LoadNextScene();
    }

    public int GetScore(Team team)
    {
        return playerDic[team].Scores;
    }
}