using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单位角色动画行为类
public class UnitCharacterAnimationBehaviour : MonoBehaviour
{
    // 动画控制器设置部分
    [Header("动画控制器")]
    // 角色动画控制器
    public Animator characterAnimator;

    // 被击中动画参数名称
    private string animGetHitParameter = "Get Hit";
    // 被击中动画参数ID
    private int animGetHitID;

    // 死亡动画参数名称
    private string animDieParameter = "Die";
    // 死亡动画参数ID
    private int animDieID;

    // 唤醒时调用，用于初始化
    void Awake()
    {
        // 设置动画参数ID
        SetupAnimationIDs();
    }

    // 设置动画参数ID的方法
    void SetupAnimationIDs()
    {
        // 获取被击中动画参数ID
        animGetHitID = Animator.StringToHash(animGetHitParameter);
        // 获取死亡动画参数ID
        animDieID = Animator.StringToHash(animDieParameter);
    }

    // 角色被击中方法
    public void CharacterWasHit()
    {
        // 触发被击中动画
        characterAnimator.SetTrigger(animGetHitID);
    }

    // 角色死亡方法
    public void CharacterHasDied()
    {
        // 触发死亡动画
        characterAnimator.SetTrigger(animDieID);
    }
}