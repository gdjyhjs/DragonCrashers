using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// �ֿ⽻��������ҿ���֮�����򿪲ֿ����
    /// </summary>
    public class Warehouse : InteractiveObject
    {
        /// <summary>
        /// ���ֿⱻ����ʱ���ã��򿪲ֿ�UI����
        /// </summary>
        public override void InteractedWith()
        {
            UIHandler.OpenWarehouse();
        }
    }
}