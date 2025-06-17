using System;
using System.Collections.Generic;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��InventoryScreen/InventoryController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class InventoryEvents
    {
        // װ����Ʒ�����ʱ�������¼�
        public static Action<GearItemComponent> GearItemClicked;

        // ���������ʱ�������¼�
        public static Action ScreenEnabled;

        // ѡ��װ����Ʒʱ�������¼�
        public static Action<EquipmentSO> GearSelected;

        // ����װ����Ʒʱ�������¼�
        public static Action<Rarity, EquipmentType> GearFiltered;

        // ��ʼ����ʱ�������¼�
        public static Action InventorySetup;

        // ˢ�¿��ʱ�������¼�
        public static Action<List<EquipmentSO>> InventoryUpdated;

        // �ӽ�ɫ�����Զ�װ��ʱ�������¼�
        public static Action<List<EquipmentSO>> GearAutoEquipped;
    }
}