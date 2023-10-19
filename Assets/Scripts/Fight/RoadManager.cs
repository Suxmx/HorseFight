using System;
using System.Collections.Generic;
using System.Linq;
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
        roads = roads.OrderBy(road => road.gameObject.name).ToList();
    }

    // public void Init(List<Road> roads)
    // {
    //     this.roads = roads;
    // }

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

    private void CheckWin()
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

    public void RegisterRoad(Road road)
    {
        if (roads == null) roads = new List<Road>();
        roads.Add(road);
    }

    public List<Road> GetRoads()
    {
        return roads;
    }

    /// <summary>
    /// 根据道路序号来获得Road
    /// </summary>
    /// <param name="num">Road序号 从一开始</param>
    /// <returns></returns>
    public Road GetRoad(int num)
    {
        if (num < 1 || num > roads.Count)
        {
            return null;
        }

        return roads[num - 1];
    }

    public void ShowAllHorses()
    {
        foreach (var road in roads)
        {
            road.ShowHorses();
        }
    }
}