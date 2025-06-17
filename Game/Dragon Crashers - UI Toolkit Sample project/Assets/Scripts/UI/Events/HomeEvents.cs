using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��HomeScreen/HomeScreenController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class HomeEvents
    {
        // ���������ʱ��ʾ��ӭ��Ϣ
        public static Action<string> HomeMessageShown;

        // ��ʾ�ؿ���Ϣ���¼�
        public static Action<LevelSO> LevelInfoShown;

        // ����/��ʾ���촰�����ݵ��¼�
        public static Action<List<ChatSO>> ChatWindowShown;

        // �˳����˵�ʱ�������¼�
        public static Action MainMenuExited;

        // �����ʼ��ťʱ�������¼�
        public static Action PlayButtonClicked;
    }
}