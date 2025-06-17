using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    // һ���ǳ����������ڲ��������������ʹ�þ�̬�������Դ��κεط�������Ч

    public class AudioManager : MonoBehaviour
    {
        // ��Ƶ�����������
        public static string MusicGroup = "Music";
        public static string SfxGroup = "SFX";

        // ������׺
        const string k_Parameter = "Volume";
        private static float s_LastSFXPlayTime = -1f;
        // ȫ����Ч������ȴʱ��
        private static float sfxCooldown = 0.1f;

        [SerializeField] AudioMixer m_MainAudioMixer;

        // ������ UI ��Ч����
        [Header("UI ��Ч")]
        [Tooltip("ͨ�ð�ť�����Ч��")]
        [SerializeField] AudioClip m_DefaultButtonSound;
        [Tooltip("ͨ�ð�ť�����Ч��")]
        [SerializeField] AudioClip m_AltButtonSound;
        [Tooltip("ͨ���̵깺����Ч��")]
        [SerializeField] AudioClip m_TransactionSound;
        [Tooltip("ͨ�ô�����Ч��")]
        [SerializeField] AudioClip m_DefaultWarningSound;

        [Header("��Ϸ��Ч")]
        [Tooltip("������ؿ�ʤ����Ч��")]
        [SerializeField] AudioClip m_VictorySound;
        [Tooltip("�ؿ�ʧ����Ч��")]
        [SerializeField] AudioClip m_DefeatSound;
        [SerializeField] AudioClip m_PotionSound;

        // ���ű�����ʱ��ע���¼�������
        void OnEnable()
        {
            SettingsEvents.SettingsUpdated += OnSettingsUpdated;

            GameplayEvents.SettingsUpdated += OnSettingsUpdated;
        }

        // ���ű�����ʱ���Ƴ��¼�������
        void OnDisable()
        {
            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;

            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;
        }

        // ����һ������Ч
        public static void PlayOneSFX(AudioClip clip, Vector3 sfxPosition)
        {
            if (clip == null)
                return;

            // ���ȫ����ȴʱ���Ƿ��ѹ�
            if (Time.time - s_LastSFXPlayTime < sfxCooldown)
            {
                return; // �������ȴʱ���ڣ��򲻲�����Ч
            }

            // �����ϴβ���ʱ��
            s_LastSFXPlayTime = Time.time;

            GameObject sfxInstance = new GameObject(clip.name);
            sfxInstance.transform.position = sfxPosition;

            AudioSource source = sfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();

            // ������Ƶ������飨�������֡���Ч�ȣ�
            source.outputAudioMixerGroup = GetAudioMixerGroup(SfxGroup);

            // ����Ч������ɺ�����ʵ��
            Destroy(sfxInstance, clip.length);
        }

        // �������Ʒ�����Ƶ�������
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

        // �� 0 �� 1 ֮�������ֵת��Ϊ�ֱ�ֵ
        public static float GetDecibelValue(float linearValue)
        {
            // ���õ�����ֵ���ֱ�ֵ��ת������
            float conversionFactor = 20f;

            float decibelValue = (linearValue != 0) ? conversionFactor * Mathf.Log10(linearValue) : -144f;
            return decibelValue;
        }

        // ���ֱ�ֵת��Ϊ 0 �� 1 ֮��ķ�Χ
        public static float GetLinearValue(float decibelValue)
        {
            float conversionFactor = 20f;

            return Mathf.Pow(10f, decibelValue / conversionFactor);

        }

        // �� 0 �� 1 ֮�������ֵת��Ϊ�ֱ�ֵ��������Ƶ�����������
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

        // ������Ƶ������ķֱ�ֵ���� 0 �� 1 ֮���ֵ
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

        // ����ķ��������ڲ���һϵ��Ԥ�������Ч
        // ����Ĭ�ϰ�ť�����Ч
        public static void PlayDefaultButtonSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefaultButtonSound, Vector3.zero);
        }

        // ���������ť�����Ч
        public static void PlayAltButtonSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_AltButtonSound, Vector3.zero);
        }

        // ����Ĭ�Ͻ�����Ч
        public static void PlayDefaultTransactionSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_TransactionSound, Vector3.zero);
        }

        // ����Ĭ�Ͼ�����Ч
        public static void PlayDefaultWarningSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefaultWarningSound, Vector3.zero);
        }

        // ����ʤ����Ч
        public static void PlayVictorySound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_VictorySound, Vector3.zero);
        }

        // ����ʧ����Ч
        public static void PlayDefeatSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_DefeatSound, Vector3.zero);
        }

        // ����ҩˮ������Ч
        public static void PlayPotionDropSound()
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
                return;

            PlayOneSFX(audioManager.m_PotionSound, Vector3.zero);
        }

        // �¼�������
        // �����ø���ʱ���ã��������ֺ���Ч����
        void OnSettingsUpdated(GameData gameData)
        {
            // ʹ����Ϸ�����������ֺ���Ч����
            SetVolume(MusicGroup + k_Parameter, gameData.MusicVolume / 100f);
            SetVolume(SfxGroup + k_Parameter, gameData.SfxVolume / 100f);
        }
    }
}