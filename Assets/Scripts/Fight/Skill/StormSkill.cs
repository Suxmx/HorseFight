using System;
using UnityEngine;

public class StormSkill: Skill
{
    private void Awake()
    {
        onStartSilentAble = true;
        owner = transform.parent.GetComponent<Horse>();
    }

    public override void OnStart()
    {
        base.OnStart();
        if (skillOnStart) return;
        if (owner.locateRoad.num == 3)
        {
            Debug.Log("风暴技能触发");
            owner.AddStatus(EStatus.Storm);
        }
    }
}