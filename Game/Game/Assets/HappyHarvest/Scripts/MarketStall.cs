namespace HappyHarvest
{
    /// <summary>
    /// �г�̯λ����������ҿ���֮�������г����� 
    /// </summary>
    public class MarketStall : InteractiveObject
    {
        /// <summary>
        /// ��̯λ������ʱ���ã����г�UI����
        /// </summary>
        public override void InteractedWith()
        {
            UIHandler.OpenMarket();
        }
    }
}