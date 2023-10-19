using System.Collections.Generic;
using UnityEngine;

public class SaviorSkill : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        // if(silented)return;
        // List<Road> roads = core.GetRoadList();
        // int rand=Random.Range(0,5);
        // Horse randHorse = null;
        // while(rand==owner.locateRoad.num-1||!randHorse )
        // {
        //     rand = Random.Range(0, 5);
        //     randHorse = roads[rand].GetHorse(owner.horseTeam);
        //     
        // }
        // randHorse.AddStatus(EStatus.Savior);
        // Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} 技能触发\n对象: Road {rand+1}");
    }

    public override void OnPut()
    {
        // Team another = owner.horseTeam == Team.A ? Team.B : Team.A;
        // Horse anotherHorse = owner.locateRoad.GetHorse(another);
        // if (anotherHorse && anotherHorse.type == EHorse.沉默者)
        // {
        //     Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}被沉默");
        //     return;
        // }

        List<Road> roads = roadManager.GetRoads();
        List<Horse> horses = new List<Horse>();
        foreach (var road in roads)
        {
            Horse horse = road.GetHorse(owner.horseTeam);
            if (horse && horse != owner)
                horses.Add(horse);
        }

        if (horses.Count == 0) return;
        int rand = Random.Range(0, horses.Count);
        horses[rand].AddStatus(EStatus.Savior);
        horses[rand].CalcDamageAndSpeed();
        Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} 技能触发\n对象: Road {horses[rand].locateRoad.num}");
    }
}