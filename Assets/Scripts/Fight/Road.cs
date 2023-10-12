using System;
using System.Collections;
using System.Collections.Generic;
using MyTimer;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class Road : MonoBehaviour
{
    public float roadLength => rightPos.x - leftPos.x;
    public int num => Convert.ToInt32((name.Split("Road ")[1]));
    [LabelText("结束")]public bool finish = false;
    [LabelText("僵持")]public bool stalemated=false;
    [LabelText("是否开始")]public bool ifStart=false;

    private Vector2 leftPos => leftTrans.position;
    private Vector2 rightPos => rightTrans.position;
    private Transform leftTrans, rightTrans;
    [SerializeField]private Horse leftHorse, rightHorse;
    private GameCore core;
    private ShopManager shop;
    private TimerOnly roadTimer;
    private float spriteSize;

    private float unitLength => roadLength / 20f;
    // private float curTime => core.curTime;

    private void Awake()
    {
        leftTrans = transform.Find("LeftStart");
        rightTrans = transform.Find("RightStart");
        roadTimer = new TimerOnly(true);
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

    private Vector2 CalcVec(Team team, float dt,int speed)
    {
        Vector2 dirVec = team == Team.A ? Vector2.right : Vector2.left;
        return  dirVec * (dt * unitLength * speed);
    }

    public void OnStart()
    {
        ifStart = true;
        roadTimer.Restart();
        leftHorse?.ShowSelf();
        rightHorse?.ShowSelf();
    }

    public void LogicUpdate()
    {
        if (finish) return;
        //TODO: 添加马匹状态检测
        
        if (CheckHit()|| stalemated)
        {
            HorseFight();
            // Debug.Log($"相撞时间{roadTimer.Time}");
        }
        
        if(leftHorse!=null&&leftHorse.transform.position.x>rightPos.x)
        {
            finish = true;
            core.AddScore(Team.A,1);
            Debug.Log("AWIN");
            Debug.Log($"到达时间{roadTimer.Time}");
        }

        if (rightHorse!=null&&rightHorse.transform.position.x < leftPos.x)
        {
            finish = true;
            Debug.Log("BWIN");
            core.AddScore(Team.B,1);
            Debug.Log($"到达时间{roadTimer.Time}");
        }
    }
    

    public void MoveHorses(float dt)
    {
        if (finish) return;
        if(!stalemated)
        {
            if (leftHorse!=null &&!leftHorse.HasStatus(EStatus.Die))
            {
                leftHorse.transform.Translate(CalcVec(Team.A, dt, leftHorse.speed));
            }

            if (rightHorse!=null&&!rightHorse.HasStatus(EStatus.Die))
            {
                rightHorse.transform.Translate(CalcVec(Team.B, dt, rightHorse.speed));
            }
        }
    }

    private bool CheckHit()
    {
        if (leftHorse == null || rightHorse == null) return false;
        return leftHorse.transform.position.x >= rightHorse.transform.position.x;
    }

    private Team HorseFight()
    {
        if (leftHorse.damage > rightHorse.damage)
        {
            rightHorse.LoseCG();
            stalemated = false;
            return Team.A;
        }
        else if (leftHorse.damage < rightHorse.damage)
        {
            leftHorse.LoseCG();
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
        if (horse.horseTeam == Team.A)
        {
            if(leftHorse!=null)return;
            leftHorse = horse;
            horse.transform.position = leftPos;
            horse.SetPutMode(horse.horseTeam,false);
            horse.SetDir(horse.horseTeam);
            horse.HideSelf();
            shop.NextRound();
            spriteSize = leftHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else if (horse.horseTeam == Team.B)
        {
            if(rightHorse!=null)return;
            rightHorse = horse;
            horse.transform.position = rightPos;
            horse.SetPutMode(horse.horseTeam,false);
            horse.SetDir(horse.horseTeam);
            horse.HideSelf();
            shop.NextRound();
            spriteSize = rightHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        
    }
}