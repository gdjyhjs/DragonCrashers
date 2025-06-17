using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

// ��Ļ���뵭��������
public class ScreenFaderManager : MonoBehaviour
{
    [Header("����")] // �����������
    public CutsceneTimelineBehaviour fadeFromBlackTimeline; // �Ӻ��������ʱ������Ϊ
    public CutsceneTimelineBehaviour fadeToBlackTimeline; // ���������ʱ������Ϊ

    [Header("�Զ�����")] // �Զ������������
    public bool autoStartSceneWithFadeFromBlack; // �����Ƿ��Զ��ԴӺ������뿪ʼ

    [Header("�¼�")] // �¼��������
    public UnityEvent fadeFromBlackFinishedEvent; // �Ӻ�����������¼�
    public UnityEvent fadeToBlackFinishedEvent; // �����������¼�

    void Start()
    {
        // ����������Զ��Ӻ������룬��ʼ�������
        if (autoStartSceneWithFadeFromBlack)
        {
            StartFadeFromBlack();
        }
    }

    // ��ʼ�Ӻ�������
    public void StartFadeFromBlack()
    {
        fadeFromBlackTimeline.StartTimeline();
    }

    // ��ʼ�������
    public void StartFadeToBlack()
    {
        fadeToBlackTimeline.StartTimeline();
    }

    // �Ӻ����������
    public void FadeFromBlackFinished()
    {
        fadeFromBlackFinishedEvent.Invoke();
    }

    // ����������
    public void FadeToBlackFinished()
    {
        fadeToBlackFinishedEvent.Invoke();
    }

}