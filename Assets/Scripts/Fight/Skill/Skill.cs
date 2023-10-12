using System;
using MyTimer;
using Services;
using UnityEngine;

public class Skill: MonoBehaviour
{
    public bool onStartSilentAble;

    protected Horse owner;
    protected TimerOnly skillTimer;
    protected GameCore core;

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
    }

    public virtual void OnStart(){}
    public virtual void TickCheck(){}
    public virtual void OnDeath(){}
    public virtual void TimeBuff(){}
    public virtual void OnKill(){}
}