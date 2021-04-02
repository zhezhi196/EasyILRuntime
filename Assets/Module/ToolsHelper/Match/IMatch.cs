using System.Collections.Generic;

namespace Module
{
    public interface IMatch
    {
        void OnMatchSuccess(IMatch[] target);
        bool CanMatch(IMatch target);
    }
}