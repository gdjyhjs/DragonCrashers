namespace HappyHarvest
{
    /// <summary>
    /// 市场摊位交互对象，玩家可与之交互打开市场界面 
    /// </summary>
    public class MarketStall : InteractiveObject
    {
        /// <summary>
        /// 当摊位被交互时调用，打开市场UI界面
        /// </summary>
        public override void InteractedWith()
        {
            UIHandler.OpenMarket();
        }
    }
}