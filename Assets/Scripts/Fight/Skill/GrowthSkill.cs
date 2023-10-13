using System;
using MyTimer;
using UnityEngine;

public class GrowthSkill: Skill
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnStart()
    {
        base.OnStart();
        skillTimer = new RepeatTimer();
        if (silented) return;
        skillTimer.Initialize(6,false);
        skillTimer.OnComplete += () =>
        {
            owner.AddStatus(EStatus.Growth);
            Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type} 技能触发");
        };
        skillTimer.Restart();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        skillTimer.Paused = true;
    }

    public override void OnWin()
    {
        base.OnWin();
        skillTimer.Paused = true;
    }
}