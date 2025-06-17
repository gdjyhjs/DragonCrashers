/*
脚本来源: https://coffeebraingames.wordpress.com/2013/12/18/a-generic-floating-point-comparison-class/
*/

using UnityEngine;

/**
 * 用于比较浮点数值的类 
 */
public static class Comparison
{

    /**
     * 返回 a 是否等于 b
     */
    public static bool TolerantEquals(float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    /**
     * 返回 a 是否大于或等于 b
     */
    public static bool TolerantGreaterThanOrEquals(float a, float b)
    {
        return a > b || TolerantEquals(a, b);
    }

    /**
     * 返回 a 是否小于或等于 b
     */
    public static bool TolerantLesserThanOrEquals(float a, float b)
    {
        return a < b || TolerantEquals(a, b);
    }

}