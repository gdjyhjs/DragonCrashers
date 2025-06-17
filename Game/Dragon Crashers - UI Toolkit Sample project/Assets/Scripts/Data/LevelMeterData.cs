using Unity.Properties;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// 表示用于等级计量器的等级数据，该计量器跟踪玩家的总等级
    /// 并通过数据绑定提供相应的排名。
    /// </summary>
    public class LevelMeterData
    {
        // 存储玩家的总等级数
        int m_TotalLevels;

        /// <summary>
        /// 累积的总等级数。此属性用于数据绑定。
        /// </summary>
        [CreateProperty]
        public int TotalLevels { get => m_TotalLevels; set => m_TotalLevels = value; }

        /// <summary>
        /// 构造函数，使用指定的总等级初始化LevelMeterData类。
        /// </summary>
        public LevelMeterData(int totalLevels)
        {
            m_TotalLevels = Mathf.Clamp(totalLevels, 0, 100);
        }

        /// <summary>
        /// 将玩家的总等级映射到排名字符串。
        /// </summary>
        /// <param name="level">玩家的总等级。</param>
        public static string GetRankFromLevel(int level)
        {
            if (level >= 100) return "终极冠军";
            if (level >= 95) return "至尊冠军";
            if (level >= 90) return "皇家冠军";
            if (level >= 85) return "精英冠军";
            if (level >= 80) return "大冠军";
            if (level >= 75) return "资深冠军";
            if (level >= 70) return "冠军";
            if (level >= 65) return "大师IV";
            if (level >= 60) return "大师III";
            if (level >= 55) return "大师II";
            if (level >= 50) return "大师I";
            if (level >= 45) return "挑战者IV";
            if (level >= 40) return "挑战者III";
            if (level >= 35) return "挑战者II";
            if (level >= 30) return "挑战者I";
            if (level >= 27) return "见习生III";
            if (level >= 23) return "见习生II";
            if (level >= 20) return "见习生I";
            if (level >= 15) return "探索者III";
            if (level >= 12) return "探索者II";
            if (level >= 10) return "探索者I";
            if (level >= 8) return "学徒II";
            if (level >= 6) return "学徒I";
            if (level >= 4) return "新手III";
            if (level >= 3) return "新手II";
            if (level >= 1) return "新手I";
            return "未排名";
        }
    }


}