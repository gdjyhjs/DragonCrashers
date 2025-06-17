using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace UIToolkitDemo
{
    [Serializable]
    public struct MagnetData
    {
        // 金币、宝石、血瓶、能量药水
        public ShopItemType ItemType;

        // 粒子系统池
        public ObjectPoolBehaviour FXPool;

        // 力场目标
        public ParticleSystemForceField ForceField;
    }

    // 从商店购买物品时的特效生成器
    public class CoinMagnet : MonoBehaviour
    {

        [Header("UI元素")]
        [Tooltip("从该文档的UI元素中定位屏幕空间位置。")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("将每个ShopItemType与一个目标VisualElement按名称匹配")]
        [SerializeField] List<MagnetData> m_MagnetData;

        [Header("相机")]
        [Tooltip("使用相机和深度来计算世界空间位置。")]
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_ZDepth = 10f;
        [Tooltip("粒子发射的3D偏移")]
        [SerializeField] Vector3 m_SourceOffset = new Vector3(0f, 0.1f, 0f);

        // 效果的起始和结束坐标
        void OnEnable()
        {
            // 来自ShopScreen的特效
            ShopEvents.TransactionProcessed += OnTransactionProcessed;

            // 来自MailScreen的特效
            ShopEvents.RewardProcessed += OnRewardProcessed;

            ThemeEvents.CameraUpdated += OnCameraUpdated;
        }



        void OnDisable()
        {
            ShopEvents.TransactionProcessed -= OnTransactionProcessed;
            ShopEvents.RewardProcessed -= OnRewardProcessed;

            ThemeEvents.CameraUpdated += OnCameraUpdated;
        }

        ObjectPoolBehaviour GetFXPool(ShopItemType itemType)
        {
            MagnetData magnetData = m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.FXPool;
        }

        ParticleSystemForceField GetForcefield(ShopItemType itemType)
        {
            MagnetData magnetData = m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.ForceField;
        }

        void PlayPooledFX(Vector2 screenPos, ShopItemType contentType)
        {
            Vector3 worldPos = screenPos.ScreenPosToWorldPos(m_Camera, m_ZDepth) + m_SourceOffset;

            ObjectPoolBehaviour fxPool = GetFXPool(contentType);

            // 初始化粒子系统
            ParticleSystem ps = fxPool.GetPooledObject().GetComponent<ParticleSystem>();

            if (ps == null)
                return;

            ps.gameObject.SetActive(true);
            ps.gameObject.transform.position = worldPos;

            // 添加力场作为目标
            ParticleSystemForceField forceField = GetForcefield(contentType);
            forceField.gameObject.SetActive(true);

            // 更新力场相对于UI的位置
            PositionToVisualElement positionToVisualElement = forceField.gameObject.GetComponent<PositionToVisualElement>();
            positionToVisualElement.MoveToElement();

            // 将力场附加到粒子系统
            ParticleSystem.ExternalForcesModule externalForces = ps.externalForces;
            externalForces.enabled = true;
            externalForces.AddInfluence(forceField);

            ps.Play();

        }

        // 事件处理方法

        // 从MailScreen领取免费奖励
        void OnRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // 仅对金币或宝石购买播放特效
            if (rewardType == ShopItemType.HealthPotion || rewardType == ShopItemType.LevelUpPotion)
                return;

            PlayPooledFX(screenPos, rewardType);
        }

        // 从ShopScreen购买物品
        void OnTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // 仅对金币或宝石购买播放特效
            if (shopItem.ContentType == ShopItemType.HealthPotion || shopItem.ContentType == ShopItemType.LevelUpPotion)
                return;

            PlayPooledFX(screenPos, shopItem.ContentType);
        }

        void OnCameraUpdated(Camera camera)
        {
            m_Camera = camera;
        }
    }
}