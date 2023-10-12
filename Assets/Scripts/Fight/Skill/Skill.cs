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
            Horse horse = owner.horseTeam == Team.A
                ? owner.locateRoad.rightHorse
                : owner.locateRoad.leftHorse;
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