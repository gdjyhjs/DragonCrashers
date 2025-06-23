using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// ���������������ڼ���ָ��Ŀ�곡��
    /// ��Ҫ����ȷ���ڹ����汾����ȷ���� GameManager
    /// </summary>
    public class Loader : MonoBehaviour
    {
        // Ŀ�곡������
        public string TargetScene = "World";

        private void Start()
        {
            // ����Ŀ�곡��
            SceneManager.LoadScene(TargetScene);
        }
    }
}
