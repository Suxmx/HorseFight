using System;
using System.Collections;
using System.Collections.Generic;
using MyTimer;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoadInfo
{
    public RoadInfo(Team team, Vector2 startPoint)
    {
        this.team = team;
        this.startPoint = startPoint;
    }

    public Team team;
    public Horse horse;
    public Vector2 startPoint;
    public float spriteSize;
}

public class Road : MonoBehaviour
{
    public float roadLength => rightPos.x - leftPos.x;
    public int num => Convert.ToInt32((name.Split("Road ")[1]));
    [LabelText("僵持")] public bool stalemated = false;
    [LabelText("是否开始")] public bool ifStart = false;

    private Vector2 leftPos => leftTrans.position;
    private Vector2 rightPos => rightTrans.position;
    private Transform leftTrans, rightTrans;
    private Dictionary<Team, RoadInfo> infoDic;
    private GameCore core;
    private ShopManager shop;
    private TimerOnly roadTimer;
    private float spriteSize;
    private bool hasHorseWin = false;

    private float unitLength => roadLength / 20f;
    // private float curTime => core.curTime;

    private void Awake()
    {
        roadTimer = new TimerOnly(true);
        leftTrans = transform.Find("LeftStart");
        rightTrans = transform.Find("RightStart");
        RoadInfo infoa = new(Team.A, leftPos), infob = new(Team.B, rightPos);

        infoDic = new Dictionary<Team, RoadInfo>();
        infoDic.Add(Team.A, infoa);
        infoDic.Add(Team.B, infob);
    }

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
        shop = ServiceLocator.Get<ShopManager>();
        core.RegisterRoad(this);
    }

    // private void Update()
    // {
    //     if (!ifStart) return;
    //     LogicUpdate();
    // }
    //
    // private void FixedUpdate()
    // {
    //     if (!ifStart) return;
    //     MoveHorses(Time.fixedDeltaTime);
    // }

    private Vector2 CalcVec(Team team, float dt, int speed)
    {
        Vector2 dirVec = team == Team.A ? Vector2.right : Vector2.left;
        return dirVec * (dt * unitLength * speed);
    }

    public void OnStart()
    {
        ifStart = true;
        roadTimer.Restart();
        //释放开场技能
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.skill.OnStart();
        }
    }

    public void LogicUpdate()
    {
        if (CheckHit() || stalemated)
        {
            HorseFight();
        }

        //检测终点
        CheckWin();
    }

    public void ClearTempStatus()
    {
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.ClearStatus();
        }
    }

    public void CalcAttribute()
    {
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.CalcDamageAndSpeed();
        }
    }

    public void TickSkill()
    {
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.skill.TickCheck();
        }
    }


    public void MoveHorses(float dt)
    {
        if (!stalemated)
        {
            foreach (var info in infoDic.Values)
            {
                if (!info.horse) continue;
                if (info.horse.HasStatus(EStatus.Die) || info.horse.HasStatus(EStatus.End) ||
                    info.horse.HasStatus(EStatus.Freeze)) continue;
                info.horse.transform.Translate(CalcVec(info.team, dt, info.horse.speed));
            }
        }
    }

    private bool CheckHit()
    {
        Horse aHorse = infoDic[Team.A].horse, bHorse = infoDic[Team.B].horse;
        if (!aHorse || !bHorse) return false;
        if ((aHorse.type == EHorse.幽灵 && !aHorse.skill.silented) ||
            (bHorse.type == EHorse.幽灵 && !bHorse.skill.silented))
            return false;
        return aHorse.transform.position.x >= bHorse.transform.position.x;
    }

    private void CheckWin()
    {
        bool flag = false;
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            if (Mathf.Abs(info.horse.transform.position.x - info.startPoint.x) < roadLength) continue;
            flag = true;
            info.horse.AddStatus(EStatus.End);
            info.horse.skill.OnEnd();
            if (!hasHorseWin)
            {
                core.AddScore(info.team, info.horse.score);
                Debug.Log($"{info.team} Win At {roadTimer.Time}");
            }
        }

        hasHorseWin = flag;
    }

    private Team HorseFight()
    {
        if (infoDic[Team.A].horse.damage > infoDic[Team.B].horse.damage)
        {
            infoDic[Team.A].horse.skill.OnKill(infoDic[Team.A].horse.damage - infoDic[Team.B].horse.damage);
            infoDic[Team.B].horse.LoseCG();
            infoDic[Team.B].horse.skill.OnDeath();
            stalemated = false;
            return Team.A;
        }
        else if (infoDic[Team.A].horse.damage < infoDic[Team.B].horse.damage)
        {
            infoDic[Team.B].horse.skill.OnKill(infoDic[Team.B].horse.damage - infoDic[Team.A].horse.damage);
            infoDic[Team.A].horse.LoseCG();
            infoDic[Team.A].horse.skill.OnDeath();
            stalemated = false;
            return Team.B;
        }
        else
        {
            stalemated = true;
            return Team.None;
        }
    }

    public void SetHorse(Horse horse)
    {
        var teamInfo = infoDic[horse.horseTeam];
        if (teamInfo.horse) return;
        teamInfo.horse = horse;
        horse.locateRoad = this;
        horse.transform.position = teamInfo.startPoint;
        horse.SetPutMode(horse.horseTeam, false);
        horse.SetDir(horse.horseTeam);
        if (horse.horseTeam == Team.A)
        {
            horse.HideSelf();
            shop.SetCoinTextUnknown(horse.price);
        }

        // teamInfo.spriteSize = horse.GetComponent<SpriteRenderer>().bounds.size.x;
        shop.NextRound();
    }

    public void ShowHorses()
    {
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.ShowSelf();
        }
    }

    public Horse GetHorse(Team team)
    {
        return infoDic[team].horse;
    }

    public RoadInfo GetInfo(Team team)
    {
        return infoDic[team];
    }
}