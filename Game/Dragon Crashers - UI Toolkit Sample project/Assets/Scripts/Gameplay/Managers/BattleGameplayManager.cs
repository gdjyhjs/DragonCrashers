using System.Collections;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using Utilities.Inspector;
using System;

// 战斗模式枚举
public enum CutsceneMode
{
    Play, // 播放过场动画
    None // 无过场动画
}

// 战斗游戏玩法管理器
public class BattleGameplayManager : MonoBehaviour
{
    public static event Action GameWon; // 游戏胜利事件
    public static event Action GameLost; // 游戏失败事件

    [Header("队伍")] // 队伍相关设置
    public List<UnitController> heroTeamUnits; // 英雄队伍单位列表
    public List<UnitController> enemyTeamUnits; // 敌人队伍单位列表

    [Header("队伍逻辑")] // 队伍逻辑相关设置
    public bool autoAssignUnitTeamTargets = false; // 是否自动分配队伍目标

    // 运行时战斗逻辑
    private List<UnitController> aliveHeroUnits; // 存活的英雄单位列表
    private List<UnitController> aliveEnemyUnits; // 存活的敌人单位列表

    [Header("战斗开场")] // 战斗开场相关设置
    public CutsceneMode introCutscene; // 开场过场动画模式
    public CutsceneTimelineBehaviour introCutsceneBehaviour; // 开场过场动画时间轴行为

    [Header("战斗结束 - 胜利")] // 战斗胜利相关设置
    public CutsceneTimelineBehaviour victoryCutsceneBehaviour; // 胜利过场动画时间轴行为
    public SceneField victoryNextScene; // 胜利后要加载的下一个场景

    [Header("战斗结束 - 失败")] // 战斗失败相关设置
    public CutsceneTimelineBehaviour defeatCutsceneBehaviour; // 失败过场动画时间轴行为
    public SceneField defeatNextScene; // 失败后要加载的下一个场景

    private SceneField selectedNextScene; // 选中的下一个场景


    void Awake()
    {
        // 设置队伍单位
        SetupTeamUnits();
        // 开始游戏逻辑
        StartGameLogic();
    }

    // 设置队伍单位
    void SetupTeamUnits()
    {
        // 创建存活单位列表
        CreateAliveUnits();

        // 如果设置了自动分配目标，则自动分配队伍目标
        if (autoAssignUnitTeamTargets)
        {
            AutoAssignUnitTeamTargets();
        }
    }

    // 创建存活单位列表
    void CreateAliveUnits()
    {
        aliveHeroUnits = new List<UnitController>();

        for (int i = 0; i < heroTeamUnits.Count; i++)
        {
            aliveHeroUnits.Add(heroTeamUnits[i]);
            aliveHeroUnits[i].SetAlive();
            aliveHeroUnits[i].UnitDiedEvent += UnitHasDied;
        }

        aliveEnemyUnits = new List<UnitController>();

        for (int i = 0; i < enemyTeamUnits.Count; i++)
        {
            aliveEnemyUnits.Add(enemyTeamUnits[i]);
            aliveEnemyUnits[i].SetAlive();
            aliveEnemyUnits[i].UnitDiedEvent += UnitHasDied;
        }
    }

    // 自动分配队伍目标
    void AutoAssignUnitTeamTargets()
    {
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            aliveHeroUnits[i].AssignTargetUnits(aliveEnemyUnits);
        }

