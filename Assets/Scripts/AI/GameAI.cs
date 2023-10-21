using System.Collections.Generic;
using Services;
using UnityEngine;

namespace AI
{
    public class GameAI
    {
        public enum EAIAction
        {
            Against = 1,
            PressFollow = 2,
            StalemateFollow = 3,
            Random = 4,
            Beginning = 5,
            Final = 6,
        }

        public GameAI(PlayerInfo playerInfo, AIMode difficulty, EventSystem eventSystem, RoadManager roadManager,
            ShopManager shopManager, HorseFactory horseFactory)
        {
            this.playerInfo = playerInfo;
            this.aiTeam = playerInfo.team;
            playerTeam = aiTeam.Opponent();
            this.eventSystem = eventSystem;
            this.difficulty = difficulty;
            this.roadManager = roadManager;
            this.shopManager = shopManager;
            this.horseFactory = horseFactory;
            eventSystem.AddListener<Team, int>(EEvent.OnNextRound, AIAction);

            configSO = Resources.Load<AIConfig>("AIConfigs");
            againstDic = configSO.againstDic;
            config = configSO.configDic[difficulty];
            playerInfo.Coins = config.coin;
        }

        protected PlayerInfo playerInfo;
        protected Team aiTeam;
        protected Team playerTeam;
        protected AIMode difficulty;
        protected AIConfig configSO;
        protected EventSystem eventSystem;
        protected RoadManager roadManager;
        protected ShopManager shopManager;
        protected HorseFactory horseFactory;
        protected Dictionary<EHorse, EHorse> againstDic;
        protected DifficultyConfig config;

        protected void AIAction(Team team, int round)
        {
            EAIAction action;
            if (team != aiTeam) return;
            if (round == 1) action = EAIAction.Beginning;
            else
                action = (EAIAction)Utilities.RandomChoose(config.against, config.pressFollow,
                    config.stalemateFollow, config.random);
            switch (action)
            {
                case EAIAction.Beginning:
                    BeginAction();
                    break;
                case EAIAction.Against:
                    break;
                case EAIAction.PressFollow:
                    break;
                case EAIAction.StalemateFollow:
                    break;
                case EAIAction.Random:
                    break;
                case EAIAction.Final:
                    break;
                default:
                    Debug.LogError($"未实现{action}对应的处理函数");
                    break;
            }
        }

        protected void BeginAction()
        {
            EHorse horse;
            int randRoad = Random.Range(1, 6);
            if (randRoad == 3)
            {
                horse = EHorse.荒原豚;
            }
            else
            {
                int randHorse = Random.Range(0, 4);
                List<EHorse> randHorses = new List<EHorse>() { EHorse.初级骑士, EHorse.淑女, EHorse.狂豚, EHorse.中级骑士 };
                horse = randHorses[randHorse];
            }

            shopManager.AIShopRequest(horse, randRoad);
        }

        protected void AgainstAction()
        {
            List<Road> againstRoads = new List<Road>();
            bool canAgainst = false;
            foreach (var road in roadManager.GetRoads())
            {
                if (road.GetHorse(playerTeam) && !road.GetHorse(playerTeam).ifHiding && !road.GetHorse(aiTeam))
                    againstRoads.Add(road);
            }
        }
    }
}