using System.Collections.Generic;

public class CoachSkill : Skill
{
    private List<Road> roads;
    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
        roads = core.GetRoadList();
    }

    public override void TickCheck()
    {
        base.TickCheck();
        if (silented) return;
        if (owner.locateRoad.num == 5) return;
        Horse downRoadHorse = roads[owner.locateRoad.num].GetHorse(owner.horseTeam);
        if (!downRoadHorse) return;
        // downRoadHorse.AddStatus()
    }
}