using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��SettingsScreen/SettingsScreenController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class SettingsEvents
    {
        // ��������ʽ�ʱ����
        public static Action PlayerFundsReset;
        // ������ҵȼ�ʱ����
        public static Action PlayerLevelReset;

        // ���ý�����ʾʱ����
        public static Action SettingsShown;

        // ���ⱻѡ��ʱ����������Ϊ��������
        public static Action<string> ThemeSelected;

        // ��SettingsScreenControllerͬ��֮ǰ��������ݵ�SettingsScreen UI
        public static Action<GameData> GameDataLoaded;

        // �����º����Ϸ���ݸ�����UI���ݸ�������
        public static Action<GameData> UIGameDataUpdated;

        // �����º�����ݴӿ��������͸�������������GameDataManager��AudioManager�ȣ�
        public static Action<GameData> SettingsUpdated;

        // �л�FPS������ʱ����
        public static Action<bool> FpsCounterToggled;

        // ����Ŀ��֡��ʱ����
        public static Action<int> TargetFrameRateSet;
    }
}