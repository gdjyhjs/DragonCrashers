using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��CharScreen/CharScreenController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class CharEvents
    {
        // �ڽ�ɫ����ѡ����һ����ɫ
        public static Action NextCharacterSelected;
        // �ڽ�ɫ����ѡ����һ����ɫ
        public static Action LastCharacterSelected;

        // ��ɫ��������ʱ����
        public static Action ScreenStarted;

        // ��ɫ��������ʱ����
        public static Action ScreenEnded;

        // ʹ���ض���װ����Ʒ�������򿪿��
        public static Action<int> InventoryOpened;

        // �Զ�װ��װ��
        public static Action GearAutoEquipped;
        // ȫ��ж��װ��
        public static Action GearAllUnequipped;

        // ���������ť
        public static Action LevelUpClicked;

        // �ڽ�ɫ������ʾ������ť
        public static Action<bool> LevelUpButtonEnabled;

        // �������̳ɹ�/ʧ��
        public static Action<bool> CharacterLeveledUp;

        // ������ɫ���Եȼ�
        public static Action<CharacterData> LevelIncremented;

        // ���µȼ���������
        public static Action<float> LevelUpdated;

        // ��ɫԤ����ʼ���󴥷�
        public static Action PreviewInitialized;

        // ��ʾ��ɫ
        public static Action<CharacterData> CharacterShown;

        // װ����Ʒ��ж��ʱ����
        public static Action<EquipmentSO> GearItemUnequipped;

        // ����װ���ۣ��ṩװ�����ݺͲ�����
        public static Action<EquipmentSO, int> GearSlotUpdated;

        // �ڵ�ǰ��ɫ���Զ�װ��װ��
        // �ṩҪ�Զ�װ���Ľ�ɫ����
        public static Action<CharacterData> CharacterAutoEquipped;

        // ��ʼ�����н�ɫ����ʼװ��ʱ����
        // �ṩ������ʼװ�����ݵĽ�ɫ�б�
        public static Action<List<CharacterData>> GearDataInitialized;

        // ʹ������ҩ�����ṩ����ҩ���Ľ�ɫ����
        public static Action<CharacterData> LevelPotionUsed;

        // ��̬�����������ṩLevelMeterData
        public static Func<LevelMeterData> GetLevelMeterData;
    }
}