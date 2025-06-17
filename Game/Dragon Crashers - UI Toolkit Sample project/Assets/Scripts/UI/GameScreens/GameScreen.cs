using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Linq;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UIDocument))]
    public class GameScreen : MonoBehaviour
    {

        [Header("菜单屏幕元素")]
        [Tooltip("用于查询视觉元素的字符串ID")]
        [SerializeField] string m_PauseScreenName = "PauseScreen";
        [SerializeField] string m_WinScreenName = "GameWinScreen";
        [SerializeField] string m_LoseScreenName = "GameLoseScreen";

        [Header("模糊效果")]
        [SerializeField] Volume m_Volume;

        const float k_DelayWinScreen = 2f;

        // 字符串ID
        // 对功能性UI元素（按钮和屏幕）的引用
        VisualElement m_PauseScreen; // 暂停屏幕
        VisualElement m_WinScreen; // 胜利屏幕
        VisualElement m_LoseScreen; // 失败屏幕
        VisualElement m_CharPortraitContainer; // 角色肖像容器

        Slider m_MusicSlider; // 音乐滑块
        Slider m_SfxSlider; // 音效滑块

        Button m_PauseButton; // 暂停按钮
        Button m_PauseResumeButton; // 暂停恢复按钮
        Button m_PauseQuitButton; // 暂停退出按钮
        Button m_PauseBackButton; // 暂停返回按钮

        Button m_WinNextButton; // 胜利下一关按钮
        Button m_LoseQuitButton; // 失败退出按钮
        Button m_LoseRetryButton; // 失败重试按钮

        UIDocument m_GameScreen; // 游戏屏幕文档

        bool m_IsGameOver; // 游戏是否结束

        void OnEnable()
        {
            SetVisualElements();
            RegisterButtonCallbacks();

            if (m_Volume == null)
                m_Volume = FindFirstObjectByType<Volume>();

            GameplayEvents.WinScreenShown += OnGameWon;
            GameplayEvents.LoseScreenShown += OnGameLost;

            GameplayEvents.CharacterCardHidden += OnHideCharacterCard;
            GameplayEvents.SettingsUpdated += OnSettingsUpdated;

            UnitController.SpecialCharged += OnSpecialCharged;
            UnitController.SpecialDischarged += OnSpecialDischarged;
        }

        void OnDisable()
        {
            GameplayEvents.WinScreenShown -= OnGameWon;
            GameplayEvents.LoseScreenShown -= OnGameLost;

            GameplayEvents.CharacterCardHidden -= OnHideCharacterCard;
            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;

            UnitController.SpecialCharged -= OnSpecialCharged;
            UnitController.SpecialDischarged -= OnSpecialDischarged;
        }

        void SetVisualElements()
        {
            m_GameScreen = GetComponent<UIDocument>();
            VisualElement rootElement = m_GameScreen.rootVisualElement;

            m_PauseScreen = rootElement.Q(m_PauseScreenName);
            m_WinScreen = rootElement.Q(m_WinScreenName);
            m_LoseScreen = rootElement.Q(m_LoseScreenName);

            m_PauseButton = rootElement.Q<Button>("pause__button");
            m_PauseResumeButton = rootElement.Q<Button>("pause__resume-button");
            m_PauseQuitButton = rootElement.Q<Button>("pause__quit-button");
            m_PauseBackButton = rootElement.Q<Button>("pause__back-button");

            m_WinNextButton = rootElement.Q<Button>("game-win__next-button");
            m_LoseQuitButton = rootElement.Q<Button>("game-lose__quit-button");
            m_LoseRetryButton = rootElement.Q<Button>("game-lose__retry-button");
            m_CharPortraitContainer = rootElement.Q<VisualElement>("game-char__container");

            m_MusicSlider = rootElement.Q<Slider>("pause__music-slider");
            m_SfxSlider = rootElement.Q<Slider>("pause__sfx-slider");
        }

        void RegisterButtonCallbacks()
        {
            // 使用RegisterCallback设置按钮
            m_PauseButton.RegisterCallback<ClickEvent>(ShowPauseScreen);
            m_PauseResumeButton.RegisterCallback<ClickEvent>(ResumeGame);
            m_PauseBackButton.RegisterCallback<ClickEvent>(ResumeGame);
            m_PauseQuitButton.RegisterCallback<ClickEvent>(QuitGame);

            m_WinNextButton.RegisterCallback<ClickEvent>(QuitGame);
            m_LoseQuitButton.RegisterCallback<ClickEvent>(QuitGame);
            m_LoseRetryButton.RegisterCallback<ClickEvent>(RestartGame);

            m_MusicSlider.RegisterValueChangedCallback(ChangeMusicVolume);
            m_SfxSlider.RegisterValueChangedCallback(ChangeSfxVolume);
        }

        void Start()
        {
            BlurBackground(false);
        }

        void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // 将角色肖像添加到容器中
        public void AddHero(CharacterCard card)
        {
            if (m_CharPortraitContainer == null)
            {
                SetVisualElements();
            }

            m_CharPortraitContainer.Add(card.CharacterTemplate);
            card.CharacterTemplate.pickingMode = PickingMode.Ignore;
            EnableFrameFX(card.CharacterTemplate, false);
        }

        void ShowPauseScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            GameplayEvents.GamePaused?.Invoke(1f);

            ShowVisualElement(m_PauseScreen, true);
            ShowVisualElement(m_PauseButton, false);

            BlurBackground(true);

            m_CharPortraitContainer.style.display = DisplayStyle.None;
        }

        void RestartGame(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            GameplayEvents.GameRestarted?.Invoke();
        }
        void QuitGame(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            GameplayEvents.GameQuit?.Invoke();
        }

        void ResumeGame(ClickEvent evt)
        {
            GameplayEvents.GameResumed?.Invoke();
            AudioManager.PlayDefaultButtonSound();
            ShowVisualElement(m_PauseScreen, false);
            ShowVisualElement(m_PauseButton, true);
            BlurBackground(false);

            m_CharPortraitContainer.style.display = DisplayStyle.Flex;
        }

        // 使用Volume对背景游戏对象进行模糊处理
        void BlurBackground(bool state)
        {
            if (m_Volume == null)
                return;

            DepthOfField blurDOF;
            if (m_Volume.profile.TryGet<DepthOfField>(out blurDOF))
            {
                blurDOF.active = state;
            }
        }

        // 禁用CharacterCard视觉元素
        void HideCharacterCard(UnitController heroUnit)
        {
            // 敌人没有CharacterCard
            if (heroUnit.CharacterCard == null)
                return;

            if (m_CharPortraitContainer == null)
                return;

            // 禁用角色卡片
            VisualElement charCard = GetCharacterCard(heroUnit);
            charCard.style.display = DisplayStyle.None;
        }

        // 查找匹配的CharacterCard
        VisualElement GetCharacterCard(UnitController heroUnit)
        {
            // 英雄单位的模板视觉树资产
            TemplateContainer cardTemplate = heroUnit.CharacterCard.CharacterTemplate;

            // 所有角色肖像卡片
            List<VisualElement> cardElements = m_CharPortraitContainer.Children().ToList();

            // 返回匹配项
            foreach (VisualElement card in cardElements)
            {
                if (card == cardTemplate)
                {
                    return card;
                }
            }
            return null;
        }

        // 特殊技能的边框特效
        void EnableFrameFX(VisualElement card, bool state)
        {
            if (card == null)
                return;

            VisualElement frameFx = card.Q<VisualElement>("game-char__fx-frame");
            ShowVisualElement(frameFx, state);
        }

        IEnumerator GameLostRoutine()
        {
            // 等待，然后显示失败屏幕并模糊背景
            yield return new WaitForSeconds(k_DelayWinScreen);

            // 隐藏UI
            m_CharPortraitContainer.style.display = DisplayStyle.None;
            m_PauseButton.style.display = DisplayStyle.None;

            AudioManager.PlayDefeatSound();
            ShowVisualElement(m_LoseScreen, true);
            BlurBackground(true);
        }

        IEnumerator GameWonRoutine()
        {
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(k_DelayWinScreen);

            // 隐藏UI
            m_CharPortraitContainer.style.display = DisplayStyle.None;
            m_PauseButton.style.display = DisplayStyle.None;

            AudioManager.PlayVictorySound();
            ShowVisualElement(m_WinScreen, true);
        }

        // 音量设置
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.MusicVolumeChanged?.Invoke(evt.newValue);
        }

        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.SfxVolumeChanged?.Invoke(evt.newValue);
        }

        // 事件处理方法
        void OnGameWon()
        {
            if (m_IsGameOver)
                return;

            m_IsGameOver = true;
            StartCoroutine(GameWonRoutine());
        }

        void OnGameLost()
        {
            if (m_IsGameOver)
                return;

            m_IsGameOver = true;
            StartCoroutine(GameLostRoutine());
        }

        void OnHideCharacterCard(UnitController unit)
        {
            HideCharacterCard(unit);
        }

        void OnSpecialDischarged(UnitController unit)
        {
            VisualElement card = GetCharacterCard(unit);
            EnableFrameFX(card, false);
        }

        void OnSpecialCharged(UnitController unit)
        {
            VisualElement card = GetCharacterCard(unit);
            EnableFrameFX(card, true);
        }

        void OnSettingsUpdated(GameData gameData)
        {
            m_MusicSlider.value = gameData.MusicVolume;
            m_SfxSlider.value = gameData.SfxVolume;
        }
    }
}