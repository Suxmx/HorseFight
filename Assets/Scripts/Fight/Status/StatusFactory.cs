﻿using System;
using System.Collections.Generic;
using Services;

public class StatusFactory : Service
{
    private Dictionary<EStatus, Func<Status>> facDic = new Dictionary<EStatus, Func<Status>>()
    {
        { EStatus.Die, () => new Status(EStatus.Die, false) },
        { EStatus.End, () => new Status(EStatus.End, false) },
        { EStatus.Storm, () => new Status(EStatus.Storm, false, damageBuffer: 2) },
        { EStatus.Growth, () => new Status(EStatus.Growth, false, damageBuffer: 1, repeatable: true) },
        { EStatus.Coach, () => new Status(EStatus.Coach, false, damageBuffer: 1, repeatable: false) },
        { EStatus.Savior, () => new Status(EStatus.Savior, false, damageBuffer: 2, repeatable: true) },
        { EStatus.Boomed, () => new Status(EStatus.Boomed, false, damageBuffer: -1, repeatable: true) },
        { EStatus.Logistics, () => new Status(EStatus.Logistics, true, damageBuffer: 1, repeatable: true) },
        { EStatus.Rush, () => new Status(EStatus.Rush, true, damageBuffer: 1, repeatable: true) },
        {
            EStatus.Freeze, () =>
            {
                Status tmp = new Status(EStatus.Freeze, false, true);
                tmp.SetTimer(3);
                return tmp;
            }
        },
        { EStatus.FireHorse ,()=>new Status(EStatus.FireHorse,false,speedBuffer:1)},
        { EStatus.Giant ,()=>new Status(EStatus.Giant,false,repeatable:true)}
    };

    private Dictionary<EStatus, Status> templateDic = new Dictionary<EStatus, Status>();

    public Status GetStatus(EStatus status)
    {
        if (facDic.TryGetValue(status, out var statusClass))
        {
            return statusClass();
        }

        throw new ArgumentException("Unknown Status type");
    }

    public Status GetTemplateStatus(EStatus status)
    {
        if (templateDic.TryGetValue(status, out var statusClass))
        {
            return statusClass;
        }

        throw new ArgumentException("Unknown Status type");
    }

    protected override void Awake()
    {
        base.Awake();
        foreach (var e in facDic.Keys)
        {
            templateDic.Add(e, GetStatus(e));
        }
    }
}