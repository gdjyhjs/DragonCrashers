/*
脚本来源: https://coffeebraingames.wordpress.com/2014/10/20/a-generic-duration-timer-class/
*/

using UnityEngine;

/**
 * 用于实现计时器的通用类（以秒为单位）
 */
public class DurationTimer
{

    private float polledTime; // 已记录的时间
    private float durationTime; // 持续时间

    /**
     * 使用指定的持续时间进行构造
     */
    public DurationTimer(float durationTime)
    {
        Reset(durationTime);
    }

    /**
     * 手动更新计时器
     */
    public void UpdateTimer()
    {
        this.polledTime += Time.deltaTime;
    }

    /**
     * 重置计时器
     */
    public void Reset()
    {
        this.polledTime = 0;
    }

    /**
     * 重置计时器并分配新的持续时间
     */
    public void Reset(float durationTime)
    {
        Reset();
        this.durationTime = durationTime;
    }

    /**
     * 返回计时持续时间是否已过去
     */
    public bool HasElapsed()
    {
        return Comparison.TolerantGreaterThanOrEquals(this.polledTime, this.durationTime);
    }

    /**
     * 返回已记录时间与持续时间的比率。返回值仅在 0 到 1 之间
     */
    public float GetRatio()
    {
        if (Comparison.TolerantLesserThanOrEquals(this.durationTime, 0))
        {
            // 持续时间值无效
            // 如果倒计时时间为零，比率将为无穷大（除以零）
            // 为了安全起见，我们这里返回 1.0
            return 1.0f;
        }

        float ratio = this.polledTime / this.durationTime;
        return Mathf.Clamp(ratio, 0, 1);
    }

    /**
     * 返回计时器启动以来已记录的时间
     */
    public float GetPolledTime()
    {
        return this.polledTime;
    }

    /**
     * 强制计时器结束
     */
    public void EndTimer()
    {
        this.polledTime = this.durationTime;
    }

    /**
     * 返回持续时间
     */
    public float GetDurationTime()
    {
        return this.durationTime;
    }

}