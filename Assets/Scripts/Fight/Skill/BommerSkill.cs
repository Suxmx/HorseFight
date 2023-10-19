using System.Collections.Generic;
using UnityEngine;

public class BommerSkill : Skill
{
    private bool triggered = false;
    public override void OnDeath()
    {
        base.OnDeath();
        if (silented|| triggered) return;
        List<Road> roads = roadManager.GetRoads();
        float posx = owner.transform.position.x, unit = roads[0].roadLength / 20f;
        Team anotherTeam = owner.horseTeam == Team.A ? Team.B : Team.A;
        foreach (var road in roads)
        {
            Horse horse = road.GetHorse(anotherTeam);
            if (!horse) continue;
            if (Mathf.Abs(posx - horse.transform.position.x) / unit <= 2.5f)
            {
                Debug.Log(
                    $"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发\n距离:{Mathf.Abs(posx - horse.transform.position.x) / unit}");
                horse.AddStatus(EStatus.Boomed);
                
            }
        }
        triggered = true;
    }
}