using System.Collections.Generic;
using UnityEngine;

public class LogisticsSkill : Skill
{
    private RoadInfo ownerInfo;
    private List<Road> roads;

    public override void OnStart()
    {
        base.OnStart();
        ownerInfo = owner.locateRoad.GetInfo(owner.horseTeam);
        roads = roadManager.GetRoads();
    }

    public override void TickCheck()
    {
        base.TickCheck();
        if (silented) return;
        float ownerLen = Mathf.Abs(ownerInfo.startPoint.x - owner.transform.position.x);
        foreach (var road in roads)
        {
            RoadInfo roadInfo = road.GetInfo(owner.horseTeam);
            if (!roadInfo.horse || road.num == owner.locateRoad.num) continue;
            float len = Mathf.Abs(roadInfo.startPoint.x - roadInfo.horse.transform.position.x);
            if (ownerLen < len&& Mathf.Abs(ownerLen-len)>0.05f)
            {
                Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发\nRoad {road.num}");
                roadInfo.horse.AddStatus(EStatus.Logistics);
            }
        }
    }
}