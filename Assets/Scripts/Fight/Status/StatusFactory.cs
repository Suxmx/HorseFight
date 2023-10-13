using System;
using System.Collections.Generic;
using Services;

public class StatusFactory: Service
{
    private Dictionary<EStatus, Func<Status>> facDic=new Dictionary<EStatus, Func<Status>>()
    {
        { EStatus.Die ,()=>new Status(EStatus.Die,false)},
        { EStatus.Storm ,()=>new Status(EStatus.Storm,false,damageBuffer:2)},
        { EStatus.Growth ,()=>new Status(EStatus.Growth,false,damageBuffer:1,repeatable:true)},
        // {E}
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
            templateDic.Add(e,GetStatus(e));
        }
    }
}