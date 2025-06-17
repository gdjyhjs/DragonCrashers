using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��ShopScreen/ShopScreenController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class ShopEvents
    {
        // ���̵����ѡ���ұ�ǩʱ����
        public static Action GoldSelected;
        // ���̵����ѡ��ʯ��ǩʱ����
        public static Action GemSelected;
        // ���̵����ѡ��ҩ����ǩʱ����
        public static Action PotionSelected;

        // ͨ������ѡ���ǩʱ����
        public static Action<string> TabSelected;

        // ���ShopItemComponent�ϵĹ���ť�������̵���Ʒ���ݺ���Ļ���λ�ã�
        public static Action<ShopItemSO, Vector2> ShopItemClicked;

        // ֪ͨShopScreenController
        public static Action<ShopItemSO, Vector2> ShopItemPurchasing;

        // �̵����ʱ����
        public static Action<List<ShopItemSO>> ShopUpdated;

        // ����ҩ��ʱ������Ϸ����
        public static Action<GameData> PotionsUpdated;

        // ������Ʒʱ����ѡ����
        public static Action<GameData> FundsUpdated;

        // �����ʼ���Ϣ�е��������
        public static Action<ShopItemType, uint, Vector2> RewardProcessed;

        // ��Ʒ����ɹ�
        public static Action<ShopItemSO, Vector2> TransactionProcessed;

        // �ʽ��㵼�¹���ʧ��
        public static Action<ShopItemSO> TransactionFailed;
    }
}