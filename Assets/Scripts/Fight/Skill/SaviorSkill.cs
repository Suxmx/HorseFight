using System.Collections.Generic;
using UnityEngine;

public class SaviorSkill : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        if(silented)return;
        List<Road> roads = core.GetRoadList();
        int rand=Random.Range(0,5);
        Horse randHorse = null;
        while(rand==owner.locateRoad.num-1||!randHorse )
        {
            rand = Random.Range(0, 5);
            randHorse = roads[rand].GetHorse(owner.horseTeam);
            
        }
        randHorse.AddStatus(EStatus.Savior);
        Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} 技能触发\n对象: Road {rand+1}");
    }
}