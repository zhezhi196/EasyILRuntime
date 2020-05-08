using System.Runtime.CompilerServices;

namespace Module
{
    public interface IAwaiter: INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
        void SetResult();
    }
}
