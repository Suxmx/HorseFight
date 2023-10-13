using System.Collections.Generic;
using System.Linq;

public class FireHorse : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
        List<Road> roads = core.GetRoadList();
        int up = owner.locateRoad.num - 2, down = owner.locateRoad.num;
        if (up >= 0)
        {
            Horse horse = roads[up].GetHorse(owner.horseTeam);
            if (!horse) return;
            horse.AddStatus(EStatus.FireHorse);
        }
        if (down < 5)
        {
            Horse horse = roads[down].GetHorse(owner.horseTeam);
            if (!horse) return;
            horse.AddStatus(EStatus.FireHorse);
        }
    }
}