/*
�ű���Դ: https://coffeebraingames.wordpress.com/2014/10/20/a-generic-duration-timer-class/
*/

using UnityEngine;

/**
 * ����ʵ�ּ�ʱ����ͨ���ࣨ����Ϊ��λ��
 */
public class DurationTimer
{

    private float polledTime; // �Ѽ�¼��ʱ��
    private float durationTime; // ����ʱ��

    /**
     * ʹ��ָ���ĳ���ʱ����й���
     */
    public DurationTimer(float durationTime)
    {
        Reset(durationTime);
    }

    /**
     * �ֶ����¼�ʱ��
     */
    public void UpdateTimer()
    {
        this.polledTime += Time.deltaTime;
    }

    /**
     * ���ü�ʱ��
     */
    public void Reset()
    {
        this.polledTime = 0;
    }

    /**
     * ���ü�ʱ���������µĳ���ʱ��
     */
    public void Reset(float durationTime)
    {
        Reset();
        this.durationTime = durationTime;
    }

    /**
     * ���ؼ�ʱ����ʱ���Ƿ��ѹ�ȥ
     */
    public bool HasElapsed()
    {
        return Comparison.TolerantGreaterThanOrEquals(this.polledTime, this.durationTime);
    }

    /**
     * �����Ѽ�¼ʱ�������ʱ��ı��ʡ�����ֵ���� 0 �� 1 ֮��
     */
    public float GetRatio()
    {
        if (Comparison.TolerantLesserThanOrEquals(this.durationTime, 0))
        {
            // ����ʱ��ֵ��Ч
            // �������ʱʱ��Ϊ�㣬���ʽ�Ϊ����󣨳����㣩
            // Ϊ�˰�ȫ������������ﷵ�� 1.0
            return 1.0f;
        }

        float ratio = this.polledTime / this.durationTime;
        return Mathf.Clamp(ratio, 0, 1);
    }

    /**
     * ���ؼ�ʱ�����������Ѽ�¼��ʱ��
     */
    public float GetPolledTime()
    {
        return this.polledTime;
    }

    /**
     * ǿ�Ƽ�ʱ������
     */
    public void EndTimer()
    {
        this.polledTime = this.durationTime;
    }

    /**
     * ���س���ʱ��
     */
    public float GetDurationTime()
    {
        return this.durationTime;
    }

}