using Module;

public class WeekBuff : Buff
{
    public override string name => "week";
    public float moveSpeedDown;
    public float attackDown;

    public override void OnInit(IBuffObject owner, BuffType type, object[] args)
    {
        base.OnInit(owner, type, args);
        this.attackDown = args[0].ToFloat();
        this.moveSpeedDown = args[1].ToFloat();
    }

    public override void OnAdd()
    {
        base.OnAdd();
        owner.GetAgentCtrl<AnimatorCtrl>().SetFloat("Normal", (1 - moveSpeedDown), 0.2f);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        owner.GetAgentCtrl<AnimatorCtrl>().SetFloat("Normal", 1, 0.2f);
    }
}