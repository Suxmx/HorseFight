using System;
using System.Collections.Generic;
using Services;

public class StatusFactory: Service
{
    private Dictionary<EStatus, Func<Status>> facDic=new Dictionary<EStatus, Func<Status>>()
    {
        { EStatus.Die ,()=>new DieStatus()}
    };

    public Status GetStatus(EStatus status)
    {
        if (facDic.TryGetValue(status, out var statusClass))
        {
            return statusClass();
        }
        throw new ArgumentException("Unknown Status type");
    }
}