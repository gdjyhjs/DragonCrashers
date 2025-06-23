using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace HappyHarvest
{
    /// <summary>
    /// 主菜单处理器，负责主菜单的交互逻辑和场景切换
    /// </summary>
    public class MainMenuHandler : MonoBehaviour
    {
        private UIDocument m_Document; // UI 文档组件
        private Button m_StartButton; // 开始游戏按钮

        private VisualElement m_Blocker; // 黑屏遮罩元素

        private void Start()
        {
            // 获取 UI 文档组件
            m_Document = GetComponent<UIDocument>();
            // 获取开始游戏按钮
            m_StartButton = m_Document.rootVisualElement.Q<Button>("StartButton");

            // 为开始按钮添加点击事件：显示黑屏遮罩
            m_StartButton.clicked += () => { m_Blocker.style.opacity = 1.0f; };

            // 获取黑屏遮罩元素
            m_Blocker = m_Document.rootVisualElement.Q<VisualElement>("Blocker");
            // 注册遮罩过渡结束事件：加载游戏场景
            m_Blocker.RegisterCallback<TransitionEndEvent>(evt =>
            {
                SceneManager.LoadScene("House_Brith", LoadSceneMode.Single);
            });
        }
    }
}