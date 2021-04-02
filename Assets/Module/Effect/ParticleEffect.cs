using Module;
using UnityEngine;

public class ParticleEffect : TimeEffect
{
    public ParticleSystem particle;
    public float speed = 1;

    public override void StartFirst()
    {
        if (duation == 0)
        {
            duation = TimeHelper.GetTime(particle.main.duration + particle.main.startDelay.constantMax);
            GameDebug.LogWarn($"{gameObject.name}没有定义时间，可能会有偏差");
        }
    }

    public override void Restart()
    {
        if (particle != null) particle.Simulate(0, true, true);
    }

    public override void Simulate(float time)
    {
        if (particle == null) return;
        particle.Simulate((play.ignoreTimeScale ? GetUnscaleDelatime(play.ignorePause) : GetDelatime(play.ignorePause))*speed, true, false);
        base.Simulate(time);
    }


}
