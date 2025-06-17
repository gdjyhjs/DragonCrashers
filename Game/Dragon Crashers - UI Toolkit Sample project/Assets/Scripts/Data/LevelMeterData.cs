using Unity.Properties;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��ʾ���ڵȼ��������ĵȼ����ݣ��ü�����������ҵ��ܵȼ�
    /// ��ͨ�����ݰ��ṩ��Ӧ��������
    /// </summary>
    public class LevelMeterData
    {
        // �洢��ҵ��ܵȼ���
        int m_TotalLevels;

        /// <summary>
        /// �ۻ����ܵȼ������������������ݰ󶨡�
        /// </summary>
        [CreateProperty]
        public int TotalLevels { get => m_TotalLevels; set => m_TotalLevels = value; }

        /// <summary>
        /// ���캯����ʹ��ָ�����ܵȼ���ʼ��LevelMeterData�ࡣ
        /// </summary>
        public LevelMeterData(int totalLevels)
        {
            m_TotalLevels = Mathf.Clamp(totalLevels, 0, 100);
        }

        /// <summary>
        /// ����ҵ��ܵȼ�ӳ�䵽�����ַ�����
        /// </summary>
        /// <param name="level">��ҵ��ܵȼ���</param>
        public static string GetRankFromLevel(int level)
        {
            if (level >= 100) return "�ռ��ھ�";
            if (level >= 95) return "����ھ�";
            if (level >= 90) return "�ʼҹھ�";
            if (level >= 85) return "��Ӣ�ھ�";
            if (level >= 80) return "��ھ�";
            if (level >= 75) return "����ھ�";
            if (level >= 70) return "�ھ�";
            if (level >= 65) return "��ʦIV";
            if (level >= 60) return "��ʦIII";
            if (level >= 55) return "��ʦII";
            if (level >= 50) return "��ʦI";
            if (level >= 45) return "��ս��IV";
            if (level >= 40) return "��ս��III";
            if (level >= 35) return "��ս��II";
            if (level >= 30) return "��ս��I";
            if (level >= 27) return "��ϰ��III";
            if (level >= 23) return "��ϰ��II";
            if (level >= 20) return "��ϰ��I";
            if (level >= 15) return "̽����III";
            if (level >= 12) return "̽����II";
            if (level >= 10) return "̽����I";
            if (level >= 8) return "ѧͽII";
            if (level >= 6) return "ѧͽI";
            if (level >= 4) return "����III";
            if (level >= 3) return "����II";
            if (level >= 1) return "����I";
            return "δ����";
        }
    }


}