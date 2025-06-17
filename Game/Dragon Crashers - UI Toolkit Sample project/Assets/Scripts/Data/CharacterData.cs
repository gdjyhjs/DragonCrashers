using System;
using Unity.Properties;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // �洢��ɫʵ�������� + ����ScriptableObject�ľ�̬����

    /// <summary>
    /// �����ɫ�Ķ�̬���ݣ�����װ����װ���͵�ǰ�ȼ���
    /// ʹ������ʱ���ݰ�ϵͳ֪ͨUIԪ�����Եĸ��ġ�
    /// ������������ľ���ֵ����һ�������ҩ�������͵�ǰ��ʵ����
    /// </summary>
    public class CharacterData : MonoBehaviour, INotifyBindablePropertyChanged
    {
        // ���ŵȼ�����������ֵ�����������ٶ�
        const float k_ProgressionFactor = 10f;

        /// <summary>
        /// ��ȡ��ɫװ����װ����Ʒ���顣
        /// </summary>
        [SerializeField] EquipmentSO[] m_GearData = new EquipmentSO[4];

        /// <summary>
        /// ��ScriptableObject��ȡ��ɫ�ľ�̬���ݡ�
        /// </summary>
        [SerializeField] CharacterBaseSO m_CharacterBaseData;

        [SerializeField] int m_CurrentLevel;

        /// <summary>
        /// ��ȡ�����ý�ɫ�ĵ�ǰ�ȼ������ڸ���ʱ֪ͨ��ϵͳ��
        /// </summary>
        [CreateProperty]
        public int CurrentLevel
        {
            get => m_CurrentLevel;
            set
            {
                if (m_CurrentLevel == value)
                    return;

                m_CurrentLevel = value;

                Notify();

                // ��CurrentLevel����ʱ��֪ͨCurrentPower��PotionsForNextLevelҲ��Ҫ���¡�
                Notify(nameof(CurrentPower));
                Notify(nameof(PotionsForNextLevel));
            }
        }

        /// <summary>
        /// �����ɫ�ĵ�ǰʵ���ȼ������ڵȼ��ͻ������ԡ�
        /// ��CurrentLevel����ʱ���¡�
        /// </summary>
        [CreateProperty]
        public int CurrentPower
        {
            get
            {
                float basePoints = m_CharacterBaseData.TotalBasePoints;

                // �������ѡ������߼��԰�����ɫ��װ��
                float equipmentPoints = 0f;

                return (int)(CurrentLevel * basePoints + equipmentPoints) / 10;
            }
        }

        /// <summary>
        /// ��CharacterBaseData��ȡ��ɫ���ơ�
        /// </summary>
        [CreateProperty]
        public string CharacterName => m_CharacterBaseData.CharacterName;

        /// <summary>
        /// ������һ�������ҩ�����������ظ�ʽ�����ַ�����
        /// </summary>
        [CreateProperty]
        public string PotionsForNextLevel => "/" + GetPotionsForNextLevel(CurrentLevel, k_ProgressionFactor);

        /// <summary>
        /// ���ɰ����Ը���ʱ�������¼���ʵ��INotifyBindablePropertyChanged���裩
        /// </summary>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        /// <summary>
        /// ֪ͨ������ĳ�������Ѹ��ġ� 
        /// </summary>
        /// <param name="property">�Ѹ��ĵ���������</param>
        void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        /// <summary>
        /// ��ȡ�����ý�ɫ�Ӿ���ʾ��Ԥ��ʵ����
        /// </summary>
        public GameObject PreviewInstance { get; set; }

        /// <summary>
        /// ��ScriptableObject��ȡ��ɫ�ľ�̬���ݡ�
        /// </summary>
        public CharacterBaseSO CharacterBaseData => m_CharacterBaseData;

        /// <summary>
        /// ��ȡ��ɫװ����װ����Ʒ���顣
        /// </summary>
        public EquipmentSO[] GearData => m_GearData;

        /// <summary>
        /// ����������һ�����������ҩ�����������޷���������ʾ��
        /// </summary>
        /// <returns>������һ������ľ���ֵ������</returns>
        public uint GetPotionsForNextLevel()
        {
            return (uint)GetPotionsForNextLevel(m_CurrentLevel, k_ProgressionFactor);
        }

        /// <summary>
        /// �����ɫ���������ҩ��������
        /// </summary>
        /// <param name="currentLevel">��ɫ�ĵ�ǰ�ȼ���</param>
        /// <param name="progressionFactor">���ڼ���Ľ������ӡ�</param>
        /// <returns>������һ�������ҩ��������</returns>
        int GetPotionsForNextLevel(int currentLevel, float progressionFactor)
        {
            currentLevel = Mathf.Clamp(currentLevel, 1, currentLevel);
            progressionFactor = Mathf.Clamp(progressionFactor, 1f, progressionFactor);

            float xp = (progressionFactor * (currentLevel));
            xp = Mathf.Ceil((float)xp);
            return (int)xp;
        }

        /// <summary>
        /// ����ɫ�ĵȼ�����1����
        /// </summary>
        public void IncrementLevel()
        {
            CurrentLevel++;

            // ֪ͨ����ϵͳ����CharScreenController�ȣ��ý�ɫ������
            CharEvents.LevelIncremented?.Invoke(this);
        }
    }
}