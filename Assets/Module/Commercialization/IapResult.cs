namespace Module
{
    public enum IapResultMessage
    {
        Success,
        Fail,
        Cancle,
        NetworkError,
    }
    public class IapResult
    {
        public IapResultMessage result;
        public Iap iap;
        public Commodity[] commodity;

        public IapResult(Iap iap, params Commodity[] commodity)
        {
            this.iap = iap;
            this.commodity = commodity;
        }

        public Commodity GetReward(int index = 0)
        {
            if (commodity.IsNullOrEmpty()) return null;
            if (index > commodity.Length) return null;
            return commodity[index];
        }
    }
}