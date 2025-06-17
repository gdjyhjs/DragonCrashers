using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    // 一个非常基础的用于播放声音的组件；使用静态方法可以从任何地方播放音效

    public class AudioManager : MonoBehaviour
    {
        // 音频混合器组名称
        public static string MusicGroup = "Music";
        public static string SfxGroup = "SFX";

        // 参数后缀
        const string k_Parameter = "Volume";
        private static float s_LastSFXPlayTime = -1f;
        // 全局音效播放冷却时间
        private static float sfxCooldown = 0.1f;

        [SerializeField] AudioMixer m_MainAudioMixer;

        // 基本的 UI 音效剪辑
        [Header("UI 音效")]
        [Tooltip("通用按钮点击音效。")]
        [SerializeField] AudioClip m_DefaultButtonSound;
        [Tooltip("通用按钮点击音效。")]
        [SerializeField] AudioClip m_AltButtonSound;
        [Tooltip("通用商店购买音效。")]
        [SerializeField] AudioClip m_TransactionSound;
        [Tooltip("通用错误音效。")]
        [SerializeField] AudioClip m_DefaultWarningSound;

        [Header("游戏音效")]
        [Tooltip("升级或关卡胜利音效。")]
        [SerializeField] AudioClip m_VictorySound;
        [Tooltip("关卡失败音效。")]
        [SerializeField] AudioClip m_DefeatSound;
        [SerializeField] AudioClip m_PotionSound;

        // 当脚本启用时，注册事件监听器
        void OnEnable()
        {
            SettingsEvents.SettingsUpdated += OnSettingsUpdated;

            GameplayEvents.SettingsUpdated += OnSettingsUpdated;
        }

        // 当脚本禁用时，移除事件监听器
        void OnDisable()
        {
            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;

            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;
        }

        // 播放一次性音效
        public static void PlayOneSFX(AudioClip clip, Vector3 sfxPosition)
        {
            if (clip == null)
                return;

            // 检查全局冷却时间是否已过
            if (Time.time - s_LastSFXPlayTime < sfxCooldown)
            {
                return; // 如果在冷却时间内，则不播放音效
            }

            // 更新上次播放时间
            s_LastSFXPlayTime = Time.time;

            GameObject sfxInstance = new GameObject(clip.name);
            sfxInstance.transform.position = sfxPosition;

            AudioSource source = sfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();

            // 设置音频混合器组（例如音乐、音效等）
            source.outputAudioMixerGroup = GetAudioMixerGroup(SfxGroup);

            // 在音效播放完成后销毁实例
            Destroy(sfxInstance, clip.length);
        }

        // 根据名称返回音频混合器组
        public static AudioMixerGroup GetAudioMixerGroup(string groupName)
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();

            if (audioManager == null)
                return null;

            if (audioManager.m_MainAudioMixer == null)
                return null;

            AudioMixerGroup[] groups = audioManager.m_MainAudioMixer.FindMatchingGroups(groupName);

            foreach (AudioMixerGroup match in groups)
            {
                if (match.ToString() == groupName)
                    return match;
            }
            return null;

        }

        // 将 0 到 1 之间的线性值转换为分贝值
        public static float GetDecibelValue(float linearValue)
        {
            // 常用的线性值到分贝值的转换因子
            float conversionFactor = 20f;

            float decibelValue = (linearValue != 0) ? conversionFactor * Mathf.Log10(linearValue) : -144f;
            return decibelValue;
        }

        // 将分贝值转换为 0 到 1 之间的范围
        public static float GetLinearValue(float decibelValue)
        {
            float conversionFactor = 20f;

            return Mathf.Pow(10f, decibelValue / conversionFactor);

        }

        // 将 0 到 1 之间的线性值转换为分贝值并设置音频混合器的音量
        public static void SetVolume(string groupName, float linearValue)
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            float decibelValue = GetDecibelValue(linearValue);

            if (audioManager.m_MainAudioMixer != null)
            {
                audioManager.m_MainAudioMixer.SetFloat(groupName, decibelValue);
            }
        }

        // 根据音频混合器的分贝值返回 0 到 1 之间的值
        public static float GetVolume(string groupName)
        {

            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return 0f;

            float decibelValue = 0f;
            if (audioManager.m_MainAudioMixer != null)
            {
                audioManager.m_MainAudioMixer.GetFloat(groupName, out decibelValue);
            }
            return GetLinearValue(decibelValue);
        }

        // 方便的方法，用于播放一系列预定义的音效
        // 播放默认按钮点击音效
        public static void PlayDefaultButtonSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefaultButtonSound, Vector3.zero);
        }

        // 播放替代按钮点击音效
        public static void PlayAltButtonSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_AltButtonSound, Vector3.zero);
        }

        // 播放默认交易音效
        public static void PlayDefaultTransactionSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_TransactionSound, Vector3.zero);
        }

        // 播放默认警告音效
        public static void PlayDefaultWarningSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefaultWarningSound, Vector3.zero);
        }

        // 播放胜利音效
        public static void PlayVictorySound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_VictorySound, Vector3.zero);
        }

        // 播放失败音效
        public static void PlayDefeatSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefeatSound, Vector3.zero);
        }

        // 播放药水掉落音效
        public static void PlayPotionDropSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_PotionSound, Vector3.zero);
        }

        // 事件处理方法
        // 当设置更新时调用，更新音乐和音效音量
        void OnSettingsUpdated(GameData gameData)
        {
            // 使用游戏数据设置音乐和音效音量
            SetVolume(MusicGroup + k_Parameter, gameData.MusicVolume / 100f);
            SetVolume(SfxGroup + k_Parameter, gameData.SfxVolume / 100f);
        }
    }
}