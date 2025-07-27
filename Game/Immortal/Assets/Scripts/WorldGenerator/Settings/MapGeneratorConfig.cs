using UnityEngine;

[CreateAssetMenu(fileName = "世界地图生成配置", menuName = "地图/世界地图生成配置", order = 1)]
public class MapGeneratorConfig : ScriptableObject
{
    /// <summary>地图宽高格子数</summary>
    public Vector2Int mapSize = new Vector2Int(200, 200);

    /// <summary>生成宗门数量</summary>
    public int sectCount = 7;

    /// <summary>生成城市数量</summary>
    public int cityCount = 12;

    /// <summary>生成村庄数量</summary>
    public int villageCount = 108;

    /// <summary>生成部落数量</summary>
    public int tribeCount = 12;

    /// <summary>宗门占用格子数（20%波动）</summary>
    public int sectSize = 72;

    /// <summary>城市占用格子数（20%波动）</summary>
    public int citySize = 36;

    /// <summary>村庄占用格子数（20%波动）</summary>
    public int villageSize = 8;

    /// <summary>部落占用格子数（20%波动）</summary>
    public int tribeSize = 12;

    /// <summary>城镇、宗门之间最小距离</summary>
    public int townSectMinDistance = 324;

    /// <summary>村庄、部落之间最小距离</summary>
    public int villageTribeMinDistance = 36;

    /// <summary>
    /// 小型岛屿数量
    /// </summary>
    public int smallIslandCount = 12;

    /// <summary>
    /// 中型岛屿数量
    /// </summary>
    public int mediumIslandCount = 6;

    /// <summary>
    /// 大型岛屿数量
    /// </summary>
    public int largeIslandCount = 3;

    /// <summary>
    /// 小型岛屿占地大小
    /// </summary>
    public int smallIslandSize = 20;

    /// <summary>
    /// 中型岛屿占地大小
    /// </summary>
    public int mediumIslandSize = 60;

    /// <summary>
    /// 大型岛屿占地大小
    /// </summary>
    public int largeIslandSize = 120;

    /// <summary>
    /// 边界影响区域比例（10%地图大小）
    /// </summary>
    public float borderInfluenceRatio = 0.1f;

    /// <summary>
    /// 陆地占比
    /// </summary>
    public float continentRatio = 0.5f;

}

// 居住点最大占用格子数： 宗门605  城市519  部落：172  村庄：1037   总：2,333   预计陆地面积：4666   预计地图总面积：6665  预计边长 82
