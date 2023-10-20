using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AI
{
    public enum AIMode
    {
        Easy,
        Normal,
        Hard,
        ExtremeHard
    }

    [System.Serializable]
    public struct DifficultyConfig
    {
        [LabelText("初始拥有金币")] public int coin;
        [Header("概率")] [LabelText("针对出牌")] public float against;
        [LabelText("压制跟牌")] public float pressFollow;
        [LabelText("僵持跟牌")] public float stalemateFollow;
        [LabelText("随机出牌")] public float random;

        [DictionaryDrawerSettings(KeyLabel = "价格", ValueLabel = "概率")] [LabelText("随机出牌设置")]
        public Dictionary<int, float> randomBuyPossibility;
    }

    [CreateAssetMenu(menuName = "SO/AIConfigs", fileName = "AIConfigs")]
    public class AIConfig : SerializedScriptableObject
    {
        [LabelText("针对表")] [DictionaryDrawerSettings(KeyLabel = "玩家出牌", ValueLabel = "AI针对")]
        public Dictionary<EHorse, EHorse> againstDic;

        [LabelText("难度设置")][DictionaryDrawerSettings(KeyLabel = "难度", ValueLabel = "设置")]
        public Dictionary<AIMode, DifficultyConfig> configDic;
    }
}