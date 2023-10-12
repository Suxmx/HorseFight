public enum EStatus
{
    /// <summary>
    /// 死亡状态
    /// </summary>
    Die,
}

public class Status
{
    public EStatus StatusTag { get; protected set; }

    public bool silentAble;
    public bool ifSilented;
    public virtual void Buff(){}

    private Horse owner;
}