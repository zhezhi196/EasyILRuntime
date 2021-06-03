using System;

namespace Module
{
    public class FreeIap: Iap
    {
        public FreeIap(IapDataBase data) : base(data)
        {
        }

        public override string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            getCount++;
            result.result = IapResultMessage.Success;
            onResultIapBeforeCall?.Invoke(result);
            callback?.Invoke(result);
            onResultIapAfterCall?.Invoke(result);
            return null;
        }

        public override bool CanPay()
        {
            return true;
        }
    }
}