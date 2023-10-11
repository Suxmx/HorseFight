using MyTimer;
using UnityEngine;

public class Skill: MonoBehaviour
{
    public bool onStartSilentAble;
    public bool teamBuffSilentAble;
    public bool onDeathSilentAble;
    public bool timeBuffSilentAble;
    public bool onKillSilentAble;

    protected Horse owner;
    protected TimerOnly skillTimer;
    
    public virtual void OnStart(){}
    public virtual void TeamBuff(){}
    public virtual void OnDeath(){}
    public virtual void TimeBuff(){}
    public virtual void OnKill(){}
}