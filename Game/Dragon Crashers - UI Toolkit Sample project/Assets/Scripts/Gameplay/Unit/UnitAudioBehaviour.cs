using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIToolkitDemo;

// ��λ��Ƶ��Ϊ��
public class UnitAudioBehaviour : MonoBehaviour
{

    // ������ò���
    [Header("�������")]
    // ��ƵԴ���
    public AudioSource audioSource;

    // ��Ч�������ǲ���
    [Header("��Ч��������")]
    // ������Ч����
    public float sfxDeathVolume;

    // ������ƵԴ����
    void SetAudioSourceVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    // ������Ƶ����
    void PlayAudioClip(AudioClip selectedAudioClip)
    {
        //audioSource.PlayOneShot(selectedAudioClip);
        AudioManager.PlayOneSFX(selectedAudioClip, Vector3.zero);
    }
}