/*
�ű���Դ: https://coffeebraingames.wordpress.com/2013/12/18/a-generic-floating-point-comparison-class/
*/

using UnityEngine;

/**
 * ���ڱȽϸ�����ֵ���� 
 */
public static class Comparison
{

    /**
     * ���� a �Ƿ���� b
     */
    public static bool TolerantEquals(float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    /**
     * ���� a �Ƿ���ڻ���� b
     */
    public static bool TolerantGreaterThanOrEquals(float a, float b)
    {
        return a > b || TolerantEquals(a, b);
    }

    /**
     * ���� a �Ƿ�С�ڻ���� b
     */
    public static bool TolerantLesserThanOrEquals(float a, float b)
    {
        return a < b || TolerantEquals(a, b);
    }

}