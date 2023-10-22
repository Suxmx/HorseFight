using System;
using MyTimer;
using Services;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public bool silented=false;

    protected Horse owner;
    protected RepeatTimer skillTimer;
    protected RoadManager roadManager;

    protected virtual void Awake()
    {
        owner = transform.parent.GetComponent<Horse>();
    }

    private void Start()
    {
        roadManager = ServiceLocator.Get<RoadManager>();
    }

    public virtual void OnStart()
    {
        Team another = owner.horseTeam == Team.A ? Team.B : Team.A;
        Horse horse = owner.locateRoad.GetHorse(another);
        if (horse && horse.type == EHorse.刽子手)
        {
            Debug.Log($"Road {owner.locateRoad.num}: Team{owner.horseTeam} {owner.type}被沉默");
            silented = true;
            owner.RemoveStatus(EStatus.Savior);
        }
    }

    public virtual void TickCheck()
    {
    }

    public virtual void OnDeath()
    {
    }

    public virtual void OnKill(int overflow)
    {
    }

    public virtual void OnEnd()
    {
    }

    public virtual void OnPut(){ }

    public virtual void Init(RoadManager roadManager)
    {
        this.roadManager = roadManager;
    }
}