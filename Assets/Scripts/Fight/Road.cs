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
    [LabelText("结束")] public bool finish = false;
    [LabelText("僵持")] public bool stalemated = false;
    [LabelText("是否开始")] public bool ifStart = false;

    private Vector2 leftPos => leftTrans.position;
    private Vector2 rightPos => rightTrans.position;
    private Transform leftTrans, rightTrans;
    public Horse leftHorse, rightHorse; //TODO:修改为字典
    private Dictionary<Team, RoadInfo> infoDic;
    private GameCore core;
    private ShopManager shop;
    private TimerOnly roadTimer;
    private float spriteSize;

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
        infoDic.Add(Team.B,infob);
    }

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
        shop = ServiceLocator.Get<ShopManager>();
        core.RegisterRoad(this);
    }

    private void Update()
    {
        if (!ifStart) return;
        LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (!ifStart) return;
        MoveHorses(Time.fixedDeltaTime);
    }

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
        // leftHorse?.skill.OnStart();
        // rightHorse?.skill.OnStart();
        foreach (var info in infoDic.Values)
        {
            if(!info.horse)continue;
            info.horse.skill.OnStart();
        }
    }

    public void LogicUpdate()
    {
        if (finish) return;
        ClearTempStatus();
        TickSkill();
        CalcAttribute();
        if (CheckHit() || stalemated)
        {
            HorseFight();
            // Debug.Log($"相撞时间{roadTimer.Time}");
        }

        //检测终点
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            if (Mathf.Abs(info.horse.transform.position.x - info.startPoint.x) < roadLength) continue;
            finish = true;
            core.AddScore(info.team, 1);
            Debug.Log($"{info.team} Win At {roadTimer.Time}");
            // if (leftHorse != null && leftHorse.transform.position.x > rightPos.x)
            // {
            //     finish = true;
            //     core.AddScore(Team.A, 1);
            //     Debug.Log("AWIN");
            //     Debug.Log($"到达时间{roadTimer.Time}");
            // }
            //
            // if (rightHorse != null && rightHorse.transform.position.x < leftPos.x)
            // {
            //     finish = true;
            //     Debug.Log("BWIN");
            //     core.AddScore(Team.B, 1);
            //     Debug.Log($"到达时间{roadTimer.Time}");
            // }
        }
    }

    private void ClearTempStatus()
    {
        foreach (var info in infoDic.Values)
        {
            if(!info.horse)continue;
            info.horse.ClearStatus();
        }
        // leftHorse?.ClearStatus();
        // rightHorse?.ClearStatus();
    }

    private void CalcAttribute()
    {
        // leftHorse?.CalcDamageAndSpeed();
        // rightHorse?.CalcDamageAndSpeed();
        foreach (var info in infoDic.Values)
        {
            if(!info.horse)continue;
            info.horse.CalcDamageAndSpeed();
        }
    }

    private void TickSkill()
    {
    }


    public void MoveHorses(float dt)
    {
        if (finish) return;
        if (!stalemated)
        {
            // if (leftHorse != null && !leftHorse.HasStatus(EStatus.Die))
            // {
            //     leftHorse.transform.Translate(CalcVec(Team.A, dt, leftHorse.speed));
            // }
            //
            // if (rightHorse != null && !rightHorse.HasStatus(EStatus.Die))
            // {
            //     rightHorse.transform.Translate(CalcVec(Team.B, dt, rightHorse.speed));
            // }
            foreach (var info in infoDic.Values)
            {
                if(!info.horse)continue;
                if(info.horse.HasStatus(EStatus.Die))continue;
                info.horse.transform.Translate(CalcVec(info.team,dt,info.horse.speed));
            }
        }
    }

    private bool CheckHit()
    {
        bool flag=true;
        
        if (!infoDic[Team.A].horse || !infoDic[Team.B].horse) return false;
        return infoDic[Team.A].horse.transform.position.x >= infoDic[Team.B].horse.transform.position.x;
    }

    private Team HorseFight()
    {
        if (infoDic[Team.A].horse.damage > infoDic[Team.B].horse.damage)
        {
            infoDic[Team.B].horse.LoseCG();
            stalemated = false;
            return Team.A;
        }
        else if (infoDic[Team.A].horse.damage < infoDic[Team.B].horse.damage)
        {
            infoDic[Team.A].horse.LoseCG();
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
        if(horse.horseTeam==Team.A)
        {
            horse.HideSelf();
            shop.SetCoinTextUnknown(horse.price);
        }
        teamInfo.spriteSize=horse.GetComponent<SpriteRenderer>().bounds.size.x;
        shop.NextRound();

        // if (horse.horseTeam == Team.A)
        // {
        //     if (leftHorse != null) return;
        //     leftHorse = horse;
        //     horse.locateRoad = this;
        //     horse.transform.position = leftPos;
        //     horse.SetPutMode(horse.horseTeam, false);
        //     horse.SetDir(horse.horseTeam);
        //     horse.HideSelf();
        //     shop.SetCoinTextUnknown(horse.price);
        //     shop.NextRound();
        //     spriteSize = leftHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        // }
        // else if (horse.horseTeam == Team.B)
        // {
        //     if (rightHorse != null) return;
        //     horse.locateRoad = this;
        //     rightHorse = horse;
        //     horse.transform.position = rightPos;
        //     horse.SetPutMode(horse.horseTeam, false);
        //     horse.SetDir(horse.horseTeam);
        //     shop.NextRound();
        //     spriteSize = rightHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        // }
    }

    public void ShowHorses()
    {
        // leftHorse?.ShowSelf();
        // rightHorse?.ShowSelf();
        foreach (var info in infoDic.Values)
        {
            if(!info.horse)continue;
            info.horse.ShowSelf();
        }
    }
}