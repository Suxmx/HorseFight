public class NofaceSkill : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        if(silented)return;
        Team another = owner.horseTeam == Team.A ? Team.B : Team.A;
        Horse horse = owner.locateRoad.GetHorse(another);
        if (!horse) return;
        owner.oriDamage = horse.damage;
    }
}