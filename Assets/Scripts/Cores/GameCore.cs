using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Cores;
using Services;
using UnityEngine;
using UnityEngine.Events;
using MyTimer;
using Shop;
using Shop.Repo;
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
    private bool _ifRandom = false;
    public GameState currentState;
    private PlayerInfo playerA, playerB;
    private ShopManager shop;
    private RepoManager _repoManager;
    [Other] private RoadManager roadManager;
    [Other] private SceneController sceneController;
    [Other] private EventSystem eventSystem;
    [Other] private HorseFactory horseFactory;

    private Dictionary<Team, PlayerInfo> playerDic;
    private Button startButton;
    private GameAI ai;
    private IShop ishop;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    protected override void Start()
    {
        base.Start();
        PlayModeConfig modeConfig = ServiceLocator.Get<PlayModeConfig>();
        if (modeConfig) //如果读取到了配置文件
        {
            ifAI = modeConfig.IfAI;
            aiMode = modeConfig.AIMode;
            _ifRandom = modeConfig.IfRandom;
        }

        if (!_ifRandom)
        {
            shop = ServiceLocator.Get<ShopManager>();
            ishop = shop;
        }

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

        playerA = new PlayerInfo(10, Team.A, new GameObject("PlayerA").transform, shop != null ? shop.coinTextA : null,
            aScoreText);
        playerB = new PlayerInfo(10, Team.B, new GameObject("PlayerB").transform, shop != null ? shop.coinTextB : null,
            bScoreText);
        playerDic.Add(Team.A, playerA);
        playerDic.Add(Team.B, playerB);

        startButton = UICanvasTrans.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartFight);
        startButton.gameObject.SetActive(false);
        //随机模式
        if (_ifRandom)
        {
            _repoManager = ServiceLocator.Get<RepoManager>();
            playerB.coinText = _repoManager.aiCoinText;
            _repoManager.SetInfos(playerB, playerA);
            ishop = _repoManager;
        }
        else 
        {
            shop.SetPlayerInfo(playerDic);
        }

        if (ifAI)
        {
            ai = new GameAI(playerB, aiMode, eventSystem, roadManager, ishop, horseFactory);
            ai.Enabled = ifAI;
        }
    }

    public IShop GetShop()
    {
        return ishop;
    }

    private void ResetGame()
    {
        InitGame();
    }

    public void FightReady()
    {
        Debug.Log("FightReady");
        startButton.gameObject.SetActive(true);
    }

    [Button("开始游戏")] //测试按钮
    public void StartFight()
    {
        currentState = GameState.Fighting;
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
        sceneController.LoadScene(3);
    }

    public int GetScore(Team team)
    {
        return playerDic[team].Scores;
    }
}