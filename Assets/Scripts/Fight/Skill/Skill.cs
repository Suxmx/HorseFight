using System;
using MyTimer;
using Services;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public bool onStartSilentAble;

    protected Horse owner;
    protected TimerOnly skillTimer;
    protected GameCore core;
    protected bool skillOnStart = false;

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
    }

    public virtual void OnStart()
    {
        if (onStartSilentAble)
        {
            Team another = owner.horseTeam == Team.A ? Team.B : Team.A;
            Horse horse = owner.locateRoad.GetHorse(another);
            if (horse && horse.type == EHorse.沉默者)
            {
                Debug.Log("被沉默");
                skillOnStart = true;
            }
        }
    }

    public virtual void TickCheck()
    {
    }

    public virtual void OnDeath()
    {
    }

    public virtual void TimeBuff()
    {
    }

    public virtual void OnKill()
    {
    }
}