using System.Collections;
using UnityEngine;

namespace Module
{
    public interface ICoroutine : ITimeCtrl
    {
        Coroutine StartCoroutine(IEnumerator c);
        void StopCoroutine(Coroutine c);
    }
}