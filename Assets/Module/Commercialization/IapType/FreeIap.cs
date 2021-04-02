using System;

namespace Module
{
    public class FreeIap: Iap
    {
        public FreeIap(IapDataBase data) : base(data)
        {
        }

        public override void OnTryGetReward(Action<IapResult> callback, IapResult result,bool skipConsume)
        {
            getCount++;
            result.result = IapResultMessage.Success;
            callback?.Invoke(result);
        }

        public override bool CanPay()
        {
            return true;
        }
    }
}