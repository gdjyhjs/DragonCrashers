using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// �������ɴ�����������ʵ�ֳ����л�����
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SceneTransition : MonoBehaviour
    {
        // Ŀ�곡���Ĺ�������
        public string TargetSceneBuildIndex = "World";
        // Ŀ�곡���е����ɵ�����
        public int TargetSpawnIndex;

        /// <summary>
        /// ����ײ����봥����ʱ������������
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            // ���� GameManager �ĳ����л�����
            GameManager.Instance.MoveTo(TargetSceneBuildIndex, TargetSpawnIndex);
        }
    }
}