using System.Collections;
using System.Collections.Generic;

namespace Module
{
    public interface INet<T> : IEnumerable<T>
    {
        List<T> nextNode { get; set; }
        List<T> priviousNode { get; set; }
        void AddNextNode(T node);
        void RemoveNextNode(T node);
        bool ContainNextNode(T node);
    }
}