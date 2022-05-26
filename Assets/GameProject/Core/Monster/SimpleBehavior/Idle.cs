using Module;

public class Idle: SimpleBehavior
{
    public override void OnStart(ISimpleBehaviorObject owner, object[] args)
    {
        base.OnStart(owner, args);
        if (!args.IsNullOrEmpty())
        {
            int idleType = args[0].ToInt();
            if (owner is Monster monster)
            {
                monster.Idle(idleType);
            }
        }
        else
        {
            if (owner is Monster monster)
            {
                monster.Idle();
            }
        }
    }
} 