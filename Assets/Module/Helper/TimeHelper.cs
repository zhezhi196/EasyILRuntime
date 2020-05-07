using System;
using DG.Tweening;
using UnityEngine;

namespace Module
{
	public static class TimeHelper
	{
		private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
		/// <summary>
		/// 客户端时间
		/// </summary>
		/// <returns></returns>
		public static long ClientNow()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000;
		}

		public static long ClientNowSeconds()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000000;
		}

		public static long Now()
		{
			return ClientNow();
		}

		private static Tweener m_timeScaleAnimator;

		public static Tweener SetTimescale(float target, float dutaion)
		{
			if (m_timeScaleAnimator != null) m_timeScaleAnimator.Kill();
			m_timeScaleAnimator = DOTween.To((() => Time.timeScale), value =>
			{
				Time.timeScale = value;
				Time.fixedDeltaTime = Time.timeScale * 0.02f;
			}, target, dutaion).SetUpdate(true);


			return m_timeScaleAnimator;
		}
	}

	

}