using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 创建一个可在Unity编辑器中创建的脚本化对象，用于存储单位音效数据
[CreateAssetMenu(fileName = "Data_SFX_", menuName = "Dragon Crashers/Unit/SFX Data", order = 1)]
public class UnitSFXData : ScriptableObject
{
    [Header("战斗音效")]
    public AudioClip[] audioClipsGetHit; // 被击中音效数组
    public AudioClip[] audioClipsDeath; // 死亡音效数组

    // 获取随机的被击中音效
    public AudioClip GetGetHitClip()
    {
        return SelectRandomAudioClip(audioClipsGetHit);
    }

    // 获取随机的死亡音效
    public AudioClip GetDeathClip()
    {
        return SelectRandomAudioClip(audioClipsDeath);
    }

    // 从音频剪辑数组中选择一个随机的音频剪辑
    AudioClip SelectRandomAudioClip(AudioClip[] audioClipArray)
    {
        if (audioClipArray.Length <= 0)
        {
            return null;
        }

        int randomClipInt = Random.Range(0, audioClipArray.Length);
        return audioClipArray[randomClipInt];
    }
}