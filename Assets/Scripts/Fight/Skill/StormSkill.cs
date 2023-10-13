using System;
using UnityEngine;

public class StormSkill: Skill
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
        if (owner.locateRoad.num == 3)
        {
            Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发");
            owner.AddStatus(EStatus.Storm);
        }
    }
}