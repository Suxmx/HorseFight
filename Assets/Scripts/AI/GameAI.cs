using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Shop;
using UnityEngine;
using Random = UnityEngine.Random;

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
            IShop shop, HorseFactory horseFactory)
        {
            this.aiInfo = playerInfo;
            this.aiTeam = playerInfo.team;
            playerTeam = aiTeam.Opponent();
            this.eventSystem = eventSystem;
            this.difficulty = difficulty;
            this.roadManager = roadManager;
            this.shop = shop;
            this.horseFactory = horseFactory;
            enabled = false;

            AIConfig configSO = Resources.Load<AIConfig>("AIConfigs");
            againstDic = configSO.againstDic;
            config = configSO.configDic[difficulty];
            playerInfo.Coins = config.coin;
            Debug.Log(config.coin);
        }

        protected bool enabled;

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                if (enabled) eventSystem.AddListener<Team, int>(EEvent.OnNextRound, AIAction);
                else eventSystem.RemoveListener<Team, int>(EEvent.OnNextRound, AIAction);
            }
        }

        protected PlayerInfo aiInfo;

        protected readonly Team aiTeam;
        protected readonly Team playerTeam;
        protected readonly AIMode difficulty;
        protected readonly EventSystem eventSystem;
        protected readonly RoadManager roadManager;
        protected readonly IShop shop;
        protected readonly HorseFactory horseFactory;
        protected readonly Dictionary<EHorse, EHorse> againstDic;
        protected readonly DifficultyConfig config;
        protected int coins => aiInfo.Coins;

        protected void AIAction(Team team, int round)
        {
            EAIAction action;
            if (team != aiTeam) return;
            if (round == 1) action = EAIAction.Beginning;
            else if (round == 5) action = EAIAction.Final;
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
                    PressFollowAction();
                    break;
                case EAIAction.StalemateFollow:
                    StalemateFollowAction();
                    break;
                case EAIAction.Random:
                    RandomAction();
                    break;
                case EAIAction.Final:
                    FinalAction();
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
            string log = "AI针对行为开始：\n";
            var againstRoads = GetAgainstRoads(ref log);
            if (againstRoads.Count == 0)
            {
                log += "\t金币不足或未发现可针对道路,采用其他行为";
                Debug.Log(log);
                int rand = Utilities.RandomChoose(config.random, config.pressFollow, config.stalemateFollow);
                if (rand == 1) PressFollowAction();
                else if (rand == 2) StalemateFollowAction();
                else RandomAction();
                return;
            }

            //Log
            log += "\t移除无针对卡牌或与买不起的道路后剩余:Road ";
            foreach (var road in againstRoads)
            {
                log += $" {road.num}";
            }

            againstRoads.Disturb();
            log += "\n";
            log += $"\t最终决策：在Road {againstRoads[0].num}购买{againstDic[againstRoads[0].GetHorse(playerTeam).type]}";
            Debug.Log(log);

            shop.AIShopRequest(againstDic[againstRoads[0].GetHorse(playerTeam).type], againstRoads[0].num);
        }

        protected void RandomAction()
        {
            string log = "AI随机行为开始：\n";
            List<int> prices = new List<int>() { 1, 2, 3, 4 };
            List<float> odds = new List<float>();
            prices.RemoveAll(price => price > coins);
            foreach (var price in prices)
                odds.Add(config.randomBuyPossibility[price]);
            int choose = Utilities.RandomChoose(odds); //得到随机的价格
            log += $"\t随机价格：{choose}\n";
            //根据攻击随机
            var horseList = horseFactory.GetHorsesByPrice(choose);
            List<int> damageList = new List<int>();
            foreach (var horse in horseList)
            {
                int damage = horseFactory.GetHorseDamage(horse);
                damage = damage == 0 ? 1 : damage;
                damageList.Add(damage);
            }

            choose = Utilities.RandomChoose(damageList) - 1;
            EHorse chooseHorse = horseList[choose];
            log += $"\t随机马匹：{chooseHorse}\n";
            //随机道路
            var roads = GetPutableRoads(true);
            log += $"\t随机道路编号：{roads[0].num}";
            Debug.Log(log);
            shop.AIShopRequest(chooseHorse, roads[0].num);
        }

        protected virtual void FinalAction()
        {
            string log = "开始最后一回合行为:\n";
            var againstRoads = GetAgainstRoads(ref log);
            againstRoads = againstRoads
                .OrderBy(road => horseFactory.GetHorsePrice(againstDic[road.GetHorse(playerTeam).type])).ToList();

            if (againstRoads.Count == 0 ||
                horseFactory.GetHorsePrice(againstDic[againstRoads[0].GetHorse(playerTeam).type]) < coins)
            {
                //不进行针对行为
                log += $"\t采用随机或是跟牌\n";
                var road = GetPutableRoads()[0];
                var horseList = horseFactory.GetHorsesByPrice(coins > 4 ? 4 : coins);
                Debug.Log(coins > 4 ? 4 : coins);
                List<int> damageList = new List<int>();
                int cnt = 0, maxDamage = -1, maxIndex = 0;
                foreach (var horse in horseList)
                {
                    int damage = horseFactory.GetHorseDamage(horse);
                    damage = damage == 0 ? 1 : damage;
                    damageList.Add(damage);
                    if (damage > maxDamage)
                    {
                        maxDamage = damage;
                        maxIndex = cnt;
                    }

                    cnt++;
                }
                //随机选择是跟牌还是随机
                int rand = Utilities.RandomChoose(config.random, config.pressFollow * 2);
                if (rand == 1)
                {
                    log += "\t采用随机决策\n";
                }
                else
                {
                    log += "\t采用跟牌决策\n";
                }
                int choose = rand == 1 ? Utilities.RandomChoose(damageList) - 1 : maxIndex;
                EHorse chooseHorse = horseList[choose];
                log += $"\t最终决策：在Road {road.num}购买{chooseHorse}";
                Debug.Log(log);
                shop.AIShopRequest(chooseHorse, road.num);
            }
            else
            {
                log +=
                    $"\t采用针对策略\n\t最终决策：在Road {againstRoads[0].num}购买{againstDic[againstRoads[0].GetHorse(playerTeam).type]}";
                Debug.Log(log);
                shop.AIShopRequest(againstDic[againstRoads[0].GetHorse(playerTeam).type], againstRoads[0].num);
            }
        }

        protected void PressFollowAction()
        {
            string log = "开始压制跟牌行为\n";
            var visibleRoads = GetVisibleRoads(ref log);
            visibleRoads.Disturb();
            if (visibleRoads.Count == 0)
            {
                log += "\t无可跟牌道路,进行随机行为";
                Debug.Log(log);
                RandomAction();
                return;
            }

            var road = visibleRoads[0];
            List<EHorse> horseList = new List<EHorse>();
            foreach (EHorse horse in Enum.GetValues(typeof(EHorse)))
            {
                if (horse == EHorse.None) continue;
                if (horseFactory.GetHorseDamage(horse) > road.GetHorse(playerTeam).damage)
                    horseList.Add(horse);
            }

            horseList = horseList.OrderBy(horse => horseFactory.GetHorsePrice(horse)).ToList();
            int cheapest = horseFactory.GetHorsePrice(horseList[0]);
            horseList.RemoveAll(horse => horseFactory.GetHorsePrice(horse) > cheapest);
            horseList.Disturb(); //打乱所有价格一致的之后再选择
            if (horseList.Count == 0 || horseFactory.GetHorsePrice(horseList[0]) > coins)
            {
                log += $"\t无法购买可压制Road {road.num}的马匹，尝试进行僵持跟牌";
                Debug.Log(log);
                StalemateFollowAction();
                return;
            }

            log += $"\t最终决策：在Road {road.num} 购买{horseList[0]}";
            Debug.Log(log);
            shop.AIShopRequest(horseList[0], road.num);
        }

        protected void StalemateFollowAction()
        {
            string log = "开始僵持跟牌行为\n";
            var visibleRoads = GetVisibleRoads(ref log);
            visibleRoads.Disturb();
            if (visibleRoads.Count == 0)
            {
                log += "\t无可跟牌道路,进行随机行为";
                Debug.Log(log);
                RandomAction();
                return;
            }

            var road = visibleRoads[0];
            List<EHorse> horseList = new List<EHorse>();
            foreach (EHorse horse in Enum.GetValues(typeof(EHorse)))
            {
                if (horse == EHorse.None) continue;
                if (horseFactory.GetHorseDamage(horse) == road.GetHorse(playerTeam).damage)
                    horseList.Add(horse);
            }

            horseList = horseList.OrderBy(horse => horseFactory.GetHorsePrice(horse)).ToList();
            int cheapest = horseFactory.GetHorsePrice(horseList[0]);
            horseList.RemoveAll(horse => horseFactory.GetHorsePrice(horse) > cheapest);
            horseList.Disturb(); //打乱所有价格一致的之后再选择
            EHorse chooseHorse = horseList[0];
            if (horseFactory.GetHorsePrice(chooseHorse) > coins)
            {
                log += $"\t无法购买可压制Road {road.num}的马匹，尝试进行随机跟牌";
                Debug.Log(log);
                RandomAction();
                return;
            }

            log += $"\t最终决策：在Road {road.num} 购买{chooseHorse}";
            Debug.Log(log);
            shop.AIShopRequest(chooseHorse, road.num);
        }

        /// <summary>
        /// 获取AI未放置卡牌而玩家已有可见卡牌的道路
        /// </summary>
        /// <param name="log">log</param>
        /// <returns></returns>
        protected List<Road> GetVisibleRoads(ref string log)
        {
            List<Road> visibleRoads = new List<Road>();
            foreach (var road in roadManager.GetRoads())
            {
                if (road.GetHorse(playerTeam) && !road.GetHorse(playerTeam).ifHiding && !road.GetHorse(aiTeam))
                {
                    visibleRoads.Add(road);
                    log += $"\t发现玩家已揭示卡牌道路：Road {road.num}\n";
                }
            }

            return visibleRoads;
        }

        /// <summary>
        /// 获取可针对的道路
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected List<Road> GetAgainstRoads(ref string log)
        {
            var againstRoads = GetVisibleRoads(ref log);
            againstRoads.RemoveAll(road => againstDic[road.GetHorse(playerTeam).type] == EHorse.None); //移除没有可针对的
            againstRoads.RemoveAll(road =>
                horseFactory.GetHorsePrice(againstDic[road.GetHorse(playerTeam).type]) > coins); //移除买不起的
            return againstRoads;
        }

        /// <summary>
        /// 获取还能够防止马匹的道路
        /// </summary>
        /// <param name="disturb">是否打乱</param>
        /// <returns></returns>
        protected List<Road> GetPutableRoads(bool disturb = false)
        {
            var roads = roadManager.GetRoads().ToList();
            roads.RemoveAll(road => road.GetHorse(aiTeam) != null);
            if (disturb) roads.Disturb();
            return roads;
        }
    }
}