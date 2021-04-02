/*
 * 脚本名称：Tree
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-02-10 09:33:43
 * 脚本作用：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Module
{
    public class Tree<T> : IDictionary<string, Tree<T>>
    {
        private Hashtable m_nodeTree;
        private Tree<T> m_parent;
        private Tree<T> m_root;
        private int m_count;
        private ICollection<string> m_keys;
        private ICollection<Tree<T>> m_value;

        public Tree<T> parent
        {
            get { return m_parent; }
        }
        public Tree<T> root
        {
            get { return m_root; }
        }
        public int Count { get; }
        public bool IsReadOnly { get; }
        public ICollection<string> Keys { get; }
        public ICollection<Tree<T>> Values { get; }

        public Tree<T> this[string key]
        {
            get
            {
                if (m_nodeTree.ContainsKey(key))
                {
                    return (Tree<T>) m_nodeTree[key];
                }

                return null;
            }
            set
            {
                if (m_nodeTree.ContainsKey(key))
                {
                    m_nodeTree[key] = value;
                }
                else
                {
                    m_nodeTree.Add(key,value);
                }
            }
        }
        
        public IEnumerator<KeyValuePair<string, Tree<T>>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, Tree<T>>>)m_nodeTree;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, Tree<T>> item)
        {
            Add(item.Key,item.Value);
        }

        public void Clear()
        {
            m_nodeTree.Clear();
        }

        public bool Contains(KeyValuePair<string, Tree<T>> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, Tree<T>>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<string, Tree<T>> item)
        {
            if (m_nodeTree.ContainsKey(item.Key))
            {
                m_nodeTree.Remove(item.Key);
                return true;
            }

            return false;
        }


        public void Add(string key, Tree<T> value)
        {
            m_nodeTree.Add(key,value);
            value.m_parent = this;
            //if(value.m)
            //value.parent = this;
        }

        public bool ContainsKey(string key)
        {
            return m_nodeTree.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (m_nodeTree.ContainsKey(key))
            {
                m_nodeTree.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(string key, out Tree<T> value)
        {
            if (m_nodeTree.ContainsKey(key))
            {
                value = (Tree<T>) m_nodeTree[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

    }
}
