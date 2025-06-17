using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���ڹ������˵�UI���ĵĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class MainMenuUIEvents
    {
        // ��ʾ�������Կ�ʼ��Ϸ
        public static Action HomeScreenShown;

        // ��ʾ��ɫ������ѡ���ɫ��װ��
        public static Action CharScreenShown;

        // ��ʾ��Ϣ���棬������Դ����
        public static Action InfoScreenShown;

        // ��ʾ�̵�����Թ�����/��ʯ/ҩ��
        public static Action ShopScreenShown;

        // ��ѡ������ʾ�̵����
        public static Action OptionsBarShopScreenShown;

        // ��ʾ�ʼ�����
        public static Action MailScreenShown;

        // ��ʾ���ý��渲�ǲ�
        public static Action SettingsScreenShown;

        // ��ʾ������
        public static Action InventoryScreenShown;

        // �������ý���
        public static Action SettingsScreenHidden;

        // ���ؿ�����
        public static Action InventoryScreenHidden;

        // ��ʾ��Ϸ�����Խ�����Ϸ
        public static Action GameScreenShown;

        // ��ʾ�µĲ˵�����ʱ����
        public static Action<MenuScreen> CurrentScreenChanged;

        // ��ǰ��ͼ����ʱ����
        public static Action<string> CurrentViewChanged;

        // ֪ͨ��ǩʽ�˵�����/ѡ���һ����ǩ
        public static Action<string> TabbedUIReset;
    }
}