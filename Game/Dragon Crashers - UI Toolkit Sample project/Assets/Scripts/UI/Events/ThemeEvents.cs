using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// �����������صĹ�����̬ί�С�
    /// ����֪ͨ�κμ��������Ի����/����������ĵ������
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class ThemeEvents
    {
        // ����������¼����ַ�����ʾ�������ƣ�
        public static Action<string> ThemeChanged;

        // �����������ʱ�������¼�
        public static Action<Camera> CameraUpdated;
    }
}