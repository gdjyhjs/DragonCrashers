using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
namespace HappyHarvest
{
    /// <summary>
    /// ���߶���������ȷ֡���ò�ͬ�¼��Դ������õ��Ӿ�Ч��
    /// �˽ű���Ҫ��ӵ����ߵ� Animator ���ڵ�ͬһ GameObject �ϣ��Ա���ն����¼���
    /// </summary>
    public class ToolAnimationEventHandler : MonoBehaviour
    {
        [Header("ǰ��")]
        // ǰ���Ӿ�Ч��
        public VisualEffect FrontEffect;
        // ǰ��Ч���¼� ID
        public string FrontEffectId;

        [Header("�Ϸ�")]
        // �Ϸ��Ӿ�Ч��
        public VisualEffect UpEffect;
        // �Ϸ�Ч���¼� ID
        public string UpEffectId;

        [Header("����")]
        // �����Ӿ�Ч��
        public VisualEffect SideEffect;
        // ����Ч���¼� ID
        public string SideEffectId;

        /// <summary>
        /// ����ǰ���Ӿ�Ч��
        /// </summary>
        public void TriggerFrontVFX()
        {
            // ����ǰ��Ч����������������Ч��
            SideEffect.gameObject.SetActive(false);
            UpEffect.gameObject.SetActive(false);
            FrontEffect.gameObject.SetActive(true);

            // ����Ч�������¼�
            FrontEffect.SendEvent(FrontEffectId);
        }

        /// <summary>
        /// ���������Ӿ�Ч��
        /// </summary>
        public void TriggerSideVFX()
        {
            // �������Ч����������������Ч��
            SideEffect.gameObject.SetActive(true);
            UpEffect.gameObject.SetActive(false);
            FrontEffect.gameObject.SetActive(false);

            // ����Ч�������¼�
            SideEffect.SendEvent(SideEffectId);
        }

        /// <summary>
        /// �����Ϸ��Ӿ�Ч��
        /// </summary>
        public void TriggerUpVFX()
        {
            // �����Ϸ�Ч����������������Ч��
            SideEffect.gameObject.SetActive(false);
            UpEffect.gameObject.SetActive(true);
            FrontEffect.gameObject.SetActive(false);

            // ����Ч�������¼�
            UpEffect.SendEvent(UpEffectId);
        }
    }
}