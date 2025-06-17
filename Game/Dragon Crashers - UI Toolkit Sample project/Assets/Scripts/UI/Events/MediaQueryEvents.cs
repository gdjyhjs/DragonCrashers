using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// �������Ļ�ֱ��ʺͿ�߱���صĹ�����̬ί�С�
    /// ����֪ͨ�κμ�������/�����л��������
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public class MediaQueryEvents
    {
        // ��Ļ�ߴ����ʱ����
        public static Action<Vector2> ResolutionUpdated;

        // ��߱ȸ���ʱ����
        public static Action<MediaAspectRatio> AspectRatioUpdated;

        // �����С����ʱ����
        public static Action CameraResized;

        // Ӧ�ð�ȫ����ʱ����
        public static Action SafeAreaApplied;
    }
}