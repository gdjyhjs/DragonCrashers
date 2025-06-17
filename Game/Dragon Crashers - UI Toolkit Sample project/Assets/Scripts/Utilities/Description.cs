using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// 这是一个用于保存注释或解释的简单组件。
    /// </summary>
    public class Description : MonoBehaviour
    {
        // 用于输入较长文本注释的字段
        [TextArea(10, 30)]
        public string note;
    }
}