        for (int i = 0; i < aliveEnemyUnits.Count; i++)
        {
            aliveEnemyUnits[i].AssignTargetUnits(aliveHeroUnits);
        }
    }

    // 开始游戏逻辑
    void StartGameLogic()
    {
        switch (introCutscene)
        {
            case CutsceneMode.Play:
                // 开始开场过场动画
                StartIntroCutscene();
                break;

            case CutsceneMode.None:
                // 开始战斗
                StartBattle();
                break;
        }
    }

    // 开始开场过场动画
    void StartIntroCutscene()
    {
        introCutsceneBehaviour.StartTimeline();
    }

    // 开始战斗
    public void StartBattle()
    {
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            aliveHeroUnits[i].BattleStarted();
        }

        for (int i = 0; i < aliveEnemyUnits.Count; i++)
        {
            aliveEnemyUnits[i].BattleStarted();
        }
    }

    // 单位死亡
    void UnitHasDied(UnitController deadUnit)
    {
        // 从存活单位列表中移除死亡单位
        RemoveUnitFromAliveUnits(deadUnit);
    }

    // 从存活单位列表中移除单位
    void RemoveUnitFromAliveUnits(UnitController unit)
    {
        // 检查剩余队伍
        CheckRemainingTeams();
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            if (unit == aliveHeroUnits[i])
            {
                aliveHeroUnits[i].UnitDiedEvent -= UnitHasDied;
                aliveHeroUnits.RemoveAt(i);
                // 从敌人队伍目标中移除该单位
                RemoveUnitFromEnemyTeamTargets(unit);
            }
        }

        if (aliveHeroUnits.Count > 0)
        {
            for (int i = 0; i < aliveEnemyUnits.Count; i++)
            {
                if (unit == aliveEnemyUnits[i])
                {
                    aliveEnemyUnits[i].UnitDiedEvent -= UnitHasDied;
                    aliveEnemyUnits.RemoveAt(i);
                    // 从英雄队伍目标中移除该单位
                    RemoveUnitFromHeroTeamTargets(unit);
                }
            }
        }

        // 再次检查剩余队伍
        CheckRemainingTeams();
    }

    // 从英雄队伍目标中移除单位
    void RemoveUnitFromHeroTeamTargets(UnitController unit)
    {
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            aliveHeroUnits[i].RemoveTargetUnit(unit);
        }
    }

    // 从敌人队伍目标中移除单位
    void RemoveUnitFromEnemyTeamTargets(UnitController unit)
    {
        for (int i = 0; i < aliveEnemyUnits.Count; i++)
        {
            aliveEnemyUnits[i].RemoveTargetUnit(unit);
        }
    }

    // 检查剩余队伍
    void CheckRemainingTeams()
    {
        // 如果英雄队伍没有存活单位，则设置战斗失败
        if (aliveHeroUnits.Count == 0)
        {
            SetBattleDefeat();
            //Debug.Log("失败。存活英雄数量 = " + aliveHeroUnits.Count + " 存活敌人数量 = " + aliveEnemyUnits.Count);
        }
        // 如果敌人队伍没有存活单位，则设置战斗胜利
        else if (aliveEnemyUnits.Count == 0)
        {
            SetBattleVictory();
            //Debug.Log("胜利。存活英雄数量 = " + aliveHeroUnits.Count + " 存活敌人数量 = " + aliveEnemyUnits.Count);
        }
    }

    // 设置战斗胜利
    void SetBattleVictory()
    {
        // 停止所有存活的英雄单位
        StopAllAliveTeamUnits(aliveHeroUnits);

        if (victoryCutsceneBehaviour != null)
        {
            // 开始胜利过场动画
            victoryCutsceneBehaviour.StartTimeline();
        }

        // 通知游戏屏幕显示胜利UI
        GameWon?.Invoke();
    }

    // 设置战斗失败
    void SetBattleDefeat()
    {
        // 停止所有存活的敌人单位
        StopAllAliveTeamUnits(aliveEnemyUnits);

        if (defeatCutsceneBehaviour != null)
        {
            // 开始失败过场动画
            defeatCutsceneBehaviour.StartTimeline();
        }

        // 通知游戏屏幕显示失败UI
        GameLost?.Invoke();
    }

    // 停止所有存活的队伍单位
    void StopAllAliveTeamUnits(List<UnitController> aliveTeamUnits)
    {
        for (int i = 0; i < aliveTeamUnits.Count; i++)
        {
            aliveTeamUnits[i].BattleEnded();
        }
    }

    // 选择胜利后的下一个场景
    public void SelectVictoryNextScene()
    {
        selectedNextScene = victoryNextScene;
    }

    // 选择失败后的下一个场景
    public void SelectDefeatNextScene()
    {
        selectedNextScene = defeatNextScene;
    }

    // 加载选中的场景
    public void LoadSelectedScene()
    {
        NextSceneLoader sceneLoader = new NextSceneLoader();
        sceneLoader.LoadNextScene(selectedNextScene);
    }

}