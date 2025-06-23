using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 仓库交互对象，玩家可与之交互打开仓库界面
    /// </summary>
    public class Warehouse : InteractiveObject
    {
        /// <summary>
        /// 当仓库被交互时调用，打开仓库UI界面
        /// </summary>
        public override void InteractedWith()
        {
            UIHandler.OpenWarehouse();
        }
    }
}