using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIToolkitDemo;

// 单位音频行为类
public class UnitAudioBehaviour : MonoBehaviour
{

    // 组件引用部分
    [Header("组件引用")]
    // 音频源组件
    public AudioSource audioSource;

    // 音效音量覆盖部分
    [Header("音效音量覆盖")]
    // 死亡音效音量
    public float sfxDeathVolume;

    // 设置音频源音量
    void SetAudioSourceVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    // 播放音频剪辑
    void PlayAudioClip(AudioClip selectedAudioClip)
    {
        //audioSource.PlayOneShot(selectedAudioClip);
        AudioManager.PlayOneSFX(selectedAudioClip, Vector3.zero);
    }
}