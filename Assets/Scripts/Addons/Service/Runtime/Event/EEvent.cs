namespace Services
{
    public enum EEvent
    {
        /// <summary>
        /// 加载场景前，参数：即将加载的场景号
        /// </summary>
        BeforeLoadScene,
        /// <summary>
        /// 加载场景后（至少一帧以后），参数：刚加载好的场景号
        /// </summary>
        AfterLoadScene,
        /// <summary>
        /// 在商店的一个购买回合开始时,参数：购买者的队伍、当前购买回合数
        /// </summary>
        OnNextRound,
    }
}