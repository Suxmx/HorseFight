using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class RoadManager: Service
{
    private List<Road> roads;
    private bool ifStart=false;
    private GameCore core;
    protected override void Start()
    {
        base.Start();
        core = ServiceLocator.Get<GameCore>();
    }

    public void Init(List<Road> roads)
    {
        this.roads = roads;
    }

    private void Update()
    {
        if (!ifStart) return;
        LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (!ifStart) return;
        PhysicsUpdate();
    }

    public void LogicUpdate()
    {
        foreach (var road in roads)
        {
            road.ClearTempStatus();
        }
        foreach (var road in roads)
        {
            road.TickSkill();
        }
        foreach (var road in roads)
        {
            road.CalcAttribute();
        }
        foreach (var road in roads)
        {
            road.LogicUpdate();
        }
        CheckWin();        
    }

    public void PhysicsUpdate()
    {
        foreach (var road in roads)
        {
            road.MoveHorses(Time.fixedDeltaTime);
        }
    }

    public void OnStart()
    {
        foreach (var road in roads)
        {
            road.OnStart();
        }

        ifStart = true;
    }

    public void CheckWin()
    {
        bool flag=true;
        foreach (var road in roads)
        {
            if (!road.end) flag = false;
        }
        if(flag)
        {
            ifStart = false;
            core.OnGameEnd();
        }
    }
}