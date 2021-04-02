/*
 * 脚本名称：Net
 * 项目名称：Bow
 * 脚本作者：黄哲智
 * 创建时间：2018-06-21 17:13:35
 * 脚本作用：
*/

using System.Collections.Generic;

namespace Module
{
    public class Net<T> where T:new()
    {
        /// <summary>
        /// 节点key
        /// </summary>
        public string key { get; private set; }

        /// <summary>
        /// 节点值
        /// </summary>
        public T value { get; private set; }

        /// <summary>
        /// 上一批
        /// </summary>
        public List<Net<T>> previousNodes { get; private set; }

        /// <summary>
        /// 下一批节点
        /// </summary>
        public List<Net<T>> nextNodes { get; private set; }

        public Net(string key)
        {
            this.key = key;
            value=new T();
            nextNodes = new List<Net<T>>();
            previousNodes = new List<Net<T>>();

        }


        public void AddNextNode(Net<T> node)
        {
            if (!nextNodes.Contains(node))
            {
                nextNodes.Add(node);
            }
        }

        public void RemoveNextNode(Net<T> node)
        {
            if (nextNodes.Contains(node))
            {
                nextNodes.Remove(node);
            }
        }

        public void AddPreviousNode(Net<T> node)
        {
            if (!previousNodes.Contains(node))
            {
                previousNodes.Add(node);
            }
        }

        public void RemovePreviousNode(Net<T> node)
        {
            if (previousNodes.Contains(node))
            {
                previousNodes.Remove(node);
            }
        }
    }

}


