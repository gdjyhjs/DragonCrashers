using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

// 过场动画时间轴行为类
public class CutsceneTimelineBehaviour : MonoBehaviour
{

    [Header("时间轴")] // 时间轴相关设置
    public PlayableDirector cutsceneTimeline; // 过场动画时间轴

    [Header("标记事件")] // 标记事件相关设置
    public UnityEvent cutsceneTimelineFinished; // 过场动画时间轴完成事件

    // 开始时间轴播放
    public void StartTimeline()
    {
        cutsceneTimeline.Play();
    }

    // 时间轴播放完成
    public void TimelineFinished()
    {
        cutsceneTimelineFinished.Invoke();
    }
}