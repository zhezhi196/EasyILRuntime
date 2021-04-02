namespace Module
{
    public class Subscribe: Iap
    {
        public Subscribe(IapDataBase data) : base(data)
        {
        }

        public override bool CanPay()
        {
            return true;
        }
    }
}