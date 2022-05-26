using System;
using Module;
using Module.SkillAction;
public class ShotAction : ISkillAction
{
    public event Action<ISkillAction> onEnd;
    public event Action<ISkillAction, float> onUpdate;

    public async void Start()
    {
        await Async.WaitforSeconds(0.2f);
        End();
    }

    public void End()
    {
        onEnd?.Invoke(this);
    }

    public void Break()
    {
    }

    public void Pause()
    {
    }

    public void Continue()
    {
    }

    public void Dispose()
    {
    }

    public void OnUpdate()
    {
    }

    public void SetTimeCallback(float percent, Action<float> callback)
    {
    }
}