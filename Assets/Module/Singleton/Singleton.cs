/*
脚本名称:
脚本作者:黄哲智
建立时间:
脚本功能:
版本号:
*/

using System.Collections;
using UnityEngine;

namespace Module
{
    /// <summary>
    ///     普通脚本的单例
    /// </summary>
    /// <typeparam id="T"></typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T instance;

        public virtual void Dispose()
        {
            instance = null;
        }
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        // #region 开启协程
        //
        // public HotUpdate.Coroutine StartCoroutine(IEnumerator routine)
        // {
        //     return GamePlay.Instance.StartCoroutine(routine);
        // }
        //
        // public Coroutine StartCoroutine(string routine)
        // {
        //     return GamePlay.Instance.StartCoroutine(routine);
        // }
        //
        // public void StopCoroutine(Coroutine coroutine)
        // {
        //     GamePlay.Instance.StopCoroutine(coroutine);
        // }
        //
        // public void StopCoroutine(IEnumerator ie)
        // {
        //     GamePlay.Instance.StopCoroutine(ie);
        // }
        //
        // #endregion
    }
}
