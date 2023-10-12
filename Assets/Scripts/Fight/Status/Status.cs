using Sirenix.OdinInspector;

public enum EStatus
{
    /// <summary>
    /// 死亡状态
    /// </summary>
    Die,

    /// <summary>
    /// 风暴技能，攻击+2
    /// </summary>
    Storm,
}

[System.Serializable]
public class Status
{
    public Status(EStatus statusTag, bool ifTmp, int damageBuffer = 0, int speedBuffer = 0)
    {
        this.ifTmp = ifTmp;
        this.damageBuffer = damageBuffer;
        this.speedBuffer = speedBuffer;
        this.statusTag = statusTag;
    }

    [LabelText("词条种类")] public EStatus statusTag;
    [LabelText("是否临时")] public bool ifTmp;
    [LabelText("攻击增益缓冲")] public int damageBuffer;
    [LabelText("速度增益缓冲")] public int speedBuffer;
}