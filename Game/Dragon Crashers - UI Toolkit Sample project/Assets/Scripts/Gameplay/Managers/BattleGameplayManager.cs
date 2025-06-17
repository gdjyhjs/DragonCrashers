using System.Collections;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using Utilities.Inspector;
using System;

// ս��ģʽö��
public enum CutsceneMode
{
    Play, // ���Ź�������
    None // �޹�������
}

// ս����Ϸ�淨������
public class BattleGameplayManager : MonoBehaviour
{
    public static event Action GameWon; // ��Ϸʤ���¼�
    public static event Action GameLost; // ��Ϸʧ���¼�

    [Header("����")] // �����������
    public List<UnitController> heroTeamUnits; // Ӣ�۶��鵥λ�б�
    public List<UnitController> enemyTeamUnits; // ���˶��鵥λ�б�

    [Header("�����߼�")] // �����߼��������
    public bool autoAssignUnitTeamTargets = false; // �Ƿ��Զ��������Ŀ��

    // ����ʱս���߼�
    private List<UnitController> aliveHeroUnits; // ����Ӣ�۵�λ�б�
    private List<UnitController> aliveEnemyUnits; // ���ĵ��˵�λ�б�

    [Header("ս������")] // ս�������������
    public CutsceneMode introCutscene; // ������������ģʽ
    public CutsceneTimelineBehaviour introCutsceneBehaviour; // ������������ʱ������Ϊ

    [Header("ս������ - ʤ��")] // ս��ʤ���������
    public CutsceneTimelineBehaviour victoryCutsceneBehaviour; // ʤ����������ʱ������Ϊ
    public SceneField victoryNextScene; // ʤ����Ҫ���ص���һ������

    [Header("ս������ - ʧ��")] // ս��ʧ���������
    public CutsceneTimelineBehaviour defeatCutsceneBehaviour; // ʧ�ܹ�������ʱ������Ϊ
    public SceneField defeatNextScene; // ʧ�ܺ�Ҫ���ص���һ������

    private SceneField selectedNextScene; // ѡ�е���һ������


    void Awake()
    {
        // ���ö��鵥λ
        SetupTeamUnits();
        // ��ʼ��Ϸ�߼�
        StartGameLogic();
    }

    // ���ö��鵥λ
    void SetupTeamUnits()
    {
        // ������λ�б�
        CreateAliveUnits();

        // ����������Զ�����Ŀ�꣬���Զ��������Ŀ��
        if (autoAssignUnitTeamTargets)
        {
            AutoAssignUnitTeamTargets();
        }
    }

    // ������λ�б�
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

    // �Զ��������Ŀ��
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

    // ��ʼ��Ϸ�߼�
    void StartGameLogic()
    {
        switch (introCutscene)
        {
            case CutsceneMode.Play:
                // ��ʼ������������
                StartIntroCutscene();
                break;

            case CutsceneMode.None:
                // ��ʼս��
                StartBattle();
                break;
        }
    }

    // ��ʼ������������
    void StartIntroCutscene()
    {
        introCutsceneBehaviour.StartTimeline();
    }

    // ��ʼս��
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

    // ��λ����
    void UnitHasDied(UnitController deadUnit)
    {
        // �Ӵ�λ�б����Ƴ�������λ
        RemoveUnitFromAliveUnits(deadUnit);
    }

    // �Ӵ�λ�б����Ƴ���λ
    void RemoveUnitFromAliveUnits(UnitController unit)
    {
        // ���ʣ�����
        CheckRemainingTeams();
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            if (unit == aliveHeroUnits[i])
            {
                aliveHeroUnits[i].UnitDiedEvent -= UnitHasDied;
                aliveHeroUnits.RemoveAt(i);
                // �ӵ��˶���Ŀ�����Ƴ��õ�λ
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
                    // ��Ӣ�۶���Ŀ�����Ƴ��õ�λ
                    RemoveUnitFromHeroTeamTargets(unit);
                }
            }
        }

        // �ٴμ��ʣ�����
        CheckRemainingTeams();
    }

    // ��Ӣ�۶���Ŀ�����Ƴ���λ
    void RemoveUnitFromHeroTeamTargets(UnitController unit)
    {
        for (int i = 0; i < aliveHeroUnits.Count; i++)
        {
            aliveHeroUnits[i].RemoveTargetUnit(unit);
        }
    }

    // �ӵ��˶���Ŀ�����Ƴ���λ
    void RemoveUnitFromEnemyTeamTargets(UnitController unit)
    {
        for (int i = 0; i < aliveEnemyUnits.Count; i++)
        {
            aliveEnemyUnits[i].RemoveTargetUnit(unit);
        }
    }

    // ���ʣ�����
    void CheckRemainingTeams()
    {
        // ���Ӣ�۶���û�д�λ��������ս��ʧ��
        if (aliveHeroUnits.Count == 0)
        {
            SetBattleDefeat();
            //Debug.Log("ʧ�ܡ����Ӣ������ = " + aliveHeroUnits.Count + " ���������� = " + aliveEnemyUnits.Count);
        }
        // ������˶���û�д�λ��������ս��ʤ��
        else if (aliveEnemyUnits.Count == 0)
        {
            SetBattleVictory();
            //Debug.Log("ʤ�������Ӣ������ = " + aliveHeroUnits.Count + " ���������� = " + aliveEnemyUnits.Count);
        }
    }

    // ����ս��ʤ��
    void SetBattleVictory()
    {
        // ֹͣ���д���Ӣ�۵�λ
        StopAllAliveTeamUnits(aliveHeroUnits);

        if (victoryCutsceneBehaviour != null)
        {
            // ��ʼʤ����������
            victoryCutsceneBehaviour.StartTimeline();
        }

        // ֪ͨ��Ϸ��Ļ��ʾʤ��UI
        GameWon?.Invoke();
    }

    // ����ս��ʧ��
    void SetBattleDefeat()
    {
        // ֹͣ���д��ĵ��˵�λ
        StopAllAliveTeamUnits(aliveEnemyUnits);

        if (defeatCutsceneBehaviour != null)
        {
            // ��ʼʧ�ܹ�������
            defeatCutsceneBehaviour.StartTimeline();
        }

        // ֪ͨ��Ϸ��Ļ��ʾʧ��UI
        GameLost?.Invoke();
    }

    // ֹͣ���д��Ķ��鵥λ
    void StopAllAliveTeamUnits(List<UnitController> aliveTeamUnits)
    {
        for (int i = 0; i < aliveTeamUnits.Count; i++)
        {
            aliveTeamUnits[i].BattleEnded();
        }
    }

    // ѡ��ʤ�������һ������
    public void SelectVictoryNextScene()
    {
        selectedNextScene = victoryNextScene;
    }

    // ѡ��ʧ�ܺ����һ������
    public void SelectDefeatNextScene()
    {
        selectedNextScene = defeatNextScene;
    }

    // ����ѡ�еĳ���
    public void LoadSelectedScene()
    {
        NextSceneLoader sceneLoader = new NextSceneLoader();
        sceneLoader.LoadNextScene(selectedNextScene);
    }

}