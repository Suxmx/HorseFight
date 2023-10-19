using System.Collections.Generic;
using UnityEngine;

public class CoachSkill : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
    }

    public override void TickCheck()
    {
        base.TickCheck();
        if (silented) return;
        if (owner.locateRoad.num == 5) return;
        Horse downRoadHorse = roadManager.GetRoad(owner.locateRoad.num+1).GetHorse(owner.horseTeam);
        if (!downRoadHorse) return;
        downRoadHorse.AddStatus(EStatus.Coach);
        Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type} 技能触发");

    }
}