using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

// 屏幕淡入淡出管理器
public class ScreenFaderManager : MonoBehaviour
{
    [Header("序列")] // 序列相关设置
    public CutsceneTimelineBehaviour fadeFromBlackTimeline; // 从黑屏淡入的时间轴行为
    public CutsceneTimelineBehaviour fadeToBlackTimeline; // 淡入黑屏的时间轴行为

    [Header("自动淡入")] // 自动淡入相关设置
    public bool autoStartSceneWithFadeFromBlack; // 场景是否自动以从黑屏淡入开始

    [Header("事件")] // 事件相关设置
    public UnityEvent fadeFromBlackFinishedEvent; // 从黑屏淡入完成事件
    public UnityEvent fadeToBlackFinishedEvent; // 淡入黑屏完成事件

    void Start()
    {
        // 如果设置了自动从黑屏淡入，则开始淡入操作
        if (autoStartSceneWithFadeFromBlack)
        {
            StartFadeFromBlack();
        }
    }

    // 开始从黑屏淡入
    public void StartFadeFromBlack()
    {
        fadeFromBlackTimeline.StartTimeline();
    }

    // 开始淡入黑屏
    public void StartFadeToBlack()
    {
        fadeToBlackTimeline.StartTimeline();
    }

    // 从黑屏淡入完成
    public void FadeFromBlackFinished()
    {
        fadeFromBlackFinishedEvent.Invoke();
    }

    // 淡入黑屏完成
    public void FadeToBlackFinished()
    {
        fadeToBlackFinishedEvent.Invoke();
    }

}