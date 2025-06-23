using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace HappyHarvest
{
    /// <summary>
    /// ���˵����������������˵��Ľ����߼��ͳ����л�
    /// </summary>
    public class MainMenuHandler : MonoBehaviour
    {
        private UIDocument m_Document; // UI �ĵ����
        private Button m_StartButton; // ��ʼ��Ϸ��ť

        private VisualElement m_Blocker; // ��������Ԫ��

        private void Start()
        {
            // ��ȡ UI �ĵ����
            m_Document = GetComponent<UIDocument>();
            // ��ȡ��ʼ��Ϸ��ť
            m_StartButton = m_Document.rootVisualElement.Q<Button>("StartButton");

            // Ϊ��ʼ��ť��ӵ���¼�����ʾ��������
            m_StartButton.clicked += () => { m_Blocker.style.opacity = 1.0f; };

            // ��ȡ��������Ԫ��
            m_Blocker = m_Document.rootVisualElement.Q<VisualElement>("Blocker");
            // ע�����ֹ��ɽ����¼���������Ϸ����
            m_Blocker.RegisterCallback<TransitionEndEvent>(evt =>
            {
                SceneManager.LoadScene("House_Brith", LoadSceneMode.Single);
            });
        }
    }
}