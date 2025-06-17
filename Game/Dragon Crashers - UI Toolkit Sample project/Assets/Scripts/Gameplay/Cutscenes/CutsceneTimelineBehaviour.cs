using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

// ��������ʱ������Ϊ��
public class CutsceneTimelineBehaviour : MonoBehaviour
{

    [Header("ʱ����")] // ʱ�����������
    public PlayableDirector cutsceneTimeline; // ��������ʱ����

    [Header("����¼�")] // ����¼��������
    public UnityEvent cutsceneTimelineFinished; // ��������ʱ��������¼�

    // ��ʼʱ���Ქ��
    public void StartTimeline()
    {
        cutsceneTimeline.Play();
    }

    // ʱ���Ქ�����
    public void TimelineFinished()
    {
        cutsceneTimelineFinished.Invoke();
    }
}