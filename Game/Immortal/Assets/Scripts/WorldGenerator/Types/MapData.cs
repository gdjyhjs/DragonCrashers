using System;
/// <summary>
/// 地图数据枚举，使用[Flags]特性允许一个格子被标记为多种类型
/// </summary>
[Flags]
public enum MapData
{
    /// <summary>无任何类型</summary>
    None = 0,
    /// <summary>宗门 土黄色</summary>
    Sect = 1,
    /// <summary>城市 蓝色</summary>
    City = 2,
    /// <summary>部落 浅黄色</summary>
    Tribe = 4,
    /// <summary>村庄 浅蓝色</summary>
    Village = 8,
    /// <summary>道路 大红色</summary>
    Road = 16,
    /// <summary>航线 粉红色</summary>
    Route = 32,
    /// <summary>平原 淡绿色</summary>
    Plain = 64,


    /// <summary>大陆</summary>
    Continent = 128,
    /// <summary>岛屿</summary>
    Island = 256,

    /// <summary>山峰 灰色</summary>
    Mountain = 512,
    /// <summary>森林 墨绿色</summary>
    Forest = 1024,
    /// <summary>湖泊 青色</summary>
    Lake = 2048,
    /// <summary>河流 黄色</summary>
    River = 4096,
    /// <summary>码头 红色 </summary>
    Dock = 8192,
    /// <summary>桥 褐色</summary>
    Bridge = 16384,
    /// <summary>海洋 暗青色</summary>
    Ocean = 32768,
}
