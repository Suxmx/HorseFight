using System.Collections.Generic;

public class SuperRushSkill : Skill
{
    public override void OnEnd()
    {
        base.OnEnd();
        if(silented)return;
        List<Road> roads = roadManager.GetRoads();
        int down = owner.locateRoad.num;
        if (down < 5)
        {
            Horse horse = roads[down].GetHorse(owner.horseTeam);
            if (!horse) return;
            horse.oriDamage = owner.oriDamage;
            horse.oriSpeed = owner.oriSpeed;
        }
    }
}