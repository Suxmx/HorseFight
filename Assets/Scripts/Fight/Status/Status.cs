public enum EStatus
{
    /// <summary>
    /// 死亡状态
    /// </summary>
    Die,
}

public class Status
{
    public Status(EStatus statusTag,bool silentAble, bool ifTmp, int damageBuffer=0, int speedBuffer=0)
    {
        this.silentAble = silentAble;
        this.ifTmp = ifTmp;
        this.damageBuffer = damageBuffer;
        this.speedBuffer = speedBuffer;
        StatusTag = statusTag;
    }

    public EStatus StatusTag { get; protected set; }

    public bool silentAble;
    public bool ifSilented;
    public bool ifTmp;
    public int damageBuffer;
    public int speedBuffer;
}