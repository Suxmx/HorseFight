using System;
using MyTimer;
using Services;
using UnityEngine;

public class Skill: MonoBehaviour
{
    public bool onStartSilentAble;

    protected Horse owner;
    protected TimerOnly skillTimer;
    protected StatusFactory factory;

    private void Start()
    {
        factory = ServiceLocator.Get<StatusFactory>();
    }

    public virtual void OnStart(){}
    public virtual void TeamBuff(){}
    public virtual void OnDeath(){}
    public virtual void TimeBuff(){}
    public virtual void OnKill(){}
}