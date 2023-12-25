using System;
using System.Collections;
using System.Collections.Generic;
using MyTimer;
using Services;
using Shop;
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
    public GameObject halo;
}

public class Road : MonoBehaviour
{
    public float roadLength => rightPos.x - leftPos.x;
    public int num => Convert.ToInt32((name.Split("Road ")[1]));
    [LabelText("僵持")] public bool stalemated = false;
    [LabelText("是否开始")] public bool ifStart = false;

    [Header("音效")] public AudioSource onwin;
    public AudioSource onput;
    public AudioSource onhit;
    public bool end => stalemated || hasHorseWin || (!GetHorse(Team.A) && !GetHorse(Team.B));

    private Vector2 leftPos => leftTrans.position;
    private Vector2 rightPos => rightTrans.position;
    private Transform leftTrans, rightTrans;
    private Dictionary<Team, RoadInfo> infoDic; //TODO:实现多马匹支持,但是似乎优先级较低
    private GameCore core;
    private RoadManager roadManager;
    private IShop shop;
    private TimerOnly roadTimer;
    private float spriteSize;
    private bool hasHorseWin = false;
    private GameObject halo;

    private float unitLength => roadLength / 20f;
    // private float curTime => core.curTime;

    private void Awake()
    {
        roadTimer = new TimerOnly(true);
        leftTrans = transform.Find("LeftStart");
        rightTrans = transform.Find("RightStart");
        RoadInfo infoa = new(Team.A, leftPos), infob = new(Team.B, rightPos);
        infoa.halo = leftTrans.Find("halo").gameObject;
        infob.halo = rightTrans.Find("halo").gameObject;

        infoDic = new Dictionary<Team, RoadInfo>();
        infoDic.Add(Team.A, infoa);
        infoDic.Add(Team.B, infob);
        roadManager = GameObject.Find("RoadManager").GetComponent<RoadManager>(); //为了在Awake中注册该road
        roadManager.RegisterRoad(this);
    }

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
        shop = core.GetShop();
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
        foreach (var info in infoDic.Values)
        {
            if (!info.horse) continue;
            info.horse.skill.OnStart();
        }
    }

    public void LogicUpdate()
    {
        if (CheckHit()) //重构逻辑防止多次触发Hit
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
        if ((aHorse.type == EHorse.淑女 && !aHorse.skill.silented) ||
            (bHorse.type == EHorse.淑女 && !bHorse.skill.silented))
            return false;
        return stalemated || ((aHorse.transform.position.x >= bHorse.transform.position.x) &&
                              (!aHorse.HasStatus(EStatus.Die) && !bHorse.HasStatus(EStatus.Die)));
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
                onwin.Play();
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
            if (!stalemated) onhit.Play();
            stalemated = true;
            return Team.None;
        }
    }

    public bool SetHorse(Horse horse)
    {
        var teamInfo = infoDic[horse.horseTeam];
        if (teamInfo.horse) return false;
        onput.Play();
        teamInfo.horse = horse;
        horse.locateRoad = this;
        horse.transform.position = teamInfo.startPoint;
        // horse.SetPutMode(horse.horseTeam, false);
        horse.SetDir(horse.horseTeam);
        horse.skill.OnPut();
        if (horse.horseTeam == Team.A&& !core.ifAI && !core._ifRandom)
        {
            horse.HideSelf();
            if(shop is ShopManager manager)
                manager.SetCoinTextUnknown(horse.price);
        }

        shop.NextRound();
        return true;
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

    public void ShowHalo(Team team)
    {
        var teamInfo = infoDic[team];
        if (teamInfo.horse) return ;
        teamInfo.halo.SetActive(true);
    }

    public void HideHalo(Team team)
    {
        infoDic[team].halo.SetActive(false);
    }
}