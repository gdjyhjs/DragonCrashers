using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace UIToolkitDemo
{
    [Serializable]
    public struct MagnetData
    {
        // ��ҡ���ʯ��Ѫƿ������ҩˮ
        public ShopItemType ItemType;

        // ����ϵͳ��
        public ObjectPoolBehaviour FXPool;

        // ����Ŀ��
        public ParticleSystemForceField ForceField;
    }

    // ���̵깺����Ʒʱ����Ч������
    public class CoinMagnet : MonoBehaviour
    {

        [Header("UIԪ��")]
        [Tooltip("�Ӹ��ĵ���UIԪ���ж�λ��Ļ�ռ�λ�á�")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("��ÿ��ShopItemType��һ��Ŀ��VisualElement������ƥ��")]
        [SerializeField] List<MagnetData> m_MagnetData;

        [Header("���")]
        [Tooltip("ʹ��������������������ռ�λ�á�")]
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_ZDepth = 10f;
        [Tooltip("���ӷ����3Dƫ��")]
        [SerializeField] Vector3 m_SourceOffset = new Vector3(0f, 0.1f, 0f);

        // Ч������ʼ�ͽ�������
        void OnEnable()
        {
            // ����ShopScreen����Ч
            ShopEvents.TransactionProcessed += OnTransactionProcessed;

            // ����MailScreen����Ч
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

            // ��ʼ������ϵͳ
            ParticleSystem ps = fxPool.GetPooledObject().GetComponent<ParticleSystem>();

            if (ps == null)
                return;

            ps.gameObject.SetActive(true);
            ps.gameObject.transform.position = worldPos;

            // ���������ΪĿ��
            ParticleSystemForceField forceField = GetForcefield(contentType);
            forceField.gameObject.SetActive(true);

            // �������������UI��λ��
            PositionToVisualElement positionToVisualElement = forceField.gameObject.GetComponent<PositionToVisualElement>();
            positionToVisualElement.MoveToElement();

            // ���������ӵ�����ϵͳ
            ParticleSystem.ExternalForcesModule externalForces = ps.externalForces;
            externalForces.enabled = true;
            externalForces.AddInfluence(forceField);

            ps.Play();

        }

        // �¼�������

        // ��MailScreen��ȡ��ѽ���
        void OnRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // ���Խ�һ�ʯ���򲥷���Ч
            if (rewardType == ShopItemType.HealthPotion || rewardType == ShopItemType.LevelUpPotion)
                return;

            PlayPooledFX(screenPos, rewardType);
        }

        // ��ShopScreen������Ʒ
        void OnTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // ���Խ�һ�ʯ���򲥷���Ч
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