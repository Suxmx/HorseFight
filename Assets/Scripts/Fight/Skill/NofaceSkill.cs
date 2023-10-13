using UnityEngine;

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
        Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}技能触发\n 复制攻击力{owner.oriDamage}");
    }
}