using System.Collections.Generic;
using UnityEngine;

public class GiantSkill : Skill
{
    private bool triggered = false;
    public override void OnKill(int overflow)
    {
        base.OnKill(overflow);
        if(silented|| triggered)return;
        List<Road> roads = roadManager.GetRoads();
        int up = owner.locateRoad.num - 2, down = owner.locateRoad.num;
        if (up >= 0)
        {
            Horse horse = roads[up].GetHorse(owner.horseTeam);
            if (horse)
            {
                horse.AddStatus(EStatus.Giant, overflow, 0);
                triggered = true;
                Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发\nRoad {up + 1}");
            }
        }
        if (down < 5)
        {
            Horse horse = roads[down].GetHorse(owner.horseTeam);
            if (horse)
            {
                horse.AddStatus(EStatus.Giant, overflow, 0);
                triggered = true;
                Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发\n Road {down + 1}");
            }
        }
    }
}