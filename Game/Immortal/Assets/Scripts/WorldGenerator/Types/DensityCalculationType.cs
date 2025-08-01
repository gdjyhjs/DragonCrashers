using System;
/// <summary>
/// 密度计算类型
/// </summary>
public enum DensityCalculationType
{
    /// <summary>
    /// 独立计算：每个对象按自身密度单独生成，互不影响（如桃树密度2、苹果树密度2，各生成2棵）
    /// </summary>
    Independent,

    /// <summary>
    /// 共享计算：所有对象的总密度分摊到每个对象，平均分配（如桃树密度2、苹果树密度2，各生成1棵）
    /// </summary>
    Shared
}
