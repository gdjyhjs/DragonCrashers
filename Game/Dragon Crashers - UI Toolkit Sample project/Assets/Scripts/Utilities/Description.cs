using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// ����һ�����ڱ���ע�ͻ���͵ļ������
    /// </summary>
    public class Description : MonoBehaviour
    {
        // ��������ϳ��ı�ע�͵��ֶ�
        [TextArea(10, 30)]
        public string note;
    }
}