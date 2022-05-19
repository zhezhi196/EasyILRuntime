namespace Module
{
    public class Subscribe: Iap
    {
        public string price { get; set; }

        public Subscribe(ISubscribeData data) : base(data)
        {
        }

        public override bool CanPay()
        {
            return true;
        }
    }
}