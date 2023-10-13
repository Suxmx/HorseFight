public class ChampionSkill : Skill
{
    public override void OnStart()
    {
        base.OnStart();
        if (silented) return;
        owner.score = 2;
    }
}