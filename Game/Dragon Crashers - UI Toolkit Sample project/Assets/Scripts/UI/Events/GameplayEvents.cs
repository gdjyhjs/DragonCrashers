using System;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public static class GameplayEvents
    {
        // ս��ʤ���󴥷�
        public static Action WinScreenShown;

        // ս��ʧ�ܺ󴥷�
        public static Action LoseScreenShown;

        // ��ɫ����ʱ����
        public static Action<UnitController> CharacterCardHidden;

        // ���ø���ʱ����
        public static Action<GameData> SettingsUpdated;

        // ʹ����Ϸ���ݼ������ֺ���Ч��������
        public static Action SettingsLoaded;

        // ֪ͨ��������ָ����������ͣ��Ϸ
        public static Action<float> GamePaused;

        // ����ͣ����ָ���Ϸ
        public static Action GameResumed;

        // ����ͣ�����˳���Ϸ
        public static Action GameQuit;

        // ����ͣ�������¿�ʼ��Ϸ
        public static Action GameRestarted;

        // ����Ϸ�����е�����������
        public static Action<float> MusicVolumeChanged;

        // ����Ϸ�����е�����Ч����
        public static Action<float> SfxVolumeChanged;

        // ������ҩ���Ϸŵ��ض������Ʋ�VisualElement��
        public static Action<VisualElement> SlotHealed;

        // ��������ҩ��������
        public static Action<int> HealingPotionUpdated;

    }
}