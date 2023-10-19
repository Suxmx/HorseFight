using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreezeSkill : Skill
{
    private List<Road> roads;

    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
        Team another = owner.horseTeam == Team.A ? Team.B : Team.A;
        roads = roadManager.GetRoads().ToList(); //副本
        roads.Remove(owner.locateRoad);
        Debug.Log(roads.Count);
        for (int i = 1; i <= 3 && roads.Count > 0; i++)
        {
            int rand = Random.Range(0, roads.Count);
            Horse horse = roads[rand].GetHorse(another);
            roads.Remove(roads[rand]);
            if (!horse)
            {
                i--;
                continue;
            }

            horse.AddStatus(EStatus.Freeze);
        }
    }
}