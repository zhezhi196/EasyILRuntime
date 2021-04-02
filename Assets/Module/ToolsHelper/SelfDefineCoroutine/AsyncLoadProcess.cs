/*
 * 脚本名称：AsyncLoadProcess
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-07 15:21:00
 * 脚本作用：
*/

using System.Collections;
using UnityEngine;

namespace Module
{
    public class AsyncLoadProcess : IEnumerator
    {
        private bool m_IsDone = false;
        public float Process
        {
            get { return Mathf.Clamp01(currentProcess / maxProcess); }
        }

        public int maxProcess = 100;

        private float currentProcess
        {
            get { return process1; }
        }

        private int process1;
        public bool IsDone
        {
            get { return m_IsDone; }
            set { m_IsDone = value; }
        }

        public bool MoveNext()
        {
            return !m_IsDone;
        }

        public void Reset()
        {
            m_IsDone = false;
        }

        public void SetComplete(int p = 1)
        {
            m_IsDone = true;
            process1 = Mathf.Clamp(process1 + 1, 0, maxProcess);
        }

        public object Current { get; private set; }
    }
}
