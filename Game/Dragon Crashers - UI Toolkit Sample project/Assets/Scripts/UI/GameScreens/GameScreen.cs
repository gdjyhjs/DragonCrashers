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

        [Header("�˵���ĻԪ��")]
        [Tooltip("���ڲ�ѯ�Ӿ�Ԫ�ص��ַ���ID")]
        [SerializeField] string m_PauseScreenName = "PauseScreen";
        [SerializeField] string m_WinScreenName = "GameWinScreen";
        [SerializeField] string m_LoseScreenName = "GameLoseScreen";

        [Header("ģ��Ч��")]
        [SerializeField] Volume m_Volume;

        const float k_DelayWinScreen = 2f;

        // �ַ���ID
        // �Թ�����UIԪ�أ���ť����Ļ��������
        VisualElement m_PauseScreen; // ��ͣ��Ļ
        VisualElement m_WinScreen; // ʤ����Ļ
        VisualElement m_LoseScreen; // ʧ����Ļ
        VisualElement m_CharPortraitContainer; // ��ɫФ������

        Slider m_MusicSlider; // ���ֻ���
        Slider m_SfxSlider; // ��Ч����

        Button m_PauseButton; // ��ͣ��ť
        Button m_PauseResumeButton; // ��ͣ�ָ���ť
        Button m_PauseQuitButton; // ��ͣ�˳���ť
        Button m_PauseBackButton; // ��ͣ���ذ�ť

        Button m_WinNextButton; // ʤ����һ�ذ�ť
        Button m_LoseQuitButton; // ʧ���˳���ť
        Button m_LoseRetryButton; // ʧ�����԰�ť

        UIDocument m_GameScreen; // ��Ϸ��Ļ�ĵ�

        bool m_IsGameOver; // ��Ϸ�Ƿ����

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
            // ʹ��RegisterCallback���ð�ť
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

        // ����ɫФ����ӵ�������
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

        // ʹ��Volume�Ա�����Ϸ�������ģ������
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

        // ����CharacterCard�Ӿ�Ԫ��
        void HideCharacterCard(UnitController heroUnit)
        {
            // ����û��CharacterCard
            if (heroUnit.CharacterCard == null)
                return;

            if (m_CharPortraitContainer == null)
                return;

            // ���ý�ɫ��Ƭ
            VisualElement charCard = GetCharacterCard(heroUnit);
            charCard.style.display = DisplayStyle.None;
        }

        // ����ƥ���CharacterCard
        VisualElement GetCharacterCard(UnitController heroUnit)
        {
            // Ӣ�۵�λ��ģ���Ӿ����ʲ�
            TemplateContainer cardTemplate = heroUnit.CharacterCard.CharacterTemplate;

            // ���н�ɫФ��Ƭ
            List<VisualElement> cardElements = m_CharPortraitContainer.Children().ToList();

            // ����ƥ����
            foreach (VisualElement card in cardElements)
            {
                if (card == cardTemplate)
                {
                    return card;
                }
            }
            return null;
        }

        // ���⼼�ܵı߿���Ч
        void EnableFrameFX(VisualElement card, bool state)
        {
            if (card == null)
                return;

            VisualElement frameFx = card.Q<VisualElement>("game-char__fx-frame");
            ShowVisualElement(frameFx, state);
        }

        IEnumerator GameLostRoutine()
        {
            // �ȴ���Ȼ����ʾʧ����Ļ��ģ������
            yield return new WaitForSeconds(k_DelayWinScreen);

            // ����UI
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

            // ����UI
            m_CharPortraitContainer.style.display = DisplayStyle.None;
            m_PauseButton.style.display = DisplayStyle.None;

            AudioManager.PlayVictorySound();
            ShowVisualElement(m_WinScreen, true);
        }

        // ��������
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.MusicVolumeChanged?.Invoke(evt.newValue);
        }

        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.SfxVolumeChanged?.Invoke(evt.newValue);
        }

        // �¼�������
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