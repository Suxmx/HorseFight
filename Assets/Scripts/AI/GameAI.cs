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
            ShopManager shop, HorseFactory horseFactory)
        {
            this.aiInfo = playerInfo;
            this.aiTeam = playerInfo.team;
            playerTeam = aiTeam.Opponent();
            this.eventSystem = eventSystem;
            this.difficulty = difficulty;
            this.roadManager = roadManager;
            this.shop = shop;
            this.horseFactory = horseFactory;
            eventSystem.AddListener<Team, int>(EEvent.OnNextRound, AIAction);

            configSO = Resources.Load<AIConfig>("AIConfigs");
            againstDic = configSO.againstDic;
            config = configSO.configDic[difficulty];
            playerInfo.Coins = config.coin;
        }

        protected PlayerInfo aiInfo;
        protected Team aiTeam;
        protected Team playerTeam;
        protected AIMode difficulty;
        protected AIConfig configSO;
        protected EventSystem eventSystem;
        protected RoadManager roadManager;
        protected ShopManager shop;
        protected HorseFactory horseFactory;
        protected Dictionary<EHorse, EHorse> againstDic;
        protected DifficultyConfig config;
        protected int coins => aiInfo.Coins;

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
                    AgainstAction();
                    break;
                case EAIAction.PressFollow:
                    AgainstAction();
                    break;
                case EAIAction.StalemateFollow:
                    AgainstAction();
                    break;
                case EAIAction.Random:
                    AgainstAction();
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

            shop.AIShopRequest(horse, randRoad);
        }

        protected void AgainstAction()
        {
            string log = "针对行为开始：\n";
            List<Road> againstRoads = new List<Road>();
            foreach (var road in roadManager.GetRoads())
            {
                if (road.GetHorse(playerTeam) && !road.GetHorse(playerTeam).ifHiding && !road.GetHorse(aiTeam))
                {
                    againstRoads.Add(road);
                    log += $"\t发现玩家已揭示卡牌道路：Road {road.num}\n";
                }
            }

            againstRoads.RemoveAll(road => againstDic[road.GetHorse(playerTeam).type] == EHorse.None); //移除没有可针对的
            againstRoads.RemoveAll(road =>
                horseFactory.GetHorsePrice(againstDic[road.GetHorse(playerTeam).type]) > coins); //移除买不起的

            if (againstRoads.Count == 0)
            {
                log += "\t金币不足或未发现可针对道路,采用随机行为";
                Debug.Log(log);
                RandomAction();
                return;
            }
            //Log
            log += "\t移除无针对卡牌或与买不起的道路后剩余:Road ";
            foreach (var road in againstRoads)
            {
                log += $" {road.num}";
            }
            log += "\n";
            log += $"\t最终决策：在Road {againstRoads[0].num}购买{againstDic[againstRoads[0].GetHorse(playerTeam).type]}";
            Debug.Log(log);

            shop.AIShopRequest(againstDic[againstRoads[0].GetHorse(playerTeam).type], againstRoads[0].num);
        }

        protected void RandomAction()
        {
        }
    }
}