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
    /// <summary>
    /// 成长技能，攻击+1
    /// </summary>
    Growth,
    /// <summary>
    /// 教官技能，正下方攻击+1
    /// </summary>
    Coach,
    /// <summary>
    /// 救星技能，随机单位攻击+2
    /// </summary>
    Savior,
    /// <summary>
    /// 到达终点状态
    /// </summary>
    End,
    /// <summary>
    /// 被爆弹魔攻击 -1攻击
    /// </summary>
    Boomed,
    /// <summary>
    /// 后勤技能，+1攻击
    /// </summary>
    Logistics,
    /// <summary>
    /// 冲锋技能，+1攻击
    /// </summary>
    Rush
}

[System.Serializable]
public class Status
{
    public Status(EStatus statusTag, bool ifTmp,bool repeatable=false ,int damageBuffer = 0, int speedBuffer = 0)
    {
        this.ifTmp = ifTmp;
        this.damageBuffer = damageBuffer;
        this.speedBuffer = speedBuffer;
        this.statusTag = statusTag;
        this.repeatable = repeatable;
    }

    [LabelText("词条种类")] public EStatus statusTag;
    [LabelText("是否临时")] public bool ifTmp;
    [LabelText("攻击增益缓冲")] public int damageBuffer;
    [LabelText("速度增益缓冲")] public int speedBuffer;
    [LabelText("叠加")]public bool repeatable;
}